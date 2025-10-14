using System;
using System.Diagnostics.CodeAnalysis;

using Windows.Win32.Devices.Properties;
using Windows.Win32.Foundation;

namespace Nefarius.Utilities.DeviceManagement.PnP;

/// <summary>
///     Describes a unified device property.
/// </summary>
/// <remarks>https://docs.microsoft.com/en-us/windows-hardware/drivers/install/unified-device-property-model--windows-vista-and-later-</remarks>
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
public partial class DevicePropertyKey : IEquatable<DevicePropertyKey>
{
    internal DevicePropertyKey(
        Guid categoryGuid,
        uint propertyIdentifier,
        Type propertyType
    )
    {
        CategoryGuid = categoryGuid;
        PropertyIdentifier = propertyIdentifier;
        PropertyType = propertyType;
    }

    internal DevicePropertyKey(DEVPROPKEY key, Type managedType) : this(key.fmtid, key.pid, managedType)
    {
    }

    /// <summary>
    ///     The <see cref="Guid" /> for teh category this property belongs to.
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

    /// <inheritdoc />
    public bool Equals(DevicePropertyKey other)
    {
        if (ReferenceEquals(null, other))
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return CategoryGuid.Equals(other.CategoryGuid) && PropertyIdentifier == other.PropertyIdentifier &&
               PropertyType.Equals(other.PropertyType);
    }

    internal DEVPROPKEY ToCsWin32Type()
    {
        return new DEVPROPKEY { fmtid = CategoryGuid, pid = PropertyIdentifier };
    }

    /// <inheritdoc />
    public override bool Equals(object obj)
    {
        return ReferenceEquals(this, obj) || (obj is DevicePropertyKey other && Equals(other));
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        unchecked
        {
            int hashCode = CategoryGuid.GetHashCode();
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
    internal CustomDeviceProperty(Guid categoryGuid, uint propertyIdentifier, Type propertyType)
        : base(categoryGuid, propertyIdentifier, propertyType)
    {
    }

    /// <summary>
    ///     Creates a custom device property.
    /// </summary>
    /// <param name="categoryGuid">The category GUID.</param>
    /// <param name="propertyIdentifier">The property identifier.</param>
    /// <param name="propertyType">The managed type to translate from/to.</param>
    /// <returns>A new instance of <see cref="CustomDeviceProperty" />.</returns>
    public static DevicePropertyKey CreateCustomDeviceProperty(
        Guid categoryGuid,
        uint propertyIdentifier,
        Type propertyType
    )
    {
        return new CustomDeviceProperty(categoryGuid, propertyIdentifier, propertyType);
    }
}