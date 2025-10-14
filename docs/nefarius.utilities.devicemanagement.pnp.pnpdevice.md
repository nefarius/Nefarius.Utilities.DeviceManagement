# PnPDevice

Namespace: Nefarius.Utilities.DeviceManagement.PnP

Describes an instance of a PNP device.

```csharp
public class PnPDevice : IPnPDevice, System.IEquatable`1[[Nefarius.Utilities.DeviceManagement.PnP.PnPDevice, Nefarius.Utilities.DeviceManagement, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [PnPDevice](./nefarius.utilities.devicemanagement.pnp.pnpdevice.md)<br>
Implements [IPnPDevice](./nefarius.utilities.devicemanagement.pnp.ipnpdevice.md), [IEquatable&lt;PnPDevice&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.iequatable-1)

## Properties

### <a id="properties-children"/>**Children**

Children of this [IPnPDevice](./nefarius.utilities.devicemanagement.pnp.ipnpdevice.md), if any.

```csharp
public IEnumerable<IPnPDevice> Children { get; }
```

#### Property Value

[IEnumerable&lt;IPnPDevice&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br>

### <a id="properties-compatibleids"/>**CompatibleIds**

List of compatible IDs, if any.

```csharp
public IEnumerable<String> CompatibleIds { get; }
```

#### Property Value

[IEnumerable&lt;String&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br>

### <a id="properties-deviceid"/>**DeviceId**

The device ID. Typically built from the hardware ID of the same make and model of hardware.

```csharp
public string DeviceId { get; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### <a id="properties-hardwareids"/>**HardwareIds**

List of hardware IDs, if any.

```csharp
public IEnumerable<String> HardwareIds { get; }
```

#### Property Value

[IEnumerable&lt;String&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br>

### <a id="properties-instanceid"/>**InstanceId**

The instance ID of the device. Uniquely identifies devices of equal make and model on the same machine.

```csharp
public string InstanceId { get; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### <a id="properties-parent"/>**Parent**

The parent of this [IPnPDevice](./nefarius.utilities.devicemanagement.pnp.ipnpdevice.md), if any.

```csharp
public IPnPDevice Parent { get; }
```

#### Property Value

[IPnPDevice](./nefarius.utilities.devicemanagement.pnp.ipnpdevice.md)<br>

### <a id="properties-siblings"/>**Siblings**

Siblings of this [IPnPDevice](./nefarius.utilities.devicemanagement.pnp.ipnpdevice.md) sharing the same parent, if any.

```csharp
public IEnumerable<IPnPDevice> Siblings { get; }
```

#### Property Value

[IEnumerable&lt;IPnPDevice&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br>

## Methods

### <a id="methods-disable"/>**Disable()**

Disables this device instance node.

```csharp
public void Disable()
```

#### Exceptions

[ConfigManagerException](./nefarius.utilities.devicemanagement.exceptions.configmanagerexception.md)<br>

### <a id="methods-enable"/>**Enable()**

Enables this device instance node.

```csharp
public void Enable()
```

#### Exceptions

[ConfigManagerException](./nefarius.utilities.devicemanagement.exceptions.configmanagerexception.md)<br>

### <a id="methods-equals"/>**Equals(PnPDevice)**

```csharp
public bool Equals(PnPDevice other)
```

#### Parameters

`other` [PnPDevice](./nefarius.utilities.devicemanagement.pnp.pnpdevice.md)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

### <a id="methods-equals"/>**Equals(Object)**

```csharp
public bool Equals(object obj)
```

#### Parameters

`obj` [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

### <a id="methods-getdevicebyinstanceid"/>**GetDeviceByInstanceId(String, DeviceLocationFlags)**

Return device identified by instance ID.

```csharp
public static PnPDevice GetDeviceByInstanceId(string instanceId, DeviceLocationFlags flags)
```

#### Parameters

`instanceId` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
The instance ID of the device.

`flags` [DeviceLocationFlags](./nefarius.utilities.devicemanagement.pnp.devicelocationflags.md)<br>
[DeviceLocationFlags](./nefarius.utilities.devicemanagement.pnp.devicelocationflags.md)

#### Returns

A [PnPDevice](./nefarius.utilities.devicemanagement.pnp.pnpdevice.md).

#### Exceptions

[PnPDeviceNotFoundException](./nefarius.utilities.devicemanagement.exceptions.pnpdevicenotfoundexception.md)<br>
The desired device instance was not found on the system.

[ConfigManagerException](./nefarius.utilities.devicemanagement.exceptions.configmanagerexception.md)<br>
Device information lookup failed.

[ArgumentOutOfRangeException](https://docs.microsoft.com/en-us/dotnet/api/system.argumentoutofrangeexception)<br>
The supplied `flags` value was invalid.

### <a id="methods-getdevicebyinterfaceid"/>**GetDeviceByInterfaceId(String, DeviceLocationFlags)**

Return device identified by instance ID/path (symbolic link).

```csharp
public static PnPDevice GetDeviceByInterfaceId(string symbolicLink, DeviceLocationFlags flags)
```

#### Parameters

`symbolicLink` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
The device interface path/ID/symbolic link name.

`flags` [DeviceLocationFlags](./nefarius.utilities.devicemanagement.pnp.devicelocationflags.md)<br>
[DeviceLocationFlags](./nefarius.utilities.devicemanagement.pnp.devicelocationflags.md)

#### Returns

A [PnPDevice](./nefarius.utilities.devicemanagement.pnp.pnpdevice.md) or null if not found.

#### Exceptions

[ConfigManagerException](./nefarius.utilities.devicemanagement.exceptions.configmanagerexception.md)<br>
Interface lookup failed.

### <a id="methods-gethashcode"/>**GetHashCode()**

```csharp
public int GetHashCode()
```

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)

### <a id="methods-getinstanceidfrominterfaceid"/>**GetInstanceIdFromInterfaceId(String)**

Resolves Interface ID/Symbolic link/Device path to Instance ID.

```csharp
public static string GetInstanceIdFromInterfaceId(string symbolicLink)
```

#### Parameters

`symbolicLink` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
The device interface path/ID/symbolic link name.

#### Returns

The Instance ID or null if not found.

#### Exceptions

[ConfigManagerException](./nefarius.utilities.devicemanagement.exceptions.configmanagerexception.md)<br>
Interface lookup failed.

### <a id="methods-getproperty"/>**GetProperty&lt;T&gt;(DevicePropertyKey)**

Returns a device instance property identified by [DevicePropertyKey](./nefarius.utilities.devicemanagement.pnp.devicepropertykey.md).

```csharp
public T GetProperty<T>(DevicePropertyKey propertyKey)
```

#### Type Parameters

`T`<br>
The managed type of the fetched property value.

#### Parameters

`propertyKey` [DevicePropertyKey](./nefarius.utilities.devicemanagement.pnp.devicepropertykey.md)<br>
The [DevicePropertyKey](./nefarius.utilities.devicemanagement.pnp.devicepropertykey.md) to query for.

#### Returns

On success, the value of the queried property.

**Remarks:**

If the queried property doesn't exist, the default value of the managed type is returned.

### <a id="methods-installcustomdriver"/>**InstallCustomDriver(String)**

Installs a custom driver identified by the provided INF name on this device instance.

```csharp
public void InstallCustomDriver(string infName)
```

#### Parameters

`infName` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
The INF file name as found in C:\Windows\INF directory. It must be the name only, not a relative
 or absolute path.

**Remarks:**

This method force-installs a given INF file on this device instance, even if no matching hardware or compatible IDs
 are found. This method can only succeed if [PnPDevice.InstallNullDriver()](./nefarius.utilities.devicemanagement.pnp.pnpdevice.md#installnulldriver) is called prior.

### <a id="methods-installcustomdriver"/>**InstallCustomDriver(String, ref Boolean)**

Installs a custom driver identified by the provided INF name on this device instance.

```csharp
public void InstallCustomDriver(string infName, ref Boolean rebootRequired)
```

#### Parameters

`infName` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
The INF file name as found in C:\Windows\INF directory. It must be the name only, not a relative
 or absolute path.

`rebootRequired` [Boolean&](https://docs.microsoft.com/en-us/dotnet/api/system.boolean&)<br>
Gets whether a reboot is required for the changes to take effect or not.

**Remarks:**

This method force-installs a given INF file on this device instance, even if no matching hardware or compatible IDs
 are found. This method can only succeed if [PnPDevice.InstallNullDriver()](./nefarius.utilities.devicemanagement.pnp.pnpdevice.md#installnulldriver) is called prior.

### <a id="methods-installnulldriver"/>**InstallNullDriver()**

Installs the NULL-driver on this device instance.

```csharp
public void InstallNullDriver()
```

**Remarks:**

This will tear down the current device stack (no matter how many open handles exist), remove the existing function
 driver and reboot the device in "raw" or "driverless" mode. Some USB devices may require a port-cycle afterward
 for the change to take effect without requiring a reboot.

### <a id="methods-installnulldriver"/>**InstallNullDriver(ref Boolean)**

Installs the NULL-driver on this device instance.

```csharp
public void InstallNullDriver(ref Boolean rebootRequired)
```

#### Parameters

`rebootRequired` [Boolean&](https://docs.microsoft.com/en-us/dotnet/api/system.boolean&)<br>

**Remarks:**

This will tear down the current device stack (no matter how many open handles exist), remove the existing function
 driver and reboot the device in "raw" or "driverless" mode. Some USB devices may require a port-cycle afterward
 for the change to take effect without requiring a reboot.

### <a id="methods-isvirtual"/>**IsVirtual(Func&lt;IPnPDevice, Boolean&gt;)**

Walks up the [PnPDevice](./nefarius.utilities.devicemanagement.pnp.pnpdevice.md)s parents chain to determine if the top most device is root enumerated.

```csharp
public bool IsVirtual(Func<IPnPDevice, Boolean> excludeIfMatches)
```

#### Parameters

`excludeIfMatches` [Func&lt;IPnPDevice, Boolean&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.func-2)<br>
Returns false if the given predicate is true.

#### Returns

True if this device originates from an emulator, false otherwise.

**Remarks:**

This is achieved by walking up the node tree until the top most parent and check if the last parent below the
 tree root is a software device. Hardware devices originate from a PCI(e) bus while virtual devices originate from a
 root enumerated device.

### <a id="methods-remove"/>**Remove()**

Attempts to remove this device node.

```csharp
public void Remove()
```

#### Exceptions

[ConfigManagerException](./nefarius.utilities.devicemanagement.exceptions.configmanagerexception.md)<br>
CM API failure occurred.

**Remarks:**

This call DOES NOT invoke device and driver uninstall routines, as soon as the device is re-enumerated, it
 will reappear and become online.

### <a id="methods-removeandsetup"/>**RemoveAndSetup()**

Attempts to restart this device by removing it from the device tree and causing re-enumeration afterwards.

```csharp
public void RemoveAndSetup()
```

#### Exceptions

[ConfigManagerException](./nefarius.utilities.devicemanagement.exceptions.configmanagerexception.md)<br>

**Remarks:**

Device restart may fail if it has open handles that currently can not be force-closed.

### <a id="methods-restart"/>**Restart()**

#### Caution

This method can cause unintended side-effects, see remarks for details.

---

Attempts to restart this device. Device restart may fail if it has open handles that currently cannot be
 force-closed.

```csharp
public void Restart()
```

**Remarks:**

This method removes and re-enumerates (adds) the device note, which might cause unintended side effects. If
 this is the behavior you seek, consider using [PnPDevice.RemoveAndSetup()](./nefarius.utilities.devicemanagement.pnp.pnpdevice.md#removeandsetup) instead. This method remains here for
 backwards compatibility.

### <a id="methods-setproperty"/>**SetProperty&lt;T&gt;(DevicePropertyKey, T)**

Creates or updates an existing property with a given value.

```csharp
public void SetProperty<T>(DevicePropertyKey propertyKey, T propertyValue)
```

#### Type Parameters

`T`<br>
The type of the property.

#### Parameters

`propertyKey` [DevicePropertyKey](./nefarius.utilities.devicemanagement.pnp.devicepropertykey.md)<br>
The [DevicePropertyKey](./nefarius.utilities.devicemanagement.pnp.devicepropertykey.md) to update.

`propertyValue` T<br>
The value to set.

### <a id="methods-tostring"/>**ToString()**

```csharp
public string ToString()
```

#### Returns

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)

### <a id="methods-uninstall"/>**Uninstall()**

Uninstalls this device instance. Unlike [PnPDevice.Remove()](./nefarius.utilities.devicemanagement.pnp.pnpdevice.md#remove) this call will unload and revert the device function
 driver to the best available compatible candidate on next device boot.

```csharp
public void Uninstall()
```

#### Exceptions

[Win32Exception](./nefarius.utilities.devicemanagement.exceptions.win32exception.md)<br>

[ArgumentOutOfRangeException](https://docs.microsoft.com/en-us/dotnet/api/system.argumentoutofrangeexception)<br>

**Remarks:**

If this is used in combination with [PnPDevice.InstallNullDriver()](./nefarius.utilities.devicemanagement.pnp.pnpdevice.md#installnulldriver) or
 [PnPDevice.InstallCustomDriver(String)](./nefarius.utilities.devicemanagement.pnp.pnpdevice.md#installcustomdriverstring), you can call [Devcon.Refresh()](./nefarius.utilities.devicemanagement.pnp.devcon.md#refresh) afterwards to trigger device
 installation.

### <a id="methods-uninstall"/>**Uninstall(ref Boolean)**

Uninstalls this device instance. Unlike [PnPDevice.Remove()](./nefarius.utilities.devicemanagement.pnp.pnpdevice.md#remove) this call will unload and revert the device function
 driver to the best available compatible candidate on next device boot.

```csharp
public void Uninstall(ref Boolean rebootRequired)
```

#### Parameters

`rebootRequired` [Boolean&](https://docs.microsoft.com/en-us/dotnet/api/system.boolean&)<br>
Gets whether a reboot is required for the changes to take effect or not.

#### Exceptions

[Win32Exception](./nefarius.utilities.devicemanagement.exceptions.win32exception.md)<br>

[ArgumentOutOfRangeException](https://docs.microsoft.com/en-us/dotnet/api/system.argumentoutofrangeexception)<br>

**Remarks:**

If this is used in combination with [PnPDevice.InstallNullDriver()](./nefarius.utilities.devicemanagement.pnp.pnpdevice.md#installnulldriver) or
 [PnPDevice.InstallCustomDriver(String)](./nefarius.utilities.devicemanagement.pnp.pnpdevice.md#installcustomdriverstring), you can call [Devcon.Refresh()](./nefarius.utilities.devicemanagement.pnp.devcon.md#refresh) afterwards to trigger device
 installation.
