using System;
using Windows.Win32.Devices.Properties;

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

        internal DEVPROPKEY ToCsWin32Type()
        {
            return new DEVPROPKEY { fmtid = CategoryGuid, pid = PropertyIdentifier };
        }

        /// <inheritdoc />
        public bool Equals(DevicePropertyKey other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return CategoryGuid.Equals(other.CategoryGuid) && PropertyIdentifier == other.PropertyIdentifier && PropertyType.Equals(other.PropertyType);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj) || obj is DevicePropertyKey other && Equals(other);
        }

        /// <inheritdoc />
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
    public abstract partial class DevicePropertyDevice : DevicePropertyKey
    {
        /// <summary>
        ///     The Device Description.
        /// </summary>
        public static readonly DevicePropertyKey DeviceDesc = new DevicePropertyDeviceDeviceDesc();

        /// <summary>
        ///     The list of hardware IDs.
        /// </summary>
        public static readonly DevicePropertyKey HardwareIds = new DevicePropertyDeviceHardwareIds();

        /// <summary>
        ///     The list of compatible IDs.
        /// </summary>
        public static readonly DevicePropertyKey CompatibleIds = new DevicePropertyDeviceCompatibleIds();

        /// <summary>
        ///     The service name.
        /// </summary>
        public static readonly DevicePropertyKey Service = new DevicePropertyDeviceService();
        
        /// <summary>
        ///     The device class name.
        /// </summary>
        public static readonly DevicePropertyKey Class = new DevicePropertyDeviceClass();

        /// <summary>
        ///     The device class guid.
        /// </summary>
        public static readonly DevicePropertyKey ClassGuid = new DevicePropertyDeviceClassGuid();

        /// <summary>
        ///     The driver name.
        /// </summary>
        public static readonly DevicePropertyKey Driver = new DevicePropertyDeviceDriver();

        /// <summary>
        ///     Possible configuration flags.
        /// </summary>
        public static readonly DevicePropertyKey ConfigFlags = new DevicePropertyDeviceConfigFlags();

        /// <summary>
        ///     The manufacturer string.
        /// </summary>
        public static readonly DevicePropertyKey Manufacturer = new DevicePropertyDeviceManufacturer();

        /// <summary>
        ///     The friendly display name.
        /// </summary>
        public static readonly DevicePropertyKey FriendlyName = new DevicePropertyDeviceFriendlyName();

        /// <summary>
        ///     The location information.
        /// </summary>
        public static readonly DevicePropertyKey LocationInfo = new DevicePropertyDeviceLocationInfo();

        /// <summary>
        ///     The Physical Device Object name.
        /// </summary>
        public static readonly DevicePropertyKey PDOName = new DevicePropertyDevicePDOName();

        /// <summary>
        ///     The device capabilities.
        /// </summary>
        public static readonly DevicePropertyKey Capabilities = new DevicePropertyDeviceCapabilities();

        /// <summary>
        ///     The UI number.
        /// </summary>
        public static readonly DevicePropertyKey UINumber = new DevicePropertyDeviceUINumber();

        /// <summary>
        ///     The upper filters list.
        /// </summary>
        public static readonly DevicePropertyKey UpperFilters = new DevicePropertyDeviceUpperFilters();

        /// <summary>
        ///     The lower filters list.
        /// </summary>
        public static readonly DevicePropertyKey LowerFilters = new DevicePropertyDeviceLowerFilters();

        /// <summary>
        ///     The bus type GUILD.
        /// </summary>
        public static readonly DevicePropertyKey BusTypeGuid = new DevicePropertyDeviceBusTypeGuid();

        /// <summary>
        ///     The legacy bus type.
        /// </summary>
        public static readonly DevicePropertyKey LegacyBusType = new DevicePropertyDeviceLegacyBusType();

        /// <summary>
        ///     The bus number.
        /// </summary>
        public static readonly DevicePropertyKey BusNumber = new DevicePropertyDeviceBusNumber();
        
        /// <summary>
        ///     The enumerator name.
        /// </summary>
        public static readonly DevicePropertyKey EnumeratorName = new DevicePropertyDeviceEnumeratorName();
        
        /// <summary>
        ///     The instance ID.
        /// </summary>
        public static readonly DevicePropertyKey InstanceId = new DevicePropertyDeviceInstanceId();
        
        /// <summary>
        ///     The parent instance ID.
        /// </summary>
        public static readonly DevicePropertyKey Parent = new DevicePropertyDeviceParent();
        
        /// <summary>
        ///     The list of child instances, if any.
        /// </summary>
        public static readonly DevicePropertyKey Children = new DevicePropertyDeviceChildren();

        /// <summary>
        ///     The list of siblings, if any.
        /// </summary>
        public static readonly DevicePropertyKey Siblings = new DevicePropertyDeviceSiblings();

        /// <summary>
        ///     The driver release date.
        /// </summary>
        public static readonly DevicePropertyKey DriverDate = new DevicePropertyDeviceDriverDate();

        /// <summary>
        ///     The driver version.
        /// </summary>
        public static readonly DevicePropertyKey DriverVersion = new DevicePropertyDeviceDriverVersion();

        /// <summary>
        ///     The driver description.
        /// </summary>
        public static readonly DevicePropertyKey DriverDesc = new DevicePropertyDeviceDriverDesc();

        /// <summary>
        ///     The driver INF path.
        /// </summary>
        public static readonly DevicePropertyKey DriverInfPath = new DevicePropertyDeviceDriverInfPath();

        /// <summary>
        ///     The driver INF section.
        /// </summary>
        public static readonly DevicePropertyKey DriverInfSection = new DevicePropertyDeviceDriverInfSection();

        /// <summary>
        ///     The matching device ID.
        /// </summary>
        public static readonly DevicePropertyKey MatchingDeviceId = new DevicePropertyDeviceMatchingDeviceId();

        /// <summary>
        ///     The driver provider name.
        /// </summary>
        public static readonly DevicePropertyKey DriverProvider = new DevicePropertyDeviceDriverProvider();

        /// <summary>
        ///     The address (bus index, port number) the enumerator assigned to the device.
        /// </summary>
        public static readonly DevicePropertyKey Address = new DevicePropertyDeviceAddress();

        /// <summary>
        ///     The driver install date.
        /// </summary>
        public static readonly DevicePropertyKey InstallDate = new DevicePropertyDeviceInstallDate();

        /// <summary>
        ///     The driver first install date.
        /// </summary>
        public static readonly DevicePropertyKey FirstInstallDate = new DevicePropertyDeviceFirstInstallDate();


        /// <summary>
        ///     The driver last arrival date.
        /// </summary>
        public static readonly DevicePropertyKey LastArrivalDate = new DevicePropertyDeviceLastArrivalDate();


        /// <summary>
        ///     The driver last removal date
        /// </summary>
        public static readonly DevicePropertyKey LastRemovalDate = new DevicePropertyDeviceLastRemovalDate();


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
    }
}
