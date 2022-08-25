# IPnPDevice

Namespace: Nefarius.Utilities.DeviceManagement.PnP

Describes an instance of a PNP device.

```csharp
public interface IPnPDevice
```

## Properties

### **InstanceId**

The instance ID of the device.

```csharp
public abstract string InstanceId { get; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **DeviceId**

The device ID.

```csharp
public abstract string DeviceId { get; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

## Methods

### **Restart()**

Attempts to restart this device. Device restart may fail if it has open handles that currently can not be force-closed.

```csharp
void Restart()
```

### **Remove()**

Attempts to remove this device node.

```csharp
void Remove()
```

### **IsVirtual()**

Walks up the [PnPDevice](./nefarius.utilities.devicemanagement.pnp.pnpdevice.md)s parents chain to determine if the top most device is root enumerated.

```csharp
bool IsVirtual()
```

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
True if this devices originates from an emulator, false otherwise.

### **GetProperty&lt;T&gt;(DevicePropertyKey)**

Returns a device instance property identified by [DevicePropertyKey](./nefarius.utilities.devicemanagement.pnp.devicepropertykey.md).

```csharp
T GetProperty<T>(DevicePropertyKey propertyKey)
```

#### Type Parameters

`T`<br>
The managed type of the fetched property value.

#### Parameters

`propertyKey` [DevicePropertyKey](./nefarius.utilities.devicemanagement.pnp.devicepropertykey.md)<br>
The  to query for.

#### Returns

T<br>
On success, the value of the queried property.

### **SetProperty&lt;T&gt;(DevicePropertyKey, T)**

Creates or updates an existing property with a given value.

```csharp
void SetProperty<T>(DevicePropertyKey propertyKey, T propertyValue)
```

#### Type Parameters

`T`<br>
The type of the property.

#### Parameters

`propertyKey` [DevicePropertyKey](./nefarius.utilities.devicemanagement.pnp.devicepropertykey.md)<br>
The  to update.

`propertyValue` T<br>
The value to set.
