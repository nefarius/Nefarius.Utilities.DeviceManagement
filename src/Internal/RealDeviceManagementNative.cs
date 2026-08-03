using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using Windows.Win32;
using Windows.Win32.Devices.DeviceAndDriverInstallation;
using Windows.Win32.Foundation;

using Nefarius.Utilities.DeviceManagement.Exceptions;
using Nefarius.Utilities.DeviceManagement.PnP;

namespace Nefarius.Utilities.DeviceManagement.Internal;

/// <summary>
///     Production <see cref="IDeviceManagementNative" /> backed by SetupAPI / CfgMgr32.
/// </summary>
internal sealed class RealDeviceManagementNative : IDeviceManagementNative
{
    public static RealDeviceManagementNative Instance { get; } = new();

    private RealDeviceManagementNative()
    {
    }

    /// <inheritdoc />
    public unsafe IEnumerable<string> EnumerateDeviceInstanceIds(Guid classGuid, bool presentOnly)
    {
        List<string> instanceIds = new();
        SetupApi.SP_DEVINFO_DATA deviceInfoData = new();
        deviceInfoData.cbSize = Marshal.SizeOf(deviceInfoData);
        Guid target = classGuid;
        HDEVINFO deviceInfoSet = SetupApi.SetupDiGetClassDevs(
            ref target,
            IntPtr.Zero,
            HWND.Null,
            presentOnly ? (uint)SETUP_DI_GET_CLASS_DEVS_FLAGS.DIGCF_PRESENT : 0
        );

        try
        {
            for (
                uint i = 0;
                SetupApi.SetupDiEnumDeviceInfo(deviceInfoSet, i, ref deviceInfoData);
                i++
            )
            {
                CONFIGRET ret = PInvoke.CM_Get_Device_ID_Size(out uint charsRequired, deviceInfoData.DevInst, 0);

                if (ret != CONFIGRET.CR_SUCCESS)
                {
                    throw new ConfigManagerException("Failed to get device ID size.", ret);
                }

                uint nBytes = (charsRequired + 1) * 2;
#pragma warning disable CA2014
                // ReSharper disable once StackAllocInsideLoop
                char* ptrInstanceBuf = stackalloc char[(int)nBytes];
#pragma warning restore CA2014

                ret = PInvoke.CM_Get_Device_IDW(deviceInfoData.DevInst, ptrInstanceBuf, charsRequired, 0);

                if (ret != CONFIGRET.CR_SUCCESS)
                {
                    throw new ConfigManagerException("Failed to get device ID.", ret);
                }

                instanceIds.Add(new string(ptrInstanceBuf).ToUpperInvariant());
            }
        }
        finally
        {
            if (deviceInfoSet != IntPtr.Zero)
            {
                SetupApi.SetupDiDestroyDeviceInfoList(deviceInfoSet);
            }
        }

        return instanceIds;
    }

    /// <inheritdoc />
    public string[]? GetHardwareIds(string instanceId, bool presentOnly)
    {
        PnPDevice device = PnPDevice.GetDeviceByInstanceId(
            instanceId,
            presentOnly ? DeviceLocationFlags.Normal : DeviceLocationFlags.Phantom
        );

        return device.GetProperty<string[]>(DevicePropertyKey.Device_HardwareIds);
    }

    /// <inheritdoc />
    public string? GetParentInstanceId(string instanceId)
    {
        PnPDevice device = PnPDevice.GetDeviceByInstanceId(instanceId, DeviceLocationFlags.Phantom);
        return device.GetProperty<string>(DevicePropertyKey.Device_Parent);
    }
}
