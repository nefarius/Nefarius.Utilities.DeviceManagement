using System;
using System.Collections.Generic;

using Windows.Win32;
using Windows.Win32.Devices.DeviceAndDriverInstallation;

using Nefarius.Utilities.DeviceManagement.Exceptions;
using Nefarius.Utilities.DeviceManagement.PnP;

namespace Nefarius.Utilities.DeviceManagement.Internal;

/// <summary>
///     Implements <see cref="Devcon.EnumerateDevices" /> on top of <c>CM_Get_Device_ID_List</c> and device properties.
/// </summary>
internal static class DeviceEnumerator
{
    // DEVPKEY_Device_DeviceDesc / DEVPKEY_Device_FriendlyName (not exposed as generated keys)
    private static readonly Guid DeviceCategory = new(0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0);
    private static readonly DevicePropertyKey DeviceDescKey = new(DeviceCategory, 2, typeof(string));
    private static readonly DevicePropertyKey FriendlyNameKey = new(DeviceCategory, 14, typeof(string));

    public static IReadOnlyList<DeviceSummary> Enumerate(bool presentOnly)
    {
        List<string> allIds = GetDeviceIdList(presentOnly);
        HashSet<string> presentIds = presentOnly
            ? new HashSet<string>(allIds, StringComparer.OrdinalIgnoreCase)
            : new HashSet<string>(GetDeviceIdList(true), StringComparer.OrdinalIgnoreCase);

        List<DeviceSummary> result = new(allIds.Count);

        foreach (string instanceId in allIds)
        {
            bool isPresent = presentIds.Contains(instanceId);

            try
            {
                PnPDevice device = PnPDevice.GetDeviceByInstanceId(instanceId,
                    isPresent ? DeviceLocationFlags.Normal : DeviceLocationFlags.Phantom);

                string? friendly = SafeGet<string>(device, FriendlyNameKey);
                string? desc = string.IsNullOrEmpty(friendly)
                    ? SafeGet<string>(device, DeviceDescKey)
                    : friendly;
                string[]? hwIds = SafeGet<string[]>(device, DevicePropertyKey.Device_HardwareIds);
                string? infPath = SafeGet<string>(device, DevicePropertyKey.Device_DriverInfPath);
                string? service = SafeGet<string>(device, DevicePropertyKey.Device_Service);
                string? className = SafeGet<string>(device, DevicePropertyKey.Device_Class);

                result.Add(new DeviceSummary(
                    instanceId.ToUpperInvariant(),
                    string.IsNullOrEmpty(desc) ? null : desc,
                    SafeGetStruct<Guid>(device, DevicePropertyKey.Device_ClassGuid) ?? Guid.Empty,
                    string.IsNullOrEmpty(className) ? null : className,
                    hwIds ?? Array.Empty<string>(),
                    string.IsNullOrEmpty(service) ? null : service,
                    string.IsNullOrEmpty(infPath) ? null : infPath,
                    isPresent));
            }
            catch (PnPDeviceNotFoundException)
            {
                // device vanished between list retrieval and property reads
            }
            catch (ConfigManagerException)
            {
                // device became inaccessible
            }
        }

        return result;
    }

    private static T? SafeGet<T>(PnPDevice device, DevicePropertyKey key) where T : class
    {
        try
        {
            return device.GetProperty<T>(key);
        }
        catch (ConfigManagerException)
        {
            return null;
        }
    }

    private static T? SafeGetStruct<T>(PnPDevice device, DevicePropertyKey key) where T : struct
    {
        try
        {
            return device.GetProperty<T>(key);
        }
        catch (ConfigManagerException)
        {
            return null;
        }
    }

    private static unsafe List<string> GetDeviceIdList(bool presentOnly)
    {
        uint flags = presentOnly ? PInvoke.CM_GETIDLIST_FILTER_PRESENT : 0;

        // The list can grow between size query and retrieval (CR_BUFFER_SMALL); retry a few times.
        for (int attempt = 0; attempt < 5; attempt++)
        {
            CONFIGRET ret = PInvoke.CM_Get_Device_ID_List_Size(out uint length, null, flags);

            if (ret != CONFIGRET.CR_SUCCESS)
            {
                throw new ConfigManagerException("Failed to get device ID list size.", ret);
            }

            char[] buffer = new char[length + 1];

            fixed (char* p = buffer)
            {
                ret = PInvoke.CM_Get_Device_ID_List((char*)null, p, (uint)buffer.Length, flags);
            }

            if (ret == CONFIGRET.CR_BUFFER_SMALL)
            {
                continue;
            }

            if (ret != CONFIGRET.CR_SUCCESS)
            {
                throw new ConfigManagerException("Failed to get device ID list.", ret);
            }

            return ParseMultiSz(buffer);
        }

        throw new ConfigManagerException("Failed to get device ID list.", CONFIGRET.CR_BUFFER_SMALL);
    }

    internal static List<string> ParseMultiSz(char[] buffer)
    {
        List<string> ids = new();
        int start = 0;

        for (int i = 0; i < buffer.Length; i++)
        {
            if (buffer[i] != '\0')
            {
                continue;
            }

            if (i == start)
            {
                break; // empty string terminates the multi-sz list
            }

            ids.Add(new string(buffer, start, i - start));
            start = i + 1;
        }

        return ids;
    }
}