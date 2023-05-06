using System.Diagnostics.CodeAnalysis;

using Windows.Win32;
using Windows.Win32.Devices.DeviceAndDriverInstallation;
using Windows.Win32.Devices.Properties;

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
    /// <returns>A <see cref="PnPDevice" />.</returns>
    public static PnPDevice GetDeviceByInterfaceId(string symbolicLink,
        DeviceLocationFlags flags = DeviceLocationFlags.Normal)
    {
        string instanceId = GetInstanceIdFromInterfaceId(symbolicLink);

        return GetDeviceByInstanceId(instanceId, flags);
    }

    /// <summary>
    ///     Resolves Interface ID/Symbolic link/Device path to Instance ID.
    /// </summary>
    /// <param name="symbolicLink">The device interface path/ID/symbolic link name.</param>
    /// <returns>The Instance ID.</returns>
    /// <exception cref="ConfigManagerException"></exception>
    public static unsafe string GetInstanceIdFromInterfaceId(string symbolicLink)
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