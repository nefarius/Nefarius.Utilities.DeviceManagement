using System;

namespace Nefarius.Utilities.DeviceManagement.PnP
{
    /// <summary>
    ///     Describes a unified device property.
    /// </summary>
    /// <remarks>https://docs.microsoft.com/en-us/windows-hardware/drivers/install/unified-device-property-model--windows-vista-and-later-</remarks>
    public abstract class DevicePropertyKey : IEquatable<DevicePropertyKey>
    {
        protected DevicePropertyKey(
            Guid categoryGuid,
            uint propertyIdentifier,
            Type propertyType
        )
        {
            CategoryGuid = categoryGuid;
            PropertyIdentifier = propertyIdentifier;
            PropertyType = propertyType;
        }

        /// <summary>
        ///     The <see cref="Guid"/> for teh category this property belongs to.
        /// </summary>
        public Guid CategoryGuid { get; }

        /// <summary>
        ///     The unique identifier withing the category group for this property.
        /// </summary>
        public uint PropertyIdentifier { get; }

        /// <summary>
        ///     The managed type of the property (integer, string, array, ...).
        /// </summary>
        public Type PropertyType { get; }

        /// <summary>
        ///     Returns native type for managed type.
        /// </summary>
        /// <returns>The native <see cref="SetupApiWrapper.DevPropKey"/>.</returns>
        internal SetupApiWrapper.DevPropKey ToNativeType()
        {
            return new SetupApiWrapper.DevPropKey(CategoryGuid, PropertyIdentifier);
        }

        public bool Equals(DevicePropertyKey other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return CategoryGuid.Equals(other.CategoryGuid) && PropertyIdentifier == other.PropertyIdentifier && PropertyType.Equals(other.PropertyType);
        }

        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj) || obj is DevicePropertyKey other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = CategoryGuid.GetHashCode();
                hashCode = (hashCode * 397) ^ (int)PropertyIdentifier;
                hashCode = (hashCode * 397) ^ PropertyType.GetHashCode();
                return hashCode;
            }
        }
    }

    /// <summary>
    ///     Describes a custom device property.
    /// </summary>
    public class CustomDeviceProperty : DevicePropertyKey
    {
        protected CustomDeviceProperty(Guid categoryGuid, uint propertyIdentifier, Type propertyType)
            : base(categoryGuid, propertyIdentifier, propertyType)
        {
        }

        public static DevicePropertyKey CreateCustomDeviceProperty(
            Guid categoryGuid,
            uint propertyIdentifier,
            Type propertyType
        )
        {
            return new CustomDeviceProperty(categoryGuid, propertyIdentifier, propertyType);
        }
    }

    /// <summary>
    ///     Common device property definitions.
    /// </summary>
    public abstract class DevicePropertyDevice : DevicePropertyKey
    {
        /// <summary>
        ///     The Device Description.
        /// </summary>
        public static DevicePropertyKey DeviceDesc = new DevicePropertyDeviceDeviceDesc();

        /// <summary>
        ///     The list of hardware IDs.
        /// </summary>
        public static DevicePropertyKey HardwareIds = new DevicePropertyDeviceHardwareIds();

        /// <summary>
        ///     The list of compatible IDs.
        /// </summary>
        public static DevicePropertyKey CompatibleIds = new DevicePropertyDeviceCompatibleIds();
        
        /// <summary>
        ///     The device class name.
        /// </summary>
        public static DevicePropertyKey Class = new DevicePropertyDeviceClass();

        /// <summary>
        ///     The device class guid.
        /// </summary>
        public static DevicePropertyKey ClassGuid = new DevicePropertyDeviceClassGuid();

        /// <summary>
        ///     The manufacturer string.
        /// </summary>
        public static DevicePropertyKey Manufacturer = new DevicePropertyDeviceManufacturer();

        /// <summary>
        ///     The friendly display name.
        /// </summary>
        public static DevicePropertyKey FriendlyName = new DevicePropertyDeviceFriendlyName();

        /// <summary>
        ///     The enumerator name.
        /// </summary>
        public static DevicePropertyKey EnumeratorName = new DevicePropertyDeviceEnumeratorName();
        
        /// <summary>
        ///     The instance ID.
        /// </summary>
        public static DevicePropertyKey InstanceId = new DevicePropertyDeviceInstanceId();
        
        /// <summary>
        ///     The parent instance ID.
        /// </summary>
        public static DevicePropertyKey Parent = new DevicePropertyDeviceParent();
        
        /// <summary>
        ///     The list of child instances, if any.
        /// </summary>
        public static DevicePropertyKey Children = new DevicePropertyDeviceChildren();

        private DevicePropertyDevice(uint propertyIdentifier, Type propertyType) : this(
            Guid.Parse("{0xa45c254e, 0xdf1c, 0x4efd, {0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0}}"),
            propertyIdentifier,
            propertyType
        )
        {
        }

        protected DevicePropertyDevice(Guid categoryGuid, uint propertyIdentifier, Type propertyType)
            : base(categoryGuid, propertyIdentifier, propertyType)
        {
        }

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
    }
}