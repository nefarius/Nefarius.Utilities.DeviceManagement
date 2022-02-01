# DevicePropertyKey

Namespace: Nefarius.Utilities.DeviceManagement.PnP

Describes a unified device property.

```csharp
public abstract class DevicePropertyKey : System.IEquatable`1[[Nefarius.Utilities.DeviceManagement.PnP.DevicePropertyKey, Nefarius.Utilities.DeviceManagement, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]]
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [DevicePropertyKey](./nefarius.utilities.devicemanagement.pnp.devicepropertykey.md)<br>
Implements [IEquatable&lt;DevicePropertyKey&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.iequatable-1)

## Properties

### **CategoryGuid**

The [Guid](https://docs.microsoft.com/en-us/dotnet/api/system.guid) for teh category this property belongs to.

```csharp
public Guid CategoryGuid { get; }
```

#### Property Value

[Guid](https://docs.microsoft.com/en-us/dotnet/api/system.guid)<br>

### **PropertyIdentifier**

The unique identifier withing the category group for this property.

```csharp
public uint PropertyIdentifier { get; }
```

#### Property Value

[UInt32](https://docs.microsoft.com/en-us/dotnet/api/system.uint32)<br>

### **PropertyType**

The managed type of the property (integer, string, array, ...).

```csharp
public Type PropertyType { get; }
```

#### Property Value

[Type](https://docs.microsoft.com/en-us/dotnet/api/system.type)<br>

## Methods

### **ToNativeType()**

Returns native type for managed type.

```csharp
internal DevPropKey ToNativeType()
```

#### Returns

[DevPropKey](./nefarius.utilities.devicemanagement.pnp.setupapiwrapper.devpropkey.md)<br>
The native .

### **Equals(DevicePropertyKey)**



```csharp
public bool Equals(DevicePropertyKey other)
```

#### Parameters

`other` [DevicePropertyKey](./nefarius.utilities.devicemanagement.pnp.devicepropertykey.md)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **Equals(Object)**



```csharp
public bool Equals(object obj)
```

#### Parameters

`obj` [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **GetHashCode()**



```csharp
public int GetHashCode()
```

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
