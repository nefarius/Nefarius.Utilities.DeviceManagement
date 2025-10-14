﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;

using Windows.Win32;
using Windows.Win32.Devices.DeviceAndDriverInstallation;
using Windows.Win32.Foundation;

using Nefarius.Utilities.DeviceManagement.Exceptions;

using Win32Exception = System.ComponentModel.Win32Exception;

// ReSharper disable InvertIf

namespace Nefarius.Utilities.DeviceManagement.PnP;

/// <summary>
///     "Device Console" utility class. Managed wrapper for common SetupAPI actions.
/// </summary>
/// <remarks>https://docs.microsoft.com/en-us/windows-hardware/drivers/install/setupapi</remarks>
[SuppressMessage("ReSharper", "UnusedMember.Global")]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
[SuppressMessage("ReSharper", "OutParameterValueIsAlwaysDiscarded.Global")]
public static class Devcon
{
    /// <summary>
    ///     Attempts to find a device within a specified device class by a given hardware ID.
    /// </summary>
    /// <param name="target">The device class GUID.</param>
    /// <param name="hardwareId">The hardware ID to search for.</param>
    /// <returns>True if found, false otherwise.</returns>
    public static bool FindInDeviceClassByHardwareId(Guid target, string hardwareId)
    {
        return FindInDeviceClassByHardwareId(target, hardwareId, out _);
    }

    /// <summary>
    ///     Attempts to find a device within a specified device class by a given hardware ID.
    /// </summary>
    /// <param name="target">The device class GUID.</param>
    /// <param name="hardwareId">The hardware ID to search for.</param>
    /// <param name="instanceIds">A list of instances found for the given search criteria.</param>
    /// <returns>True if found, false otherwise.</returns>
    public static bool FindInDeviceClassByHardwareId(Guid target, string hardwareId,
        out IEnumerable<string> instanceIds)
    {
        return FindInDeviceClassByHardwareId(target, hardwareId, out instanceIds, false /* backwards compatibility */);
    }

