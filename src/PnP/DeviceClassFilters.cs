using System;

using Windows.Win32;
using Windows.Win32.System.Registry;

using Microsoft.Win32.SafeHandles;

using Nefarius.Utilities.DeviceManagement.Exceptions;

namespace Nefarius.Utilities.DeviceManagement.PnP;

public sealed class DeviceClassFilters
{
    internal DeviceClassFilters() { }

    public static void AddUpper(Guid classGuid, string service)
    {
        SafeRegistryHandle key = PInvoke.SetupDiOpenClassRegKey(classGuid, (uint)REG_SAM_FLAGS.KEY_ALL_ACCESS);

        if (key.IsInvalid)
        {
            throw new Win32Exception("Failed to open class registry key");
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