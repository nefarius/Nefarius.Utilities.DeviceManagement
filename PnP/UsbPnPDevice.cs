using System;
using System.Linq;
using System.Runtime.InteropServices;
using Nefarius.Utilities.DeviceManagement.Extensions;
using Nefarius.Utilities.DeviceManagement.Util;
using PInvoke;

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
        public void CyclePort()
        {
            var hubDevice = this;
            var compositeDevice = this;

            // find root hub
            while (hubDevice is not null)
            {
                var parentId = hubDevice.GetProperty<string>(DevicePropertyDevice.Parent);
                var service = hubDevice.GetProperty<string>(DevicePropertyDevice.Service);

                if (service is not null)
                {
                    // if one parent is a composite device, use their port number instead
                    if (service.Equals("usbccgp", StringComparison.OrdinalIgnoreCase) || 
                        service.StartsWith("xusb", StringComparison.OrdinalIgnoreCase))
                        compositeDevice = hubDevice;
                    // we have reached the hub object, bail
                    if (service.StartsWith("USBHUB", StringComparison.OrdinalIgnoreCase))
                        break;
                }

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
                SetupApiWrapper.CM_GET_DEVICE_INTERFACE_LIST_FLAG.CM_GET_DEVICE_INTERFACE_LIST_PRESENT
            );

            if (ret != SetupApiWrapper.ConfigManagerResult.Success)
                throw new Win32Exception(PInvoke.Kernel32.GetLastError(), "Failed to get device interface list size.");

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
                    throw new Win32Exception(PInvoke.Kernel32.GetLastError(), "Failed to get device interface list.");

                var hubPath = listBuffer.MultiSzPointerToStringArray(bytesRequired).FirstOrDefault();

                if (hubPath is null)
                    throw new ArgumentException("Failed to get device interface path.");

                using var hubHandle = PInvoke.Kernel32.CreateFile(hubPath,
                    PInvoke.Kernel32.ACCESS_MASK.GenericRight.GENERIC_READ |
                    PInvoke.Kernel32.ACCESS_MASK.GenericRight.GENERIC_WRITE,
                    PInvoke.Kernel32.FileShare.FILE_SHARE_READ | PInvoke.Kernel32.FileShare.FILE_SHARE_WRITE,
                    IntPtr.Zero, PInvoke.Kernel32.CreationDisposition.OPEN_EXISTING,
                    PInvoke.Kernel32.CreateFileFlags.FILE_ATTRIBUTE_NORMAL
                    | PInvoke.Kernel32.CreateFileFlags.FILE_FLAG_NO_BUFFERING
                    | PInvoke.Kernel32.CreateFileFlags.FILE_FLAG_WRITE_THROUGH,
                    PInvoke.Kernel32.SafeObjectHandle.Null
                );

                var size = Marshal.SizeOf<USB_CYCLE_PORT_PARAMS>();
                buffer = Marshal.AllocHGlobal(size);

                Marshal.StructureToPtr(parameters, buffer, false);

                // request hub to power-cycle port, effectively force-restarting the device
                var success = PInvoke.Kernel32.DeviceIoControl(
                    hubHandle,
                    IOCTL_USB_HUB_CYCLE_PORT,
                    buffer,
                    size,
                    buffer,
                    size,
                    out _,
                    IntPtr.Zero
                );

                var err = PInvoke.Kernel32.GetLastError();

                switch (success)
                {
                    case false when err == Win32ErrorCode.ERROR_GEN_FAILURE:
                        throw new ArgumentException("Request failed, this operation requires administrative privileges.");
                    /* STATUS_NO_SUCH_DEVICE */
                    case false when (int)err == 433:
                        throw new ArgumentException($"Request failed, device on port {compositeDevice.Port} not found.");
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