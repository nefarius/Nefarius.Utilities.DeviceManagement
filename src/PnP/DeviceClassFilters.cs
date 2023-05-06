#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;

using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.Security;
using Windows.Win32.Storage.FileSystem;
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
    private const string UpperFilters = "UpperFilters";
    private const string LowerFilters = "LowerFilters";
    internal DeviceClassFilters() { }

    /// <summary>
    ///     Adds an entry to the device class upper filters.
    /// </summary>
    /// <param name="classGuid">The device class GUID to modify.</param>
    /// <param name="service">The driver service name to add.</param>
    /// <remarks>
    ///     If the filters value doesn't exist, it will get created. If the provided service entry already exists, it will
    ///     not get added again.
    /// </remarks>
    /// <exception cref="Win32Exception" />
    public static void AddUpper(Guid classGuid, string service)
    {
        AddFiltersEntry(classGuid, UpperFilters, service);
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
    /// <exception cref="Win32Exception" />
    public static void RemoveUpper(Guid classGuid, string service)
    {
        RemoveFiltersEntry(classGuid, UpperFilters, service);
    }

    /// <summary>
    ///     Returns the list of device class upper filter services configured, or null of the value doesn't exist at all.
    /// </summary>
    /// <param name="classGuid">The device class GUID to query.</param>
    /// <returns>A list of service names or null.</returns>
    /// <exception cref="Win32Exception" />
    public static IEnumerable<string>? GetUpper(Guid classGuid)
    {
        return GetFiltersEntry(classGuid, UpperFilters);
    }

    /// <summary>
    ///     Deletes the entire upper filters value for the provided device class.
    /// </summary>
    /// <param name="classGuid">The device class GUID to delete the value for.</param>
    /// <remarks>If the value doesn't exist, this method does nothing.</remarks>
    /// <exception cref="Win32Exception" />
    public static void DeleteUpper(Guid classGuid)
    {
        DeleteFiltersEntry(classGuid, UpperFilters);
    }

    /// <summary>
    ///     Adds an entry to the device class lower filters.
    /// </summary>
    /// <param name="classGuid">The device class GUID to modify.</param>
    /// <param name="service">The driver service name to add.</param>
    /// <remarks>
    ///     If the filters value doesn't exist, it will get created. If the provided service entry already exists, it will
    ///     not get added again.
    /// </remarks>
    /// <exception cref="Win32Exception" />
    public static void AddLower(Guid classGuid, string service)
    {
        AddFiltersEntry(classGuid, LowerFilters, service);
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
    /// <exception cref="Win32Exception" />
    public static void RemoveLower(Guid classGuid, string service)
    {
        RemoveFiltersEntry(classGuid, LowerFilters, service);
    }

    /// <summary>
    ///     Returns the list of device class lower filter services configured, or null of the value doesn't exist at all.
    /// </summary>
    /// <param name="classGuid">The device class GUID to query.</param>
    /// <returns>A list of service names or null.</returns>
    /// <exception cref="Win32Exception" />
    public static IEnumerable<string>? GetLower(Guid classGuid)
    {
        return GetFiltersEntry(classGuid, LowerFilters);
    }

    /// <summary>
    ///     Deletes the entire lower filters value for the provided device class.
    /// </summary>
    /// <param name="classGuid">The device class GUID to delete the value for.</param>
    /// <remarks>If the value doesn't exist, this method does nothing.</remarks>
    /// <exception cref="Win32Exception" />
    public static void DeleteLower(Guid classGuid)
    {
        DeleteFiltersEntry(classGuid, LowerFilters);
    }

    private static unsafe void AddFiltersEntry(Guid classGuid, string filter, string service)
    {
        const uint serviceFlags = (uint)FILE_ACCESS_RIGHTS.STANDARD_RIGHTS_REQUIRED | PInvoke.SC_MANAGER_CONNECT |
                                  PInvoke.SC_MANAGER_ENUMERATE_SERVICE;

        SC_HANDLE serviceManager = PInvoke.OpenSCManager(null, "ServicesActive", serviceFlags);

        if (serviceManager.Value == 0)
        {
            throw new Win32Exception("Failed to open service controller");
        }

        SC_HANDLE serviceHandle =
            PInvoke.OpenService(serviceManager, service, serviceFlags);

        if (serviceHandle.Value == 0)
        {
            WIN32_ERROR error = (WIN32_ERROR)Marshal.GetLastWin32Error();

            PInvoke.CloseServiceHandle(serviceManager);

            if (error == WIN32_ERROR.ERROR_SERVICE_DOES_NOT_EXIST)
            {
                throw new DriverServiceNotFoundException($"{service} is not a valid service on this machine",
                    (int)error);
            }

            throw new Win32Exception("Failed to open service handle");
        }

        PInvoke.CloseServiceHandle(serviceManager);
        PInvoke.CloseServiceHandle(serviceHandle);

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

    private static unsafe IEnumerable<string>? GetFiltersEntry(Guid classGuid, string filter)
    {
        using SafeRegistryHandle key = PInvoke.SetupDiOpenClassRegKey(classGuid, (uint)REG_SAM_FLAGS.KEY_READ);

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

        switch (status)
        {
            // value exists
            case WIN32_ERROR.ERROR_SUCCESS:
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

                    return ((IntPtr)buffer).MultiSzPointerToStringArray((int)sizeRequired);
                }
            case WIN32_ERROR.ERROR_FILE_NOT_FOUND:
                return null;
            default:
                throw new Win32Exception("Unexpected failure", (int)status);
        }
    }

    private static void DeleteFiltersEntry(Guid classGuid, string filter)
    {
        using SafeRegistryHandle key = PInvoke.SetupDiOpenClassRegKey(classGuid, (uint)REG_SAM_FLAGS.KEY_ALL_ACCESS);

        if (key.IsInvalid)
        {
            throw new Win32Exception("Failed to open class registry key");
        }

        WIN32_ERROR status = PInvoke.RegDeleteValue(key, filter);

        // success 
        if (status is WIN32_ERROR.ERROR_SUCCESS or WIN32_ERROR.ERROR_FILE_NOT_FOUND)
        {
            return;
        }

        throw new Win32Exception("Unexpected failure", (int)status);
    }
}