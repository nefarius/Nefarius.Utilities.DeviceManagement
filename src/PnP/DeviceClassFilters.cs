using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.System.Registry;

using Microsoft.Win32.SafeHandles;

using Nefarius.Utilities.DeviceManagement.Exceptions;
using Nefarius.Utilities.DeviceManagement.Util;

namespace Nefarius.Utilities.DeviceManagement.PnP;

/// <summary>
///     Utility class to adjust class filter settings.
/// </summary>
[SuppressMessage("ReSharper", "UnusedMember.Global")]
public sealed class DeviceClassFilters
{
    internal DeviceClassFilters() { }

    /// <summary>
    ///     Adds an entry to the device class upper filters.
    /// </summary>
    /// <param name="classGuid">The device class GUID to modify.</param>
    /// <param name="service">The driver service name to add.</param>
    /// <remarks>If the provided service entry already exists, it will not get added again.</remarks>
    public static void AddUpper(Guid classGuid, string service)
    {
        AddFiltersEntry(classGuid, "UpperFilters", service);
    }

    /// <summary>
    ///     Removes an entry from the device class upper filters.
    /// </summary>
    /// <param name="classGuid">The device class GUID to modify.</param>
    /// <param name="service">The driver service name to add.</param>
    /// <remarks>
    ///     If the provided service entry doesn't exist or the entire filter value is not present, this method does
    ///     nothing.
    /// </remarks>
    public static void RemoveUpper(Guid classGuid, string service)
    {
        RemoveFiltersEntry(classGuid, "UpperFilters", service);
    }

    /// <summary>
    ///     Adds an entry to the device class lower filters.
    /// </summary>
    /// <param name="classGuid">The device class GUID to modify.</param>
    /// <param name="service">The driver service name to add.</param>
    /// <remarks>If the provided service entry already exists, it will not get added again.</remarks>
    public static void AddLower(Guid classGuid, string service)
    {
        AddFiltersEntry(classGuid, "LowerFilters", service);
    }

    /// <summary>
    ///     Removes an entry from the device class lower filters.
    /// </summary>
    /// <param name="classGuid">The device class GUID to modify.</param>
    /// <param name="service">The driver service name to add.</param>
    /// <remarks>
    ///     If the provided service entry doesn't exist or the entire filter value is not present, this method does
    ///     nothing.
    /// </remarks>
    public static void RemoveLower(Guid classGuid, string service)
    {
        RemoveFiltersEntry(classGuid, "LowerFilters", service);
    }

    private static unsafe void AddFiltersEntry(Guid classGuid, string filter, string service)
    {
        using SafeRegistryHandle key = PInvoke.SetupDiOpenClassRegKey(classGuid, (uint)REG_SAM_FLAGS.KEY_ALL_ACCESS);

        if (key.IsInvalid)
        {
            throw new Win32Exception("Failed to open class registry key");
        }

        REG_VALUE_TYPE type;
        uint sizeRequired = 0;

        WIN32_ERROR status = PInvoke.RegQueryValueEx(
            key,
            filter,
            &type,
            null,
            &sizeRequired
        );

        // value exists
        if (status == WIN32_ERROR.ERROR_SUCCESS)
        {
            byte* buffer = stackalloc byte[(int)sizeRequired];

            status = PInvoke.RegQueryValueEx(
                key,
                filter,
                &type,
                buffer,
                &sizeRequired
            );

            if (status != WIN32_ERROR.ERROR_SUCCESS)
            {
                throw new Win32Exception("Failed to query value");
            }

            List<string> elements = ((IntPtr)buffer).MultiSzPointerToStringArray((int)sizeRequired).ToList();

            elements.Add(service);

            IntPtr rawBuffer = elements
                // strip empty entries
                .Where(e => !string.IsNullOrWhiteSpace(e))
                // remove duplicates
                .Distinct()
                .StringArrayToMultiSzPointer(out int length);

            status = PInvoke.RegSetValueEx(
                key,
                filter,
                type,
                new ReadOnlySpan<byte>(rawBuffer.ToPointer(), length)
            );

            if (status != WIN32_ERROR.ERROR_SUCCESS)
            {
                throw new Win32Exception("Failed to write value");
            }

            return;
        }

        if (status == WIN32_ERROR.ERROR_FILE_NOT_FOUND)
        {
            type = REG_VALUE_TYPE.REG_MULTI_SZ;
            List<string> elements = new() { service };

            IntPtr rawBuffer = elements
                // strip empty entries
                .Where(e => !string.IsNullOrWhiteSpace(e))
                // remove duplicates
                .Distinct()
                .StringArrayToMultiSzPointer(out int length);

            status = PInvoke.RegSetValueEx(
                key,
                filter,
                type,
                new ReadOnlySpan<byte>(rawBuffer.ToPointer(), length)
            );

            if (status != WIN32_ERROR.ERROR_SUCCESS)
            {
                throw new Win32Exception("Failed to write value");
            }

            return;
        }

        throw new Win32Exception("Unexpected failure", (int)status);
    }

    private static unsafe void RemoveFiltersEntry(Guid classGuid, string filter, string service)
    {
        using SafeRegistryHandle key = PInvoke.SetupDiOpenClassRegKey(classGuid, (uint)REG_SAM_FLAGS.KEY_ALL_ACCESS);

        if (key.IsInvalid)
        {
            throw new Win32Exception("Failed to open class registry key");
        }

        REG_VALUE_TYPE type;
        uint sizeRequired = 0;

        WIN32_ERROR status = PInvoke.RegQueryValueEx(
            key,
            filter,
            &type,
            null,
            &sizeRequired
        );

        // value exists
        if (status == WIN32_ERROR.ERROR_SUCCESS)
        {
            byte* buffer = stackalloc byte[(int)sizeRequired];

            status = PInvoke.RegQueryValueEx(
                key,
                filter,
                &type,
                buffer,
                &sizeRequired
            );

            if (status != WIN32_ERROR.ERROR_SUCCESS)
            {
                throw new Win32Exception("Failed to query value");
            }

            List<string> elements = ((IntPtr)buffer).MultiSzPointerToStringArray((int)sizeRequired).ToList();

            elements.RemoveAll(e => e.Equals(service, StringComparison.OrdinalIgnoreCase));

            IntPtr rawBuffer = elements
                // strip empty entries
                .Where(e => !string.IsNullOrWhiteSpace(e))
                // remove duplicates
                .Distinct()
                .StringArrayToMultiSzPointer(out int length);

            status = PInvoke.RegSetValueEx(
                key,
                filter,
                type,
                new ReadOnlySpan<byte>(rawBuffer.ToPointer(), length)
            );

            if (status != WIN32_ERROR.ERROR_SUCCESS)
            {
                throw new Win32Exception("Failed to write value");
            }

            return;
        }

        // nothing to do for us then
        if (status == WIN32_ERROR.ERROR_FILE_NOT_FOUND)
        {
            return;
        }

        throw new Win32Exception("Unexpected failure", (int)status);
    }
}