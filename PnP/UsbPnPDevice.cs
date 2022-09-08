using System;
using System.Runtime.InteropServices;
using Windows.Win32;
using Windows.Win32.Devices.DeviceAndDriverInstallation;
using Windows.Win32.Foundation;
using Windows.Win32.Storage.FileSystem;
using Nefarius.Utilities.DeviceManagement.Exceptions;
using Nefarius.Utilities.DeviceManagement.Extensions;

namespace Nefarius.Utilities.DeviceManagement.PnP
{
    /// <summary>
    ///     Describes an instance of a USB PNP device.
    /// </summary>
    public class UsbPnPDevice : PnPDevice
    {
        internal UsbPnPDevice(string instanceId, DeviceLocationFlags flags) : base(instanceId, flags)
        {
            var enumerator = GetProperty<string>(DevicePropertyKey.Device_EnumeratorName);

            if (!Equals(enumerator, "USB"))
                throw new ArgumentException("This device is not a USB device.");

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
            var hubDevice = this;
            var compositeDevice = this;

            // find root hub
            while (hubDevice is not null)
            {
                var parentId = hubDevice.GetProperty<string>(DevicePropertyKey.Device_Parent);
                var service = hubDevice.GetProperty<string>(DevicePropertyKey.Device_Service);

                if (service is not null)
                    // we have reached the hub object, bail
                    if (service.StartsWith("USBHUB", StringComparison.OrdinalIgnoreCase))
                        break;

                // grab topmost child to get real port number
                compositeDevice = hubDevice;

                // get next parent
                hubDevice = GetDeviceByInstanceId(parentId, DeviceLocationFlags.Phantom).ToUsbPnPDevice();
            }

            if (hubDevice is null)
                throw new ArgumentException("Unable to find root hub for the current device.");

            var parameters = new USB_CYCLE_PORT_PARAMS
            {
                ConnectionIndex = compositeDevice.Port
            };

            fixed (char* hubInstanceId = hubDevice.InstanceId)
            {
                var ret = PInvoke.CM_Get_Device_Interface_List_Size(
                    out var listLength,
                    PInvoke.GUID_DEVINTERFACE_USB_HUB,
                    hubInstanceId,
                    PInvoke.CM_GET_DEVICE_INTERFACE_LIST_PRESENT
                );

                if (ret != CONFIGRET.CR_SUCCESS)
                    throw new ConfigManagerException("Failed to get device interface list size.", ret);

                var bytesRequired = (int)listLength * 2;
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
                        throw new ConfigManagerException("Failed to get device interface list.", ret);

                    var hubPath = listBuffer.ToString();

                    if (hubPath is null)
                        throw new ArgumentException("Failed to get device interface path.");

                    using var hubHandle = PInvoke.CreateFile(
                        hubPath,
                        FILE_ACCESS_FLAGS.FILE_GENERIC_READ | FILE_ACCESS_FLAGS.FILE_GENERIC_WRITE,
                        FILE_SHARE_MODE.FILE_SHARE_READ | FILE_SHARE_MODE.FILE_SHARE_WRITE,
                        null,
                        FILE_CREATION_DISPOSITION.OPEN_EXISTING,
                        FILE_FLAGS_AND_ATTRIBUTES.FILE_ATTRIBUTE_NORMAL
                        | FILE_FLAGS_AND_ATTRIBUTES.FILE_FLAG_NO_BUFFERING
                        | FILE_FLAGS_AND_ATTRIBUTES.FILE_FLAG_WRITE_THROUGH,
                        null
                    );

                    var size = Marshal.SizeOf<USB_CYCLE_PORT_PARAMS>();

                    // request hub to power-cycle port, effectively force-restarting the device
                    var success = PInvoke.DeviceIoControl(
                        hubHandle,
                        PInvoke.IOCTL_USB_HUB_CYCLE_PORT,
                        &parameters,
                        (uint)size,
                        &parameters,
                        (uint)size,
                        null,
                        null
                    );

                    var err = (WIN32_ERROR)Marshal.GetLastWin32Error();

                    switch (success.Value > 0)
                    {
                        case false when err == WIN32_ERROR.ERROR_GEN_FAILURE:
                            throw new ArgumentException(
                                "Request failed, this operation requires administrative privileges.");
                        case false when err == WIN32_ERROR.ERROR_NO_SUCH_DEVICE:
                            throw new ArgumentException(
                                $"Request failed, device on port {compositeDevice.Port} not found.");
                    }

                    if (parameters.Status != 0)
                        throw new ArgumentException("Port cycle request failed.");
                }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct USB_CYCLE_PORT_PARAMS
        {
            internal UInt32 ConnectionIndex;

            internal UInt32 Status;
        }
    }
}