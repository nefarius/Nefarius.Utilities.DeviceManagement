# PnPDevice

Namespace: Nefarius.Utilities.DeviceManagement.PnP

Describes an instance of a PNP device.

```csharp
public class PnPDevice
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [PnPDevice](./nefarius.utilities.devicemanagement.pnp.pnpdevice.md)

## Properties

### **InstanceId**

The instance ID of the device.

```csharp
public string InstanceId { get; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **DeviceId**

The device ID.

```csharp
public string DeviceId { get; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

## Methods

### **Restart()**

Attempts to restart this device. Device restart may fail if it has open handles that currently can not be force-closed.

```csharp
public void Restart()
```

### **GetDeviceByInstanceId(String)**

Return device identified by instance ID.

```csharp
public static PnPDevice GetDeviceByInstanceId(string instanceId)
```

#### Parameters

`instanceId` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
The instance ID of the device.

#### Returns

[PnPDevice](./nefarius.utilities.devicemanagement.pnp.pnpdevice.md)<br>
A .

### **GetDeviceByInstanceId(String, DeviceLocationFlags)**

Return device identified by instance ID.

```csharp
public static PnPDevice GetDeviceByInstanceId(string instanceId, DeviceLocationFlags flags)
```

#### Parameters

`instanceId` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
The instance ID of the device.

`flags` [DeviceLocationFlags](./nefarius.utilities.devicemanagement.pnp.devicelocationflags.md)<br>

#### Returns

[PnPDevice](./nefarius.utilities.devicemanagement.pnp.pnpdevice.md)<br>
A .

### **GetDeviceByInterfaceId(String, DeviceLocationFlags)**

Return device identified by instance ID/path (symbolic link).

```csharp
public static PnPDevice GetDeviceByInterfaceId(string symbolicLink, DeviceLocationFlags flags)
```

#### Parameters

`symbolicLink` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
The device interface path/ID/symbolic link name.

`flags` [DeviceLocationFlags](./nefarius.utilities.devicemanagement.pnp.devicelocationflags.md)<br>

#### Returns

[PnPDevice](./nefarius.utilities.devicemanagement.pnp.pnpdevice.md)<br>
A .

### **GetInstanceIdFromInterfaceId(String)**

Resolves Interface ID/Symbolic link/Device path to Instance ID.

```csharp
public static string GetInstanceIdFromInterfaceId(string symbolicLink)
```

#### Parameters

`symbolicLink` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
The device interface path/ID/symbolic link name.

#### Returns

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
The Instance ID.

### **GetDeviceByInterfaceId(String)**

Return device identified by instance ID/path (symbolic link).

```csharp
public static PnPDevice GetDeviceByInterfaceId(string symbolicLink)
```

#### Parameters

`symbolicLink` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
The device interface path/ID/symbolic link name.

#### Returns

[PnPDevice](./nefarius.utilities.devicemanagement.pnp.pnpdevice.md)<br>
A .

### **GetProperty&lt;T&gt;(DevicePropertyKey)**

Returns a device instance property identified by [DevicePropertyKey](./nefarius.utilities.devicemanagement.pnp.devicepropertykey.md).

```csharp
public T GetProperty<T>(DevicePropertyKey propertyKey)
```

#### Type Parameters

`T`<br>
The managed type of the fetched porperty value.

#### Parameters

`propertyKey` [DevicePropertyKey](./nefarius.utilities.devicemanagement.pnp.devicepropertykey.md)<br>
The  to query for.

#### Returns

T<br>
On success, the value of the queried property.

### **SetProperty&lt;T&gt;(DevicePropertyKey, T)**

Creates or updates an existing property with a given value.

```csharp
public void SetProperty<T>(DevicePropertyKey propertyKey, T propertyValue)
```

#### Type Parameters

`T`<br>
The type of the property.

#### Parameters

`propertyKey` [DevicePropertyKey](./nefarius.utilities.devicemanagement.pnp.devicepropertykey.md)<br>
The  to update.

`propertyValue` T<br>
The value to set.
