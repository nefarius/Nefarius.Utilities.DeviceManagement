#nullable enable
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

using Windows.Win32;
using Windows.Win32.Devices.DeviceAndDriverInstallation;
using Windows.Win32.Devices.Properties;
using Windows.Win32.Foundation;

using Nefarius.Utilities.DeviceManagement.Exceptions;

namespace Nefarius.Utilities.DeviceManagement.PnP;

/// <summary>
///     Describes an instance of a PNP device.
/// </summary>
[SuppressMessage("ReSharper", "UnusedMember.Global")]
public partial class PnPDevice : IPnPDevice, IEquatable<PnPDevice>
{
    /// <summary>
    ///     A CfgMgr32-compatible handle of this device instance.
    /// </summary>
    private readonly uint _instanceHandle;

    /// <summary>
    ///     Creates a new <see cref="PnPDevice" /> based on the supplied instance ID to search in the device tree.
    /// </summary>
    /// <param name="instanceId">The instance ID to look for.</param>
    /// <param name="flags">The <see cref="DeviceLocationFlags" /> influencing search behavior.</param>
    /// <exception cref="ConfigManagerException"></exception>
    protected unsafe PnPDevice(string instanceId, DeviceLocationFlags flags)
    {
        InstanceId = instanceId;
        uint iFlags = PInvoke.CM_LOCATE_DEVNODE_NORMAL;

        switch (flags)
        {
            case DeviceLocationFlags.Normal:
                iFlags = PInvoke.CM_LOCATE_DEVNODE_NORMAL;
                break;
            case DeviceLocationFlags.Phantom:
                iFlags = PInvoke.CM_LOCATE_DEVNODE_PHANTOM;
                break;
            case DeviceLocationFlags.CancelRemove:
                iFlags = PInvoke.CM_LOCATE_DEVNODE_CANCELREMOVE;
                break;
        }

        fixed (char* pInstId = instanceId)
        {
            CONFIGRET ret = PInvoke.CM_Locate_DevNode(
                out _instanceHandle,
                pInstId,
                iFlags
            );

            if (ret == CONFIGRET.CR_NO_SUCH_DEVINST)
            {
                throw new ArgumentException("The supplied instance wasn't found.", nameof(instanceId));
            }

            if (ret != CONFIGRET.CR_SUCCESS)
            {
                throw new ConfigManagerException("Failed to locate device node.", ret);
            }

            ret = PInvoke.CM_Get_Device_ID_Size(out uint charsRequired, _instanceHandle, 0);

            if (ret != CONFIGRET.CR_SUCCESS)
            {
                throw new ConfigManagerException("Fetching device ID size failed.", ret);
            }

            uint nBytes = (charsRequired + 1) * 2;
            char* ptrInstanceBuf = stackalloc char[(int)nBytes];

            ret = PInvoke.CM_Get_Device_IDW(
                _instanceHandle,
                ptrInstanceBuf,
                nBytes,
                0
            );

            if (ret != CONFIGRET.CR_SUCCESS)
            {
                throw new ConfigManagerException("Fetching device ID failed.", ret);
            }

            DeviceId = new string(ptrInstanceBuf);
        }
    }

    /// <summary>
    ///     The instance ID of the device. Uniquely identifies devices of equal make and model on the same machine.
    /// </summary>
    public string InstanceId { get; }

    /// <summary>
    ///     The device ID. Typically built from the hardware ID of the same make and model of hardware.
    /// </summary>
    public string DeviceId { get; }

    /// <summary>
    ///     Attempts to restart this device. Device restart may fail if it has open handles that currently can not be
    ///     force-closed.
    /// </summary>
    /// <remarks>
    ///     This method removes and re-enumerates (adds) the device note, which might cause unintended side-effects. If
    ///     this is the behaviour you seek, consider using <see cref="RemoveAndSetup" /> instead. This method remains here for
    ///     backwards compatibility.
    /// </remarks>
    [Obsolete("This method can cause unintended side-effects, see remarks for details.")]
    public unsafe void Restart()
    {
        CONFIGRET ret = PInvoke.CM_Query_And_Remove_SubTree(
            _instanceHandle,
            null, null, 0,
            PInvoke.CM_REMOVE_NO_RESTART
        );

        if (ret != CONFIGRET.CR_SUCCESS)
        {
            throw new ConfigManagerException("Node removal failed.", ret);
        }

        ret = PInvoke.CM_Setup_DevNode(
            _instanceHandle,
            PInvoke.CM_SETUP_DEVNODE_READY
        );

        if (ret is CONFIGRET.CR_NO_SUCH_DEVINST or CONFIGRET.CR_SUCCESS)
        {
            return;
        }

        throw new ConfigManagerException("Node addition failed.", ret);
    }

