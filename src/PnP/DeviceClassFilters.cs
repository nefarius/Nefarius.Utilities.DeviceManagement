using System;
using System.Collections.Generic;
using System.Linq;

using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.System.Registry;

using Microsoft.Win32.SafeHandles;

using Nefarius.Utilities.DeviceManagement.Exceptions;
using Nefarius.Utilities.DeviceManagement.Util;

namespace Nefarius.Utilities.DeviceManagement.PnP;

public sealed class DeviceClassFilters
{
    internal DeviceClassFilters() { }

    public static unsafe void AddUpper(Guid classGuid, string service)
    {
        using SafeRegistryHandle key = PInvoke.SetupDiOpenClassRegKey(classGuid, (uint)REG_SAM_FLAGS.KEY_ALL_ACCESS);

        if (key.IsInvalid)
        {
            throw new Win32Exception("Failed to open class registry key");
        }

        const string filter = "UpperFilters";
        REG_VALUE_TYPE type;
        uint sizeRequired = 0;

        var status = PInvoke.RegQueryValueEx(
            key,
            filter,
            &type,
            null,
            &sizeRequired
        );

        // value exists
        if (status == WIN32_ERROR.ERROR_SUCCESS)
        {
            var buffer = stackalloc byte[(int)sizeRequired];

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
        }
    }

    public static void RemoveUpper()
    {
    }

    public static void AddLower()
    {
    }

    public static void RemoveLower()
    {
    }
}