    /// <summary>
    ///     Attempts to find a device within a specified device class by a given hardware ID.
    /// </summary>
    /// <param name="target">The device class GUID.</param>
    /// <param name="hardwareId">The hardware ID to search for.</param>
    /// <param name="instanceIds">A list of instances found for the given search criteria.</param>
    /// <param name="presentOnly">True to filter currently plugged in devices, false to get all matching devices.</param>
    /// <param name="allowPartial">True to match substrings, false to match the exact ID value.</param>
    /// <returns>True if found, false otherwise.</returns>
    public static unsafe bool FindInDeviceClassByHardwareId(Guid target, string hardwareId,
        out IEnumerable<string> instanceIds, bool presentOnly, bool allowPartial = false /* backwards compatibility */)
    {
        instanceIds = new List<string>();
        bool found = false;
        SetupApi.SP_DEVINFO_DATA deviceInfoData = new();
        deviceInfoData.cbSize = Marshal.SizeOf(deviceInfoData);
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

                string instanceId = new string(ptrInstanceBuf).ToUpperInvariant();

                PnPDevice device = PnPDevice.GetDeviceByInstanceId(
                    instanceId,
                    presentOnly
                        ? DeviceLocationFlags.Normal
                        : DeviceLocationFlags.Phantom
                );

                string[] property = device.GetProperty<string[]>(DevicePropertyKey.Device_HardwareIds);

                if (property is null)
                {
                    continue;
                }

                List<string> hardwareIds = property.Select(id => id.ToUpperInvariant()).ToList();

                if (
                    /* partial match */
                    (allowPartial && hardwareIds.Any(id => id.Contains(hardwareId.ToUpperInvariant()))) ||
                    /* exact match */
                    (!allowPartial && hardwareIds.Contains(hardwareId, StringComparer.OrdinalIgnoreCase))
                )
                {
                    ((List<string>)instanceIds).Add(instanceId);
                    found = true;
                }
            }
        }
        finally
        {
            if (deviceInfoSet != IntPtr.Zero)
            {
                SetupApi.SetupDiDestroyDeviceInfoList(deviceInfoSet);
            }
        }

        return found;
    }

    /// <summary>
    ///     Searches for devices matching the provided interface GUID and returns the device path and instance ID.
    /// </summary>
    /// <param name="target">The interface GUID to enumerate.</param>
    /// <param name="path">The device path of the enumerated device.</param>
    /// <param name="instanceId">The instance ID of the enumerated device.</param>
    /// <param name="instance">Optional instance ID (zero-based) specifying the device to process on multiple matches.</param>
    /// <param name="presentOnly">
    ///     Only enumerate currently connected devices by default, set to False to also include phantom
    ///     devices.
    /// </param>
    /// <returns>True if at least one device was found with the provided class, false otherwise.</returns>
    public static unsafe bool FindByInterfaceGuid(Guid target, out string path, out string instanceId, int instance = 0,
        bool presentOnly = true)
    {
        HDEVINFO deviceInfoSet = HDEVINFO.Null;

        try
        {
            SetupApi.SP_DEVINFO_DATA deviceInterfaceData = new(),
                da = new();
            int bufferSize = 0, memberIndex = 0;

            int flags = (int)SETUP_DI_GET_CLASS_DEVS_FLAGS.DIGCF_DEVICEINTERFACE;

            if (presentOnly)
            {
                flags |= (int)SETUP_DI_GET_CLASS_DEVS_FLAGS.DIGCF_PRESENT;
            }

            deviceInfoSet = SetupApi.SetupDiGetClassDevs(ref target, IntPtr.Zero, HWND.Null, (uint)flags);

            deviceInterfaceData.cbSize = da.cbSize = Marshal.SizeOf(deviceInterfaceData);

            while (SetupApi.SetupDiEnumDeviceInterfaces(deviceInfoSet, IntPtr.Zero, ref target, memberIndex,
                       ref deviceInterfaceData))
            {
                SetupApi.SetupDiGetDeviceInterfaceDetail(
                    deviceInfoSet,
                    ref deviceInterfaceData,
                    IntPtr.Zero,
                    0,
                    ref bufferSize,
                    ref da
                );
                {
                    IntPtr detailDataBuffer = Marshal.AllocHGlobal(bufferSize);

                    Marshal.WriteInt32(detailDataBuffer,
                        IntPtr.Size == 4 ? 4 + Marshal.SystemDefaultCharSize : 8);

                    if (SetupApi.SetupDiGetDeviceInterfaceDetail(deviceInfoSet, ref deviceInterfaceData,
                            detailDataBuffer,
                            bufferSize, ref bufferSize, ref da))
                    {
                        IntPtr pDevicePathName = detailDataBuffer + 4;

                        path = (Marshal.PtrToStringAuto(pDevicePathName) ?? string.Empty).ToUpperInvariant();

                        if (memberIndex == instance)
                        {
                            CONFIGRET ret = PInvoke.CM_Get_Device_ID_Size(out uint charsRequired, da.DevInst, 0);

                            if (ret != CONFIGRET.CR_SUCCESS)
                            {
                                throw new ConfigManagerException("Failed to get device ID size.", ret);
                            }

                            uint nBytes = (charsRequired + 1) * 2;
                            // ReSharper disable once StackAllocInsideLoop
                            char* ptrInstanceBuf = stackalloc char[(int)nBytes];

                            PInvoke.CM_Get_Device_IDW(da.DevInst, ptrInstanceBuf, nBytes, 0);
                            instanceId = new string(ptrInstanceBuf).ToUpperInvariant();

                            return true;
                        }
                    }
                    else
                    {
                        Marshal.FreeHGlobal(detailDataBuffer);
                    }
                }

                memberIndex++;
            }
        }
        finally
        {
            if (deviceInfoSet != HDEVINFO.Null)
            {
                SetupApi.SetupDiDestroyDeviceInfoList(deviceInfoSet);
            }
        }

        path = instanceId = string.Empty;
        return false;
    }

    /// <summary>
    ///     Searches for devices matching the provided interface GUID and returns a <see cref="PnPDevice" />.
    /// </summary>
    /// <param name="target">The interface GUID to enumerate.</param>
    /// <param name="device">The <see cref="PnPDevice" /> wrapper object.</param>
    /// <param name="instance">Optional instance ID (zero-based) specifying the device to process on multiple matches.</param>
    /// <param name="presentOnly">
    ///     Only enumerate currently connected devices by default, set to False to also include phantom
    ///     devices.
    /// </param>
    /// <returns>True if at least one device was found with the provided class, false otherwise.</returns>
    public static bool FindByInterfaceGuid(Guid target, out PnPDevice device, int instance = 0,
        bool presentOnly = true)
    {
        bool ret = FindByInterfaceGuid(target, out _, out string instanceId, instance, presentOnly);

        device = PnPDevice.GetDeviceByInstanceId(
            instanceId,
            presentOnly
                ? DeviceLocationFlags.Normal
                : DeviceLocationFlags.Phantom
        );

        return ret;
    }

    /// <summary>
    ///     Searches for devices matching the provided interface GUID and returns the device path and instance ID.
    /// </summary>
    /// <param name="target">The class GUID to enumerate.</param>
    /// <param name="path">The device path of the enumerated device.</param>
    /// <param name="instanceId">The instance ID of the enumerated device.</param>
    /// <param name="instance">Optional instance ID (zero-based) specifying the device to process on multiple matches.</param>
    /// <returns>True if at least one device was found with the provided class, false otherwise.</returns>
    /// <remarks>
    ///     This is here for backwards compatibility, please use
    ///     <see cref="FindByInterfaceGuid(System.Guid,out string,out string,int,bool)" /> instead.
    /// </remarks>
    [Obsolete("Do not use, see remarks.")]
    public static bool Find(Guid target, out string path, out string instanceId, int instance = 0)
    {
        return FindByInterfaceGuid(target, out path, out instanceId, instance);
    }

    /// <summary>
    ///     Invokes the installation of a driver via provided .INF file.
    /// </summary>
    /// <param name="fullInfPath">An absolute path to the .INF file to install.</param>
    /// <param name="rebootRequired">True if a machine reboot is required, false otherwise.</param>
    /// <returns>True on success, false otherwise.</returns>
    public static bool Install(string fullInfPath, out bool rebootRequired)
    {
        return SetupApi.DiInstallDriver(HWND.Null, fullInfPath, (uint)DIINSTALLDRIVER_FLAGS.DIIRFLAG_FORCE_INF,
            out rebootRequired);
    }

    /// <summary>
    ///     Creates a virtual device node (hardware ID) in the provided device class.
    /// </summary>
    /// <param name="className">The device class name.</param>
    /// <param name="classGuid">The GUID of the device class.</param>
    /// <param name="node">The node path terminated by two null characters.</param>
    /// <returns>True on success, false otherwise.</returns>
    public static bool Create(string className, Guid classGuid, string node)
    {
        HDEVINFO deviceInfoSet = HDEVINFO.Null;
        SetupApi.SP_DEVINFO_DATA deviceInfoData = new();

        try
        {
            deviceInfoSet = SetupApi.SetupDiCreateDeviceInfoList(ref classGuid, HWND.Null);

            if (deviceInfoSet == (IntPtr)(-1))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            deviceInfoData.cbSize = Marshal.SizeOf(deviceInfoData);

            if (!SetupApi.SetupDiCreateDeviceInfo(
                    deviceInfoSet,
                    className,
                    ref classGuid,
                    null,
                    HWND.Null,
                    (int)SETUP_DI_DEVICE_CREATION_FLAGS.DICD_GENERATE_ID,
                    ref deviceInfoData
                ))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            if (!SetupApi.SetupDiSetDeviceRegistryProperty(
                    deviceInfoSet,
                    ref deviceInfoData,
                    (int)SETUP_DI_REGISTRY_PROPERTY.SPDRP_HARDWAREID,
                    node,
                    node.Length * 2
                ))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            if (!SetupApi.SetupDiCallClassInstaller(
                    (int)DI_FUNCTION.DIF_REGISTERDEVICE,
                    deviceInfoSet,
                    ref deviceInfoData
                ))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
        }
        finally
        {
            if (deviceInfoSet != HDEVINFO.Null)
            {
                SetupApi.SetupDiDestroyDeviceInfoList(deviceInfoSet);
            }
        }

        return true;
    }

    /// <summary>
    ///     Removed a device node identified by class GUID, path and instance ID.
    /// </summary>
    /// <param name="classGuid">The device class GUID.</param>
    /// <param name="instanceId">The instance ID.</param>
    /// <returns>True on success, false otherwise.</returns>
    public static bool Remove(Guid classGuid, string instanceId)
    {
        return Remove(classGuid, instanceId, out _);
    }

    /// <summary>
    ///     Removed a device node identified by interface GUID and instance ID.
    /// </summary>
    /// <param name="classGuid">The device class GUID.</param>
    /// <param name="instanceId">The instance ID.</param>
    /// <param name="rebootRequired">True if a reboot is required to complete the uninstall action, false otherwise.</param>
    /// <returns>True on success, false otherwise.</returns>
    public static unsafe bool Remove(Guid classGuid, string instanceId, out bool rebootRequired)
    {
        HDEVINFO deviceInfoSet = HDEVINFO.Null;
        IntPtr installParams = Marshal.AllocHGlobal(584); // Max struct size on x64 platform

        try
        {
            SetupApi.SP_DEVINFO_DATA deviceInfoData = new();
            deviceInfoData.cbSize = Marshal.SizeOf(deviceInfoData);

            deviceInfoSet = SetupApi.SetupDiGetClassDevs(
                ref classGuid,
                IntPtr.Zero,
                HWND.Null,
                (int)SETUP_DI_GET_CLASS_DEVS_FLAGS.DIGCF_PRESENT |
                (int)SETUP_DI_GET_CLASS_DEVS_FLAGS.DIGCF_DEVICEINTERFACE
            );

            if (SetupApi.SetupDiOpenDeviceInfo(
                    deviceInfoSet,
                    instanceId,
                    IntPtr.Zero,
                    0,
                    ref deviceInfoData
                ))
            {
                SetupApi.SP_REMOVEDEVICE_PARAMS props = new()
                {
                    ClassInstallHeader = new SetupApi.SP_CLASSINSTALL_HEADER()
                };

                props.ClassInstallHeader.cbSize = Marshal.SizeOf(props.ClassInstallHeader);
                props.ClassInstallHeader.InstallFunction = (int)DI_FUNCTION.DIF_REMOVE;

                props.Scope = (int)SETUP_DI_REMOVE_DEVICE_SCOPE.DI_REMOVEDEVICE_GLOBAL;
                props.HwProfile = 0x00;

                // Prepare class (un-)installer
                if (SetupApi.SetupDiSetClassInstallParams(
                        deviceInfoSet,
                        &deviceInfoData,
                        &props,
                        Marshal.SizeOf(props)
                    ))
                {
                    // Invoke class installer with uninstall action
                    if (!SetupApi.SetupDiCallClassInstaller((int)DI_FUNCTION.DIF_REMOVE, deviceInfoSet,
                            ref deviceInfoData))
                    {
                        throw new Win32Exception(Marshal.GetLastWin32Error());
                    }

                    // Fill cbSize field
                    Marshal.WriteInt32(
                        installParams,
                        0, // cbSize is first field, always 32 bits long
                        IntPtr.Size == 4 ? 556 /* x86 size */ : 584 /* x64 size */
                    );

                    // Fill SP_DEVINSTALL_PARAMS struct
                    if (!SetupApi.SetupDiGetDeviceInstallParams(deviceInfoSet, &deviceInfoData,
                            installParams))
                    {
                        throw new Win32Exception(Marshal.GetLastWin32Error());
                    }

                    // Grab Flags field of SP_DEVINSTALL_PARAMS (offset of 32 bits)
                    int flags = Marshal.ReadInt32(installParams, Marshal.SizeOf(typeof(uint)));

                    // Test for restart/reboot flags being present
                    rebootRequired = (flags & (uint)SETUP_DI_DEVICE_INSTALL_FLAGS.DI_NEEDRESTART) != 0 ||
                                     (flags & (uint)SETUP_DI_DEVICE_INSTALL_FLAGS.DI_NEEDREBOOT) != 0;

                    return true;
                }
                else
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }
            }
            else
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
        }
        finally
        {
            if (deviceInfoSet != HDEVINFO.Null)
            {
                SetupApi.SetupDiDestroyDeviceInfoList(deviceInfoSet);
            }

            Marshal.FreeHGlobal(installParams);
        }
    }

    /// <summary>
    ///     Instructs the system to re-enumerate hardware devices.
    /// </summary>
    /// <returns>True on success, false otherwise.</returns>
    public static bool Refresh()
    {
        if (PInvoke.CM_Locate_DevNode_Ex(out uint devRoot, null, 0, 0) != CONFIGRET.CR_SUCCESS)
        {
            return false;
        }

        return PInvoke.CM_Reenumerate_DevNode_Ex(devRoot, 0, 0) == CONFIGRET.CR_SUCCESS;
    }

    /// <summary>
    ///     Instructs the system to re-enumerate hardware devices including disconnected ones.
    /// </summary>
    /// <returns>True on success, false otherwise.</returns>
    public static bool RefreshPhantom()
    {
        if (PInvoke.CM_Locate_DevNode_Ex(out uint devRoot, null,
                (uint)CM_LOCATE_DEVNODE_FLAGS.CM_LOCATE_DEVNODE_PHANTOM, 0) !=
            CONFIGRET.CR_SUCCESS)
        {
            return false;
        }

        return PInvoke.CM_Reenumerate_DevNode_Ex(devRoot, (uint)CM_REENUMERATE_FLAGS.CM_REENUMERATE_SYNCHRONOUS, 0) ==
               CONFIGRET.CR_SUCCESS;
    }

    /// <summary>
    ///     Given an INF file and a hardware ID, this function installs updated drivers for devices that match the hardware ID.
    /// </summary>
    /// <param name="hardwareId">A string that supplies the hardware identifier to match existing devices on the computer.</param>
    /// <param name="fullInfPath">A string that supplies the full path file name of an INF file.</param>
    /// <param name="rebootRequired">A variable that indicates whether a restart is required and who should prompt for it.</param>
    /// <returns>
    ///     The function returns TRUE if a device was upgraded to the specified driver.
    ///     Otherwise, it returns FALSE and the logged error can be retrieved with a call to GetLastError.
    /// </returns>
    public static unsafe bool Update(string hardwareId, string fullInfPath,
        out bool rebootRequired)
    {
        BOOL reboot = false;

        BOOL ret = PInvoke.UpdateDriverForPlugAndPlayDevices(
            HWND.Null,
            hardwareId,
            fullInfPath,
            UPDATEDRIVERFORPLUGANDPLAYDEVICES_FLAGS.INSTALLFLAG_FORCE |
            UPDATEDRIVERFORPLUGANDPLAYDEVICES_FLAGS.INSTALLFLAG_NONINTERACTIVE,
            &reboot
        );

        rebootRequired = reboot > 0;

        return ret;
    }

    /// <summary>
    ///     Uninstalls a driver identified via a given INF and optionally removes it from the driver store as well.
    /// </summary>
    /// <param name="oemInfName">The OEM INF name (name and extension only).</param>
    /// <param name="fullInfPath">The fully qualified absolute path to the INF to remove from driver store.</param>
    /// <param name="forceDelete">Remove driver store copy, if true.</param>
    public static void DeleteDriver(string oemInfName, string fullInfPath = default, bool forceDelete = false)
    {
        if (string.IsNullOrEmpty(oemInfName))
        {
            throw new ArgumentNullException(nameof(oemInfName));
        }

        if (forceDelete
            && Kernel32.MethodExists("newdev.dll", "DiUninstallDriverW")
            && !SetupApi.DiUninstallDriver(
                HWND.Null,
                fullInfPath,
                (uint)DIUNINSTALLDRIVER_FLAGS.DIURFLAG_NO_REMOVE_INF,
                out _))
        {
            throw new Win32Exception(Marshal.GetLastWin32Error());
        }

        if (!SetupApi.SetupUninstallOEMInf(
                oemInfName,
                forceDelete
                    ? PInvoke.SUOI_FORCEDELETE
                    : 0,
                IntPtr.Zero))
        {
            throw new Win32Exception(Marshal.GetLastWin32Error());
        }
    }
}