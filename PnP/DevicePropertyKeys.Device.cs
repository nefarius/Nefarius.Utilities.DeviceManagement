using System;

namespace Nefarius.Utilities.DeviceManagement.PnP
{
    public abstract partial class DevicePropertyDevice
    {
        private class DevicePropertyDeviceDeviceDesc : DevicePropertyDevice
        {
            public DevicePropertyDeviceDeviceDesc() : base(2, typeof(string))
            {
            }
        }

        private class DevicePropertyDeviceHardwareIds : DevicePropertyDevice
        {
            public DevicePropertyDeviceHardwareIds() : base(3, typeof(string[]))
            {
            }
        }

        private class DevicePropertyDeviceCompatibleIds : DevicePropertyDevice
        {
            public DevicePropertyDeviceCompatibleIds() : base(4, typeof(string[]))
            {
            }
        }

        private class DevicePropertyDeviceService : DevicePropertyDevice
        {
            public DevicePropertyDeviceService() : base(6, typeof(string))
            {
            }
        }

        private class DevicePropertyDeviceClass : DevicePropertyDevice
        {
            public DevicePropertyDeviceClass() : base(9, typeof(string))
            {
            }
        }

        private class DevicePropertyDeviceClassGuid : DevicePropertyDevice
        {
            public DevicePropertyDeviceClassGuid() : base(10, typeof(Guid))
            {
            }
        }

        private class DevicePropertyDeviceDriver : DevicePropertyDevice
        {
            public DevicePropertyDeviceDriver() : base(11, typeof(string))
            {
            }
        }

        private class DevicePropertyDeviceConfigFlags : DevicePropertyDevice
        {
            public DevicePropertyDeviceConfigFlags() : base(12, typeof(UInt32))
            {
            }
        }

        private class DevicePropertyDeviceManufacturer : DevicePropertyDevice
        {
            public DevicePropertyDeviceManufacturer() : base(13, typeof(string))
            {
            }
        }

        private class DevicePropertyDeviceFriendlyName : DevicePropertyDevice
        {
            public DevicePropertyDeviceFriendlyName() : base(14, typeof(string))
            {
            }
        }

        private class DevicePropertyDeviceLocationInfo : DevicePropertyDevice
        {
            public DevicePropertyDeviceLocationInfo() : base(15, typeof(string))
            {
            }
        }

        private class DevicePropertyDevicePDOName : DevicePropertyDevice
        {
            public DevicePropertyDevicePDOName() : base(16, typeof(string))
            {
            }
        }

        private class DevicePropertyDeviceCapabilities : DevicePropertyDevice
        {
            public DevicePropertyDeviceCapabilities() : base(17, typeof(UInt32))
            {
            }
        }

        private class DevicePropertyDeviceUINumber : DevicePropertyDevice
        {
            public DevicePropertyDeviceUINumber() : base(18, typeof(UInt32))
            {
            }
        }

        private class DevicePropertyDeviceUpperFilters : DevicePropertyDevice
        {
            public DevicePropertyDeviceUpperFilters() : base(19, typeof(string[]))
            {
            }
        }

        private class DevicePropertyDeviceLowerFilters : DevicePropertyDevice
        {
            public DevicePropertyDeviceLowerFilters() : base(20, typeof(string[]))
            {
            }
        }

        private class DevicePropertyDeviceBusTypeGuid : DevicePropertyDevice
        {
            public DevicePropertyDeviceBusTypeGuid() : base(21, typeof(Guid))
            {
            }
        }

        private class DevicePropertyDeviceLegacyBusType : DevicePropertyDevice
        {
            public DevicePropertyDeviceLegacyBusType() : base(22, typeof(UInt32))
            {
            }
        }

        private class DevicePropertyDeviceBusNumber : DevicePropertyDevice
        {
            public DevicePropertyDeviceBusNumber() : base(23, typeof(UInt32))
            {
            }
        }

        private class DevicePropertyDeviceEnumeratorName : DevicePropertyDevice
        {
            public DevicePropertyDeviceEnumeratorName() : base(24, typeof(string))
            {
            }
        }
    }
}
