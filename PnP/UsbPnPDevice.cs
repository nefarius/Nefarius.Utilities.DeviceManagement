using System;

namespace Nefarius.Utilities.DeviceManagement.PnP
{
    public class UsbPnPDevice : PnPDevice
    {
        protected UsbPnPDevice(string instanceId, DeviceLocationFlags flags) : base(instanceId, flags)
        {
            var className = GetProperty<string>(DevicePropertyDevice.Class);

            if (!Equals(className, "USB"))
                throw new ArgumentException("This device is not a USB device.");
        }
    }
}