    /// <summary>
    ///     Attempts to remove this device node.
    /// </summary>
    /// <exception cref="ConfigManagerException"></exception>
    /// <remarks>
    ///     This call DOES NOT invoke device and driver uninstall routines, as soon as the device is re-enumerated, it
    ///     will reappear and become online.
    /// </remarks>
    public unsafe void Remove()
    {
        CONFIGRET ret = PInvoke.CM_Query_And_Remove_SubTree(
            _instanceHandle,
            null, null, 0,
            PInvoke.CM_REMOVE_NO_RESTART
        );

        if (ret != CONFIGRET.CR_SUCCESS)
        {
            throw new ConfigManagerException("Node removal failed.", ret);
        }
    }

    /// <summary>
    ///     Walks up the <see cref="PnPDevice" />s parents chain to determine if the top most device is root enumerated.
    /// </summary>
    /// <remarks>
    ///     This is achieved by walking up the node tree until the top most parent and check if the last parent below the
    ///     tree root is a software device. Hardware devices originate from a PCI(e) bus while virtual devices originate from a
    ///     root enumerated device.
    /// </remarks>
    /// <param name="excludeIfMatches">Returns false if the given predicate is true.</param>
    /// <returns>True if this devices originates from an emulator, false otherwise.</returns>
    public bool IsVirtual(Func<IPnPDevice, bool>? excludeIfMatches = default)
    {
        IPnPDevice device = this;

        while (device is not null)
        {
            if (excludeIfMatches != null && excludeIfMatches(device))
            {
                return false;
            }

            string parentId = device.GetProperty<string>(DevicePropertyKey.Device_Parent);

            if (parentId.Equals(@"HTREE\ROOT\0", StringComparison.OrdinalIgnoreCase))
            {
                break;
            }

            device = GetDeviceByInstanceId(parentId, DeviceLocationFlags.Phantom);
        }

        //
        // TODO: test how others behave (reWASD, NVIDIA, ...)
        // 
        return device is not null &&
               (device.InstanceId.StartsWith(@"ROOT\SYSTEM", StringComparison.OrdinalIgnoreCase)
                || device.InstanceId.StartsWith(@"ROOT\USB", StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    ///     Installs the NULL-driver on this device instance.
    /// </summary>
    /// <remarks>
    ///     This will tear down the current device stack (no matter how many open handles exist), remove the existing function
    ///     driver and reboot the device in "raw" or "driverless" mode. Some USB devices may require a port-cycle afterwards
    ///     for the change to take effect without requiring a reboot.
    /// </remarks>
    public void InstallNullDriver()
    {
        InstallNullDriver(out _);
    }

    /// <summary>
    ///     Installs the NULL-driver on this device instance.
    /// </summary>
    /// <remarks>
    ///     This will tear down the current device stack (no matter how many open handles exist), remove the existing function
    ///     driver and reboot the device in "raw" or "driverless" mode. Some USB devices may require a port-cycle afterwards
    ///     for the change to take effect without requiring a reboot.
    /// </remarks>
    public unsafe void InstallNullDriver(out bool rebootRequired)
    {
        SetupApi.SP_DEVINFO_DATA spDevinfoData = new();
        spDevinfoData.cbSize = Marshal.SizeOf(spDevinfoData);

        HDEVINFO hDevInfo = SetupApi.SetupDiGetClassDevs(
            null,
            null,
            HWND.Null,
            PInvoke.DIGCF_ALLCLASSES | PInvoke.DIGCF_PRESENT
        );

        if (hDevInfo.IsNull)
        {
            throw new Win32Exception("Failed to get devices for all classes");
        }

        try
        {
            for (UInt32 devIndex = 0; SetupApi.SetupDiEnumDeviceInfo(hDevInfo, devIndex, &spDevinfoData); devIndex++)
            {
                DEVPROPKEY instanceProp = DevicePropertyKey.Device_InstanceId.ToCsWin32Type();

                bool success = SetupApi.SetupDiGetDeviceProperty(
                    hDevInfo,
                    &spDevinfoData,
                    &instanceProp,
                    out _,
                    null,
                    0,
                    out uint requiredSize,
                    0
                );

                WIN32_ERROR error = (WIN32_ERROR)Marshal.GetLastWin32Error();

                if (success || error != WIN32_ERROR.ERROR_INSUFFICIENT_BUFFER)
                {
                    throw new Win32Exception("Unexpected result while querying Instance ID property size");
                }

                StringBuilder sb = new((int)requiredSize);

                success = SetupApi.SetupDiGetDeviceProperty(
                    hDevInfo,
                    &spDevinfoData,
                    &instanceProp,
                    out _,
                    sb,
                    requiredSize,
                    out _,
                    0
                );

                if (!success)
                {
                    throw new Win32Exception("Failed to query Instance ID property");
                }

                string instanceId = sb.ToString();

                if (!InstanceId.Equals(instanceId, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                success = SetupApi.DiInstallDevice(
                    HWND.Null,
                    hDevInfo,
                    &spDevinfoData,
                    IntPtr.Zero,
                    PInvoke.DIIDFLAG_INSTALLNULLDRIVER,
                    out rebootRequired
                );

                if (!success)
                {
                    throw new Win32Exception("NULL-driver installation failed");
                }

                return;
            }
        }
        finally
        {
            if (!hDevInfo.IsNull)
            {
                SetupApi.SetupDiDestroyDeviceInfoList(hDevInfo);
            }
        }

        throw new ArgumentOutOfRangeException(nameof(InstanceId), $"Failed to find device instance {InstanceId}");
    }

    /// <summary>
    ///     Installs a custom driver identified by the provided INF name on this device instance.
    /// </summary>
    /// <remarks>
    ///     This method force-installs a given INF file on this device instance, even if no matching hardware or compatible IDs
    ///     are found. This method can only succeed if <see cref="InstallNullDriver()" /> is called prior.
    /// </remarks>
    /// <param name="infName">
    ///     The INF file name as found in C:\Windows\INF directory. It must be the name only, not a relative
    ///     or absolute path.
    /// </param>
    public void InstallCustomDriver(string infName)
    {
        InstallCustomDriver(infName, out _);
    }

    /// <summary>
    ///     Installs a custom driver identified by the provided INF name on this device instance.
    /// </summary>
    /// <remarks>
    ///     This method force-installs a given INF file on this device instance, even if no matching hardware or compatible IDs
    ///     are found. This method can only succeed if <see cref="InstallNullDriver()" /> is called prior.
    /// </remarks>
    /// <param name="infName">
    ///     The INF file name as found in C:\Windows\INF directory. It must be the name only, not a relative
    ///     or absolute path.
    /// </param>
    /// <param name="rebootRequired">
    ///     Gets whether a reboot is required for the changes to take effect or not.
    /// </param>
    public unsafe void InstallCustomDriver(string infName, out bool rebootRequired)
    {
        DirectoryInfo systemRoot = Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.System))!;

        string fullInfPath = Path.Combine(systemRoot!.FullName, "INF", infName);

        if (!File.Exists(fullInfPath))
        {
            throw new ArgumentException($"The provided INF file {infName} wasn't found in the system directory",
                nameof(infName));
        }

        SetupApi.SP_DEVINSTALL_PARAMS deviceInstallParams = new();
        deviceInstallParams.cbSize = Marshal.SizeOf(deviceInstallParams);

        SetupApi.SP_DRVINFO_DATA driverInfoData = new();
        driverInfoData.cbSize = Marshal.SizeOf(driverInfoData);

        SetupApi.SP_DEVINFO_DATA devInfoData = new();
        devInfoData.cbSize = Marshal.SizeOf(devInfoData);

        bool freeDriverInfoList = false;

        HDEVINFO hDevInfo = SetupApi.SetupDiCreateDeviceInfoList(null, HWND.Null);

        if (hDevInfo.IsNull)
        {
            throw new Win32Exception("Failed to get get empty device info list");
        }

        try
        {
            bool success = SetupApi.SetupDiOpenDeviceInfo(
                hDevInfo,
                InstanceId,
                HWND.Null,
                0,
                &devInfoData
            );

            if (!success)
            {
                throw new Win32Exception("Failed to open device info");
            }

            // extra plausibility check to inform the caller with a more helpful message
            if (devInfoData.ClassGuid != Guid.Empty)
            {
                throw new InvalidOperationException(
                    $"Expected class GUID {Guid.Empty} but got {devInfoData.ClassGuid}, " +
                    $"subsequent calls will not succeed, did you forget to call {nameof(InstallNullDriver)}?");
            }

            success = SetupApi.SetupDiSetSelectedDevice(hDevInfo, &devInfoData);

            if (!success)
            {
                throw new Win32Exception("Failed to set the selected device");
            }

            success = SetupApi.SetupDiGetDeviceInstallParams(
                hDevInfo,
                &devInfoData,
                ref deviceInstallParams
            );

            if (!success)
            {
                throw new Win32Exception("Failed to get the device install parameters");
            }

            deviceInstallParams.Flags |= PInvoke.DI_ENUMSINGLEINF;
            deviceInstallParams.FlagsEx |= PInvoke.DI_FLAGSEX_ALLOWEXCLUDEDDRVS;
            deviceInstallParams.DriverPath = infName;

            success = SetupApi.SetupDiSetDeviceInstallParams(
                hDevInfo,
                &devInfoData,
                ref deviceInstallParams
            );

            if (!success)
            {
                throw new Win32Exception("Failed to set the device install parameters");
            }

            success = SetupApi.SetupDiBuildDriverInfoList(
                hDevInfo,
                &devInfoData,
                SETUP_DI_BUILD_DRIVER_DRIVER_TYPE.SPDIT_CLASSDRIVER
            );

            if (!success)
            {
                throw new Win32Exception("Failed to build driver info list");
            }

            freeDriverInfoList = true;

            success = SetupApi.SetupDiEnumDriverInfo(
                hDevInfo,
                &devInfoData,
                SETUP_DI_BUILD_DRIVER_DRIVER_TYPE.SPDIT_CLASSDRIVER,
                0,
                ref driverInfoData
            );

            if (!success)
            {
                throw new Win32Exception("Failed to enumerate driver info");
            }

            success = SetupApi.SetupDiSetSelectedDriver(
                hDevInfo,
                &devInfoData,
                ref driverInfoData
            );

            if (!success)
            {
                throw new Win32Exception("Failed to set selected driver");
            }

            success = SetupApi.DiInstallDevice(
                HWND.Null,
                hDevInfo,
                &devInfoData,
                ref driverInfoData,
                0,
                out rebootRequired
            );

            if (!success)
            {
                throw new Win32Exception("Failed to install selected driver");
            }
        }
        finally
        {
            if (freeDriverInfoList)
            {
                SetupApi.SetupDiDestroyDriverInfoList(
                    hDevInfo,
                    &devInfoData,
                    SETUP_DI_BUILD_DRIVER_DRIVER_TYPE.SPDIT_CLASSDRIVER
                );
            }

            if (!hDevInfo.IsNull)
            {
                SetupApi.SetupDiDestroyDeviceInfoList(hDevInfo);
            }
        }
    }

    /// <summary>
    ///     Uninstalls this device instance. Unlike <see cref="Remove" /> this call will unload and revert the device function
    ///     driver to the best available compatible candidate on next device boot.
    /// </summary>
    /// <remarks>
    ///     If this is used in combination with <see cref="InstallNullDriver()" /> or
    ///     <see cref="InstallCustomDriver(string)" />, you can call <see cref="Devcon.Refresh" /> afterwards to trigger device
    ///     installation.
    /// </remarks>
    /// <exception cref="Win32Exception"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public void Uninstall()
    {
        Uninstall(out _);
    }

    /// <summary>
    ///     Uninstalls this device instance. Unlike <see cref="Remove" /> this call will unload and revert the device function
    ///     driver to the best available compatible candidate on next device boot.
    /// </summary>
    /// <remarks>
    ///     If this is used in combination with <see cref="InstallNullDriver()" /> or
    ///     <see cref="InstallCustomDriver(string)" />, you can call <see cref="Devcon.Refresh" /> afterwards to trigger device
    ///     installation.
    /// </remarks>
    /// <param name="rebootRequired">
    ///     Gets whether a reboot is required for the changes to take effect or not.
    /// </param>
    /// <exception cref="Win32Exception"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public unsafe void Uninstall(out bool rebootRequired)
    {
        SetupApi.SP_DEVINFO_DATA spDevinfoData = new();
        spDevinfoData.cbSize = Marshal.SizeOf(spDevinfoData);

        SetupApi.SP_DRVINFO_DATA driverInfoData = new();
        driverInfoData.cbSize = Marshal.SizeOf(driverInfoData);

        HDEVINFO hDevInfo = SetupApi.SetupDiGetClassDevs(
            null,
            null,
            HWND.Null,
            PInvoke.DIGCF_ALLCLASSES | PInvoke.DIGCF_PRESENT
        );

        if (hDevInfo.IsNull)
        {
            throw new Win32Exception("Failed to get devices for all classes");
        }

        for (UInt32 devIndex = 0; SetupApi.SetupDiEnumDeviceInfo(hDevInfo, devIndex, &spDevinfoData); devIndex++)
        {
            DEVPROPKEY instanceProp = DevicePropertyKey.Device_InstanceId.ToCsWin32Type();

            bool success = SetupApi.SetupDiGetDeviceProperty(
                hDevInfo,
                &spDevinfoData,
                &instanceProp,
                out _,
                null,
                0,
                out uint requiredSize,
                0
            );

            WIN32_ERROR error = (WIN32_ERROR)Marshal.GetLastWin32Error();

            if (success || error != WIN32_ERROR.ERROR_INSUFFICIENT_BUFFER)
            {
                throw new Win32Exception("Unexpected result while querying Instance ID property size");
            }

            StringBuilder sb = new((int)requiredSize);

            success = SetupApi.SetupDiGetDeviceProperty(
                hDevInfo,
                &spDevinfoData,
                &instanceProp,
                out _,
                sb,
                requiredSize,
                out _,
                0
            );

            if (!success)
            {
                throw new Win32Exception("Failed to query Instance ID property");
            }

            string instanceId = sb.ToString();

            if (!InstanceId.Equals(instanceId, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            // found our device

            /*
            success = SetupApi.SetupDiBuildDriverInfoList(
                hDevInfo,
                &spDevinfoData,
                SETUP_DI_BUILD_DRIVER_DRIVER_TYPE.SPDIT_COMPATDRIVER
            );

            if (!success)
            {
                throw new Win32Exception("Failed to build driver info list");
            }

            success = SetupApi.SetupDiEnumDriverInfo(
                hDevInfo,
                &spDevinfoData,
                SETUP_DI_BUILD_DRIVER_DRIVER_TYPE.SPDIT_COMPATDRIVER,
                0,
                ref driverInfoData
            );

            if (!success)
            {
                throw new Win32Exception("Failed to enumerate driver info");
            }

            SetupApi.SP_DRVINFO_DETAIL_DATA drvInfoDetailData = new();
            drvInfoDetailData.cbSize = Marshal.SizeOf(drvInfoDetailData);

            // grab required buffer size
            _ = SetupApi.SetupDiGetDriverInfoDetail(
                hDevInfo,
                &spDevinfoData,
                ref driverInfoData,
                ref drvInfoDetailData,
                (uint)drvInfoDetailData.cbSize,
                out uint requiredBufferSize
            );

            if (requiredBufferSize == 0)
            {
                throw new Win32Exception("Failed to get required driver info details buffer size");
            }

            IntPtr drvInfoDetailDataBuffer = Marshal.AllocHGlobal((int)requiredBufferSize);
            Marshal.WriteInt32(drvInfoDetailDataBuffer, drvInfoDetailData.cbSize);

            success = SetupApi.SetupDiGetDriverInfoDetail(
                hDevInfo,
                &spDevinfoData,
                ref driverInfoData,
                drvInfoDetailDataBuffer,
                requiredBufferSize,
                out _
            );

            if (!success)
            {
                throw new Win32Exception("Failed to get driver info details");
            }

            // this is save to do if we do not read the Hardware IDs dynamic field
            drvInfoDetailData = Marshal.PtrToStructure<SetupApi.SP_DRVINFO_DETAIL_DATA>(drvInfoDetailDataBuffer);
            */

            // uninstall device
            success = SetupApi.DiUninstallDevice(
                HWND.Null,
                hDevInfo,
                &spDevinfoData,
                0,
                out bool devNeedsReboot
            );

            if (!success)
            {
                throw new Win32Exception("Device uninstallation failed");
            }

            /*
            success = SetupApi.DiUninstallDriver(
                HWND.Null,
                drvInfoDetailData.InfFileName,
                0,
                out bool drvNeedsReboot
            );

            if (!success)
            {
                throw new Win32Exception("Driver uninstallation failed");
            }

            rebootRequired = devNeedsReboot || drvNeedsReboot;
            */

            rebootRequired = devNeedsReboot;

            return;
        }

        throw new ArgumentOutOfRangeException(nameof(InstanceId), $"Failed to find device instance {InstanceId}");
    }

    /// <summary>
    ///     Attempts to restart this device by removing it from the device tree and causing re-enumeration afterwards.
    /// </summary>
    /// <remarks>Device restart may fail if it has open handles that currently can not be force-closed.</remarks>
    /// <exception cref="ConfigManagerException"></exception>
    public unsafe void RemoveAndSetup()
    {
        CONFIGRET ret = PInvoke.CM_Query_And_Remove_SubTree(
            _instanceHandle,
            null, null, 0,
            PInvoke.CM_REMOVE_NO_RESTART
        );

        if (ret != CONFIGRET.CR_SUCCESS)
        {
            throw new ConfigManagerException("Node removal failed.", ret);
        }

        ret = PInvoke.CM_Setup_DevNode(
            _instanceHandle,
            PInvoke.CM_SETUP_DEVNODE_READY
        );

        if (ret is CONFIGRET.CR_NO_SUCH_DEVINST or CONFIGRET.CR_SUCCESS)
        {
            return;
        }

        throw new ConfigManagerException("Node addition failed.", ret);
    }

    /// <summary>
    ///     Disables this device instance node.
    /// </summary>
    /// <exception cref="ConfigManagerException"></exception>
    public void Disable()
    {
        CONFIGRET ret = PInvoke.CM_Disable_DevNode(_instanceHandle, PInvoke.CM_DISABLE_UI_NOT_OK);

        if (ret != CONFIGRET.CR_SUCCESS)
        {
            throw new ConfigManagerException("Disabling node failed.", ret);
        }
    }

    /// <summary>
    ///     Enables this device instance node.
    /// </summary>
    /// <exception cref="ConfigManagerException"></exception>
    public void Enable()
    {
        CONFIGRET ret = PInvoke.CM_Enable_DevNode(_instanceHandle, 0);

        if (ret != CONFIGRET.CR_SUCCESS)
        {
            throw new ConfigManagerException("Enabling node failed.", ret);
        }
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return InstanceId;
    }

    #region IEquatable

    /// <inheritdoc />
    public bool Equals(PnPDevice other)
    {
        if (ReferenceEquals(null, other))
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return string.Equals(DeviceId, other.DeviceId, StringComparison.InvariantCultureIgnoreCase);
    }

    /// <inheritdoc />
    public override bool Equals(object obj)
    {
        return ReferenceEquals(this, obj) || (obj is PnPDevice other && Equals(other));
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return StringComparer.InvariantCultureIgnoreCase.GetHashCode(DeviceId);
    }

    /// <summary>
    ///     Compares two instances of <see cref="PnPDevice" /> for equality.
    /// </summary>
    public static bool operator ==(PnPDevice left, PnPDevice right)
    {
        return Equals(left, right);
    }

    /// <summary>
    ///     Compares two instances of <see cref="PnPDevice" /> for equality.
    /// </summary>
    public static bool operator !=(PnPDevice left, PnPDevice right)
    {
        return !Equals(left, right);
    }

    #endregion
}