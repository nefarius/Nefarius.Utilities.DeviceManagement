# CustomDeviceProperty

Namespace: Nefarius.Utilities.DeviceManagement.PnP

Describes a custom device property.

```csharp
public class CustomDeviceProperty : DevicePropertyKey, System.IEquatable`1[[Nefarius.Utilities.DeviceManagement.PnP.DevicePropertyKey, Nefarius.Utilities.DeviceManagement, Version=3.0.0.0, Culture=neutral, PublicKeyToken=null]]
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [DevicePropertyKey](./nefarius.utilities.devicemanagement.pnp.devicepropertykey.md) → [CustomDeviceProperty](./nefarius.utilities.devicemanagement.pnp.customdeviceproperty.md)<br>
Implements [IEquatable&lt;DevicePropertyKey&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.iequatable-1)

## Properties

### <a id="properties-categoryguid"/>**CategoryGuid**

The [Guid](https://docs.microsoft.com/en-us/dotnet/api/system.guid) for teh category this property belongs to.

```csharp
public Guid CategoryGuid { get; }
```

#### Property Value

[Guid](https://docs.microsoft.com/en-us/dotnet/api/system.guid)<br>

### <a id="properties-propertyidentifier"/>**PropertyIdentifier**

The unique identifier withing the category group for this property.

```csharp
public uint PropertyIdentifier { get; }
```

#### Property Value

[UInt32](https://docs.microsoft.com/en-us/dotnet/api/system.uint32)<br>

### <a id="properties-propertytype"/>**PropertyType**

The managed type of the property (integer, string, array, ...).

```csharp
public Type PropertyType { get; }
```

#### Property Value

[Type](https://docs.microsoft.com/en-us/dotnet/api/system.type)<br>

## Methods

### <a id="methods-createcustomdeviceproperty"/>**CreateCustomDeviceProperty(Guid, UInt32, Type)**

Creates a custom device property.

```csharp
public static DevicePropertyKey CreateCustomDeviceProperty(Guid categoryGuid, uint propertyIdentifier, Type propertyType)
```

#### Parameters

`categoryGuid` [Guid](https://docs.microsoft.com/en-us/dotnet/api/system.guid)<br>
The category GUID.

`propertyIdentifier` [UInt32](https://docs.microsoft.com/en-us/dotnet/api/system.uint32)<br>
The property identifier.

`propertyType` [Type](https://docs.microsoft.com/en-us/dotnet/api/system.type)<br>
The managed type to translate from/to.

#### Returns

A new instance of [CustomDeviceProperty](./nefarius.utilities.devicemanagement.pnp.customdeviceproperty.md).
