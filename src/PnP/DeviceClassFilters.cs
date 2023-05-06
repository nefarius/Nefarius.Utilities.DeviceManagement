using System;

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

        const string filter = "LowerFilters";
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

            var elements = ((IntPtr)buffer).MultiSzPointerToStringArray((int)sizeRequired);
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