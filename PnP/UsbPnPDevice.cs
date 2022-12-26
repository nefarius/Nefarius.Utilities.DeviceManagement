using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

using Windows.Win32;
using Windows.Win32.Devices.DeviceAndDriverInstallation;
using Windows.Win32.Foundation;
using Windows.Win32.Storage.FileSystem;

using Microsoft.Win32.SafeHandles;

using Nefarius.Utilities.DeviceManagement.Exceptions;
using Nefarius.Utilities.DeviceManagement.Extensions;

namespace Nefarius.Utilities.DeviceManagement.PnP;

/// <summary>
///     Describes an instance of a USB PNP device.
/// </summary>
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
[SuppressMessage("ReSharper", "UnusedMember.Global")]
public class UsbPnPDevice : PnPDevice
{
    internal UsbPnPDevice(string instanceId, DeviceLocationFlags flags) : base(instanceId, flags)
    {
        string enumerator = GetProperty<string>(DevicePropertyKey.Device_EnumeratorName);

        if (!Equals(enumerator, "USB"))
        {
            throw new UsbPnPDeviceConversionException("This device is not a USB device.");
        }

        Port = GetProperty<UInt32>(DevicePropertyKey.Device_Address);
    }

    /// <summary>
    ///     The port number/index of this device on its root hub.
    /// </summary>
    public uint Port { get; }

    /// <summary>
    ///     Power-cycles the hub port this device is attached to, causing it to restart.
    /// </summary>
    /// <remarks>Requires administrative privileges.</remarks>
    public unsafe void CyclePort()
    {
        UsbPnPDevice hubDevice = this;
        UsbPnPDevice compositeDevice = this;

        // find root hub
        while (hubDevice is not null)
        {
            string parentId = hubDevice.GetProperty<string>(DevicePropertyKey.Device_Parent);
            string service = hubDevice.GetProperty<string>(DevicePropertyKey.Device_Service);

            if (service is not null)
                // we have reached the hub object, bail
            {
                if (service.StartsWith("USBHUB", StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }
            }

            // grab topmost child to get real port number
            compositeDevice = hubDevice;

            // get next parent
            hubDevice = GetDeviceByInstanceId(parentId, DeviceLocationFlags.Phantom).ToUsbPnPDevice();
        }

        if (hubDevice is null)
        {
            throw new ArgumentException("Unable to find root hub for the current device.");
        }

        USB_CYCLE_PORT_PARAMS parameters = new() { ConnectionIndex = compositeDevice.Port };

        fixed (char* hubInstanceId = hubDevice.InstanceId)
        {
            CONFIGRET ret = PInvoke.CM_Get_Device_Interface_List_Size(
                out uint listLength,
                PInvoke.GUID_DEVINTERFACE_USB_HUB,
                hubInstanceId,
                PInvoke.CM_GET_DEVICE_INTERFACE_LIST_PRESENT
            );

            if (ret != CONFIGRET.CR_SUCCESS)
            {
                throw new ConfigManagerException("Failed to get device interface list size.", ret);
            }

            int bytesRequired = (int)listLength * 2;
            Span<char> listBuffer = stackalloc char[bytesRequired];

            fixed (char* pListBuffer = listBuffer)
            {
                ret = PInvoke.CM_Get_Device_Interface_List(
                    PInvoke.GUID_DEVINTERFACE_USB_HUB,
                    hubInstanceId,
                    pListBuffer,
                    listLength,
                    PInvoke.CM_GET_DEVICE_INTERFACE_LIST_PRESENT
                );

                if (ret != CONFIGRET.CR_SUCCESS)
                {
                    throw new ConfigManagerException("Failed to get device interface list.", ret);
                }

                string hubPath = listBuffer.ToString();

                if (hubPath is null)
                {
                    throw new ArgumentException("Failed to get device interface path.");
                }

                using SafeFileHandle hubHandle = PInvoke.CreateFile(
                    hubPath,
                    FILE_ACCESS_FLAGS.FILE_GENERIC_READ | FILE_ACCESS_FLAGS.FILE_GENERIC_WRITE,
                    FILE_SHARE_MODE.FILE_SHARE_READ | FILE_SHARE_MODE.FILE_SHARE_WRITE,
                    null,
                    FILE_CREATION_DISPOSITION.OPEN_EXISTING,
                    FILE_FLAGS_AND_ATTRIBUTES.FILE_ATTRIBUTE_NORMAL,
                    null
                );

                int size = Marshal.SizeOf<USB_CYCLE_PORT_PARAMS>();

                // request hub to power-cycle port, effectively force-restarting the device
                BOOL success = PInvoke.DeviceIoControl(
                    hubHandle,
                    PInvoke.IOCTL_USB_HUB_CYCLE_PORT,
                    &parameters,
                    (uint)size,
                    &parameters,
                    (uint)size,
                    null,
                    null
                );

                WIN32_ERROR err = (WIN32_ERROR)Marshal.GetLastWin32Error();

                switch (success.Value > 0)
                {
                    case false when err == WIN32_ERROR.ERROR_GEN_FAILURE:
                        throw new ArgumentException(
                            "Request failed, this operation requires administrative privileges.");
                    case false when err == WIN32_ERROR.ERROR_NO_SUCH_DEVICE:
                        throw new ArgumentException(
                            $"Request failed, device on port {compositeDevice.Port} not found.");
                }

                if (parameters.Status != 0 || hubHandle.IsInvalid)
                {
                    throw new Win32Exception("Port cycle request failed.", (int)err);
                }
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    [SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Global")]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    internal struct USB_CYCLE_PORT_PARAMS
    {
        internal UInt32 ConnectionIndex;

        internal UInt32 Status;
    }
}