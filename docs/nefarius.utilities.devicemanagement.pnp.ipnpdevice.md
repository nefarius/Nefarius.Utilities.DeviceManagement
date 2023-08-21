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

Attempts to restart this device. Device restart may fail if it has open handles that currently can not be
 force-closed.

```csharp
void Restart()
```

### **Remove()**

Attempts to remove this device node.

```csharp
void Remove()
```

### **IsVirtual(Func&lt;IPnPDevice, Boolean&gt;)**

Walks up the [PnPDevice](./nefarius.utilities.devicemanagement.pnp.pnpdevice.md)s parents chain to determine if the top most device is root enumerated.

```csharp
bool IsVirtual(Func<IPnPDevice, bool> excludeIfMatches)
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

### **InstallNullDriver()**

Installs the NULL-driver on this device instance.

```csharp
void InstallNullDriver()
```

**Remarks:**

This will tear down the current device stack (no matter how many open handles exist), remove the existing function
 driver and reboot the device in "raw" or "driverless" mode. Some USB devices may require a port-cycle afterwards
 for the change to take effect without requiring a reboot.

### **InstallNullDriver(Boolean&)**

Installs the NULL-driver on this device instance.

```csharp
void InstallNullDriver(Boolean& rebootRequired)
```

#### Parameters

`rebootRequired` [Boolean&](https://docs.microsoft.com/en-us/dotnet/api/system.boolean&)<br>

**Remarks:**

This will tear down the current device stack (no matter how many open handles exist), remove the existing function
 driver and reboot the device in "raw" or "driverless" mode. Some USB devices may require a port-cycle afterwards
 for the change to take effect without requiring a reboot.

### **InstallCustomDriver(String)**

Installs a custom driver identified by the provided INF name on this device instance.

```csharp
void InstallCustomDriver(string infName)
```

#### Parameters

`infName` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

                The INF file name as found in C:\Windows\INF directory. It must be the name only, not a relative
                or absolute path.

**Remarks:**

This method force-installs a given INF file on this device instance, even if no matching hardware or compatible IDs
 are found. This method can only succeed if  is called prior.

### **InstallCustomDriver(String, Boolean&)**

Installs a custom driver identified by the provided INF name on this device instance.

```csharp
void InstallCustomDriver(string infName, Boolean& rebootRequired)
```

#### Parameters

`infName` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

                The INF file name as found in C:\Windows\INF directory. It must be the name only, not a relative
                or absolute path.

`rebootRequired` [Boolean&](https://docs.microsoft.com/en-us/dotnet/api/system.boolean&)<br>

                Gets whether a reboot is required for the changes to take effect or not.

**Remarks:**

This method force-installs a given INF file on this device instance, even if no matching hardware or compatible IDs
 are found. This method can only succeed if  is called prior.

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
