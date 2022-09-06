using System;
using System.Linq;
using System.Runtime.InteropServices;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.Storage.FileSystem;
using Nefarius.Utilities.DeviceManagement.Exceptions;
using Nefarius.Utilities.DeviceManagement.Extensions;
using Nefarius.Utilities.DeviceManagement.Util;

namespace Nefarius.Utilities.DeviceManagement.PnP
{
    /// <summary>
    ///     Describes an instance of a USB PNP device.
    /// </summary>
    public class UsbPnPDevice : PnPDevice
    {
        private const int IOCTL_USB_HUB_CYCLE_PORT = 0x220444;

        private static Guid GUID_DEVINTERFACE_USB_HUB = Guid.Parse("{f18a0e88-c30c-11d0-8815-00a0c906bed8}");

        internal UsbPnPDevice(string instanceId, DeviceLocationFlags flags) : base(instanceId, flags)
        {
            var enumerator = GetProperty<string>(DevicePropertyDevice.EnumeratorName);

            if (!Equals(enumerator, "USB"))
                throw new ArgumentException("This device is not a USB device.");

            Port = GetProperty<UInt32>(DevicePropertyDevice.Address);
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
                var parentId = hubDevice.GetProperty<string>(DevicePropertyDevice.Parent);
                var service = hubDevice.GetProperty<string>(DevicePropertyDevice.Service);

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

            var ret = SetupApiWrapper.CM_Get_Device_Interface_List_SizeW(
                out var listLength,
                ref GUID_DEVINTERFACE_USB_HUB,
                hubDevice.InstanceId,
                PInvoke.CM_GET_DEVICE_INTERFACE_LIST_PRESENT
            );

            if (ret != SetupApiWrapper.ConfigManagerResult.Success)
                throw new ConfigManagerException("Failed to get device interface list size.", ret);

            var bytesRequired = (int)listLength * 2;
            var listBuffer = IntPtr.Zero;
            var buffer = IntPtr.Zero;

            try
            {
                listBuffer = Marshal.AllocHGlobal(bytesRequired);

                ret = SetupApiWrapper.CM_Get_Device_Interface_ListW(
                    ref GUID_DEVINTERFACE_USB_HUB,
                    hubDevice.InstanceId,
                    listBuffer,
                    listLength,
                    SetupApiWrapper.CM_GET_DEVICE_INTERFACE_LIST_FLAG.CM_GET_DEVICE_INTERFACE_LIST_PRESENT
                );

                if (ret != SetupApiWrapper.ConfigManagerResult.Success)
                    throw new ConfigManagerException("Failed to get device interface list.", ret);

                var hubPath = listBuffer.MultiSzPointerToStringArray(bytesRequired).FirstOrDefault();

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
                buffer = Marshal.AllocHGlobal(size);

                Marshal.StructureToPtr(parameters, buffer, false);

                // request hub to power-cycle port, effectively force-restarting the device
                var success = PInvoke.DeviceIoControl(
                    hubHandle,
                    IOCTL_USB_HUB_CYCLE_PORT,
                    &buffer,
                    (uint)size,
                    &buffer,
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

                var result = Marshal.PtrToStructure<USB_CYCLE_PORT_PARAMS>(buffer);

                if (result.Status != 0)
                    throw new ArgumentException("Port cycle request failed.");
            }
            finally
            {
                Marshal.FreeHGlobal(listBuffer);
                Marshal.FreeHGlobal(buffer);
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