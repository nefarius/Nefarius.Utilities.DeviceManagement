using Nefarius.Utilities.DeviceManagement.PnP;

namespace Nefarius.Utilities.DeviceManagement.Extensions
{
    public static class PnPDeviceExtensions
    {
        public static UsbPnPDevice ToUsbPnPDevice(this PnPDevice device)
        {
            return new UsbPnPDevice(device.InstanceId, DeviceLocationFlags.Phantom);
        }
    }
}
