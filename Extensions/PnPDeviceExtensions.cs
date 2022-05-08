using Nefarius.Utilities.DeviceManagement.PnP;

namespace Nefarius.Utilities.DeviceManagement.Extensions
{
    /// <summary>
    ///     Helper methods for <see cref="PnPDevice"/> objects.
    /// </summary>
    public static class PnPDeviceExtensions
    {
        /// <summary>
        ///     Creates a <see cref="UsbPnPDevice"/> from the provided <see cref="PnPDevice"/>.
        /// </summary>
        /// <param name="device">The <see cref="PnPDevice"/> to base this USB device on.</param>
        /// <returns>The new <see cref="UsbPnPDevice"/>.</returns>
        public static UsbPnPDevice ToUsbPnPDevice(this PnPDevice device)
        {
            return new UsbPnPDevice(device.InstanceId, DeviceLocationFlags.Phantom);
        }
    }
}
