using System;
using System.Diagnostics.CodeAnalysis;

using Windows.Win32;
using Windows.Win32.Devices.DeviceAndDriverInstallation;
using Windows.Win32.Foundation;

using Nefarius.Utilities.DeviceManagement.Exceptions;

namespace Nefarius.Utilities.DeviceManagement.PnP;

[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
public partial class PnPDevice
{
    /// <summary>
    ///     Return device identified by instance ID.
    /// </summary>
    /// <param name="instanceId">The instance ID of the device.</param>
    /// <param name="flags">
    ///     <see cref="DeviceLocationFlags" />
    /// </param>
    /// <returns>A <see cref="PnPDevice" />.</returns>
    /// <exception cref="PnPDeviceNotFoundException">The desired device instance was not found on the system.</exception>
    /// <exception cref="ConfigManagerException">Device information lookup failed.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The supplied <paramref name="flags" /> value was invalid.</exception>
    public static PnPDevice GetDeviceByInstanceId(string instanceId,
        DeviceLocationFlags flags = DeviceLocationFlags.Normal)
    {
        return new PnPDevice(instanceId, flags);
    }

    /// <summary>
    ///     Return device identified by instance ID/path (symbolic link).
    /// </summary>
    /// <param name="symbolicLink">The device interface path/ID/symbolic link name.</param>
    /// <param name="flags">
    ///     <see cref="DeviceLocationFlags" />
    /// </param>
    /// <returns>A <see cref="PnPDevice" /> or null if not found.</returns>
    /// <exception cref="ConfigManagerException">Interface lookup failed.</exception>
    public static PnPDevice? GetDeviceByInterfaceId(string symbolicLink,
        DeviceLocationFlags flags = DeviceLocationFlags.Normal)
    {
        string? instanceId = GetInstanceIdFromInterfaceId(symbolicLink);

        return string.IsNullOrEmpty(instanceId) ? null : GetDeviceByInstanceId(instanceId!, flags);
    }

    /// <summary>
    ///     Resolves Interface ID/Symbolic link/Device path to Instance ID.
    /// </summary>
    /// <param name="symbolicLink">The device interface path/ID/symbolic link name.</param>
    /// <returns>The Instance ID or null if not found.</returns>
    /// <exception cref="ConfigManagerException">Interface lookup failed.</exception>
    public static unsafe string? GetInstanceIdFromInterfaceId(string symbolicLink)
    {
        DEVPROPKEY property = DevicePropertyKey.Device_InstanceId.ToCsWin32Type();
        uint sizeRequired = 0;

        CONFIGRET ret = PInvoke.CM_Get_Device_Interface_Property(
            symbolicLink,
            property,
            out _,
            null,
            ref sizeRequired,
            0
        );

        // queried interface does not exist
        if (ret == CONFIGRET.CR_NO_SUCH_DEVICE_INTERFACE)
        {
            return null;
        }

        // unexpected error
        if (ret != CONFIGRET.CR_BUFFER_SMALL)
        {
            throw new ConfigManagerException("Failed to get instance interface property size.", ret);
        }

        char* buffer = stackalloc char[(int)sizeRequired];

        ret = PInvoke.CM_Get_Device_Interface_Property(
            symbolicLink,
            property,
            out _,
            (byte*)buffer,
            ref sizeRequired,
            0
        );

        if (ret != CONFIGRET.CR_SUCCESS)
        {
            throw new ConfigManagerException("Failed to get instance interface property.", ret);
        }

        return new string(buffer);
    }
}