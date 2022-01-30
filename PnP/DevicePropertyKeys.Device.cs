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

        private class DevicePropertyDeviceInstanceId : DevicePropertyDevice
        {
            public DevicePropertyDeviceInstanceId()
                : base(Guid.Parse("{0x78c34fc8, 0x104a, 0x4aca, {0x9e, 0xa4, 0x52, 0x4d, 0x52, 0x99, 0x6e, 0x57}}"),
                    256, typeof(string))
            {
            }
        }

        private class DevicePropertyDeviceParent : DevicePropertyDevice
        {
            public DevicePropertyDeviceParent()
                : base(Guid.Parse("{0x4340a6c5, 0x93fa, 0x4706, {0x97, 0x2c, 0x7b, 0x64, 0x80, 0x08, 0xa5, 0xa7}}"),
                    8, typeof(string))
            {
            }
        }

        private class DevicePropertyDeviceChildren : DevicePropertyDevice
        {
            public DevicePropertyDeviceChildren()
                : base(Guid.Parse("{0x4340a6c5, 0x93fa, 0x4706, {0x97, 0x2c, 0x7b, 0x64, 0x80, 0x08, 0xa5, 0xa7}}"),
                    9, typeof(string[]))
            {
            }
        }

        private class DevicePropertyDeviceSiblings : DevicePropertyDevice
        {
            public DevicePropertyDeviceSiblings()
                : base(Guid.Parse("{0x4340a6c5, 0x93fa, 0x4706, {0x97, 0x2c, 0x7b, 0x64, 0x80, 0x08, 0xa5, 0xa7}}"),
                    10, typeof(string[]))
            {
            }
        }

        private class DevicePropertyDeviceDriverDate : DevicePropertyDevice
        {
            public DevicePropertyDeviceDriverDate()
                : base(Guid.Parse("{0xa8b865dd, 0x2e3d, 0x4094, {0xad, 0x97, 0xe5, 0x93, 0xa7, 0xc, 0x75, 0xd6}}"),
                    2, typeof(DateTimeOffset))
            {
            }
        }

        private class DevicePropertyDeviceDriverVersion : DevicePropertyDevice
        {
            public DevicePropertyDeviceDriverVersion()
                : base(Guid.Parse("{0xa8b865dd, 0x2e3d, 0x4094, {0xad, 0x97, 0xe5, 0x93, 0xa7, 0xc, 0x75, 0xd6}}"),
                    3, typeof(string))
            {
            }
        }

        private class DevicePropertyDeviceDriverDesc : DevicePropertyDevice
        {
            public DevicePropertyDeviceDriverDesc()
                : base(Guid.Parse("{0xa8b865dd, 0x2e3d, 0x4094, {0xad, 0x97, 0xe5, 0x93, 0xa7, 0xc, 0x75, 0xd6}}"),
                    4, typeof(string))
            {
            }
        }

        private class DevicePropertyDeviceDriverInfPath : DevicePropertyDevice
        {
            public DevicePropertyDeviceDriverInfPath()
                : base(Guid.Parse("{0xa8b865dd, 0x2e3d, 0x4094, {0xad, 0x97, 0xe5, 0x93, 0xa7, 0xc, 0x75, 0xd6}}"),
                    5, typeof(string))
            {
            }
        }

        private class DevicePropertyDeviceDriverInfSection : DevicePropertyDevice
        {
            public DevicePropertyDeviceDriverInfSection()
                : base(Guid.Parse("{0xa8b865dd, 0x2e3d, 0x4094, {0xad, 0x97, 0xe5, 0x93, 0xa7, 0xc, 0x75, 0xd6}}"),
                    6, typeof(string))
            {
            }
        }

        private class DevicePropertyDeviceMatchingDeviceId : DevicePropertyDevice
        {
            public DevicePropertyDeviceMatchingDeviceId()
                : base(Guid.Parse("{0xa8b865dd, 0x2e3d, 0x4094, {0xad, 0x97, 0xe5, 0x93, 0xa7, 0xc, 0x75, 0xd6}}"),
                    8, typeof(string))
            {
            }
        }

        private class DevicePropertyDeviceDriverProvider : DevicePropertyDevice
        {
            public DevicePropertyDeviceDriverProvider()
                : base(Guid.Parse("{0xa8b865dd, 0x2e3d, 0x4094, {0xad, 0x97, 0xe5, 0x93, 0xa7, 0xc, 0x75, 0xd6}}"),
                    9, typeof(string))
            {
            }
        }
    }
}
