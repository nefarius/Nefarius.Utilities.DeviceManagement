# PnPDevice

Namespace: Nefarius.Utilities.DeviceManagement.PnP

Describes an instance of a PNP device.

```csharp
public class PnPDevice : IPnPDevice, System.IEquatable`1[[Nefarius.Utilities.DeviceManagement.PnP.PnPDevice, Nefarius.Utilities.DeviceManagement, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]]
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [PnPDevice](./nefarius.utilities.devicemanagement.pnp.pnpdevice.md)<br>
Implements [IPnPDevice](./nefarius.utilities.devicemanagement.pnp.ipnpdevice.md), [IEquatable&lt;PnPDevice&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.iequatable-1)

## Properties

### **InstanceId**

The instance ID of the device. Uniquely identifies devices of equal make and model on the same machine.

```csharp
public string InstanceId { get; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **DeviceId**

The device ID. Typically built from the hardware ID of the same make and model of hardware.

```csharp
public string DeviceId { get; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

## Methods

### **Restart()**

Attempts to restart this device. Device restart may fail if it has open handles that currently can not be
 force-closed.

```csharp
public void Restart()
```

### **Remove()**

Attempts to remove this device node.

```csharp
public void Remove()
```

#### Exceptions

[ConfigManagerException](./nefarius.utilities.devicemanagement.exceptions.configmanagerexception.md)<br>

### **IsVirtual(Func&lt;IPnPDevice, Boolean&gt;)**

Walks up the [PnPDevice](./nefarius.utilities.devicemanagement.pnp.pnpdevice.md)s parents chain to determine if the top most device is root enumerated.

```csharp
public bool IsVirtual(Func<IPnPDevice, bool> excludeIfMatches)
```

#### Parameters

`excludeIfMatches` [Func&lt;IPnPDevice, Boolean&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.func-2)<br>
Returns false if the given predicate is true.

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
True if this devices originates from an emulator, false otherwise.

**Remarks:**

This is achieved by walking up the node tree until the top most parent and check if the last parent below the
 tree root is a software device. Hardware devices originate from a PCI(e) bus while virtual devices originate from a
 root enumerated device.

### **Disable()**

Disables this device instance node.

```csharp
public void Disable()
```

#### Exceptions

[ConfigManagerException](./nefarius.utilities.devicemanagement.exceptions.configmanagerexception.md)<br>

### **Enable()**

Enables this device instance node.

```csharp
public void Enable()
```

#### Exceptions

[ConfigManagerException](./nefarius.utilities.devicemanagement.exceptions.configmanagerexception.md)<br>

### **ToString()**

```csharp
public string ToString()
```

#### Returns

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **Equals(PnPDevice)**

```csharp
public bool Equals(PnPDevice other)
```

#### Parameters

`other` [PnPDevice](./nefarius.utilities.devicemanagement.pnp.pnpdevice.md)<br>

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

### **GetProperty&lt;T&gt;(DevicePropertyKey)**

Returns a device instance property identified by [DevicePropertyKey](./nefarius.utilities.devicemanagement.pnp.devicepropertykey.md).

```csharp
public T GetProperty<T>(DevicePropertyKey propertyKey)
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

#### Exceptions

[ConfigManagerException](./nefarius.utilities.devicemanagement.exceptions.configmanagerexception.md)<br>
