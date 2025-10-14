# Devcon

Namespace: Nefarius.Utilities.DeviceManagement.PnP

"Device Console" utility class. Managed wrapper for common SetupAPI actions.

```csharp
public static class Devcon
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [Devcon](./nefarius.utilities.devicemanagement.pnp.devcon.md)

**Remarks:**

https://docs.microsoft.com/en-us/windows-hardware/drivers/install/setupapi

## Methods

### <a id="methods-create"/>**Create(String, Guid, String)**

Creates a virtual device node (hardware ID) in the provided device class.

```csharp
public static bool Create(string className, Guid classGuid, string node)
```

#### Parameters

`className` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
The device class name.

`classGuid` [Guid](https://docs.microsoft.com/en-us/dotnet/api/system.guid)<br>
The GUID of the device class.

`node` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
The node path terminated by two null characters.

#### Returns

True on success, false otherwise.

### <a id="methods-deletedriver"/>**DeleteDriver(String, String, Boolean)**

Uninstalls a driver identified via a given INF and optionally removes it from the driver store as well.

```csharp
public static void DeleteDriver(string oemInfName, string fullInfPath, bool forceDelete)
```

#### Parameters

`oemInfName` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
The OEM INF name (name and extension only).

`fullInfPath` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
The fully qualified absolute path to the INF to remove from the driver store.

`forceDelete` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
Remove the driver store copy, if true.

### <a id="methods-find"/>**Find(Guid, ref String, ref String, Int32)**

#### Caution

Do not use, see remarks.

---

Searches for devices matching the provided interface GUID and returns the device path and instance ID.

```csharp
public static bool Find(Guid target, ref String path, ref String instanceId, int instance)
```

#### Parameters

`target` [Guid](https://docs.microsoft.com/en-us/dotnet/api/system.guid)<br>
The class GUID to enumerate.

`path` [String&](https://docs.microsoft.com/en-us/dotnet/api/system.string&)<br>
The device path of the enumerated device.

`instanceId` [String&](https://docs.microsoft.com/en-us/dotnet/api/system.string&)<br>
The instance ID of the enumerated device.

`instance` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
Optional instance ID (zero-based) specifying the device to process on multiple matches.

#### Returns

True if at least one device was found with the provided class, false otherwise.

**Remarks:**

This is here for backwards compatibility, please use
 [Devcon.FindByInterfaceGuid(Guid, ref String, ref String, Int32, Boolean)](./nefarius.utilities.devicemanagement.pnp.devcon.md#findbyinterfaceguidguid-ref-string-ref-string-int32-boolean) instead.

### <a id="methods-findbyinterfaceguid"/>**FindByInterfaceGuid(Guid, ref String, ref String, Int32, Boolean)**

Searches for devices matching the provided interface GUID and returns the device path and instance ID.

```csharp
public static bool FindByInterfaceGuid(Guid target, ref String path, ref String instanceId, int instance, bool presentOnly)
```

#### Parameters

`target` [Guid](https://docs.microsoft.com/en-us/dotnet/api/system.guid)<br>
The interface GUID to enumerate.

`path` [String&](https://docs.microsoft.com/en-us/dotnet/api/system.string&)<br>
The device path of the enumerated device.

`instanceId` [String&](https://docs.microsoft.com/en-us/dotnet/api/system.string&)<br>
The instance ID of the enumerated device.

`instance` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
Optional instance ID (zero-based) specifying the device to process on multiple matches.

`presentOnly` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
Only enumerate currently connected devices by default, set to False to also include phantom
 devices.

#### Returns

True if at least one device was found with the provided class, false otherwise.

### <a id="methods-findbyinterfaceguid"/>**FindByInterfaceGuid(Guid, ref PnPDevice, Int32, Boolean)**

Searches for devices matching the provided interface GUID and returns a [PnPDevice](./nefarius.utilities.devicemanagement.pnp.pnpdevice.md).

```csharp
public static bool FindByInterfaceGuid(Guid target, ref PnPDevice device, int instance, bool presentOnly)
```

#### Parameters

`target` [Guid](https://docs.microsoft.com/en-us/dotnet/api/system.guid)<br>
The interface GUID to enumerate.

`device` [PnPDevice&](./nefarius.utilities.devicemanagement.pnp.pnpdevice&.md)<br>
The [PnPDevice](./nefarius.utilities.devicemanagement.pnp.pnpdevice.md) wrapper object.

`instance` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
Optional instance ID (zero-based) specifying the device to process on multiple matches.

`presentOnly` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
Only enumerate currently connected devices by default, set to False to also include phantom
 devices.

#### Returns

True if at least one device was found with the provided class, false otherwise.

### <a id="methods-findindeviceclassbyhardwareid"/>**FindInDeviceClassByHardwareId(Guid, String)**

Attempts to find a device within a specified device class by a given hardware ID.

```csharp
public static bool FindInDeviceClassByHardwareId(Guid target, string hardwareId)
```

#### Parameters

`target` [Guid](https://docs.microsoft.com/en-us/dotnet/api/system.guid)<br>
The device class GUID.

`hardwareId` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
The hardware ID to search for.

#### Returns

True if found, false otherwise.

### <a id="methods-findindeviceclassbyhardwareid"/>**FindInDeviceClassByHardwareId(Guid, String, ref IEnumerable`1)**

Attempts to find a device within a specified device class by a given hardware ID.

```csharp
public static bool FindInDeviceClassByHardwareId(Guid target, string hardwareId, ref IEnumerable`1 instanceIds)
```

#### Parameters

`target` [Guid](https://docs.microsoft.com/en-us/dotnet/api/system.guid)<br>
The device class GUID.

`hardwareId` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
The hardware ID to search for.

`instanceIds` [IEnumerable`1&](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1&)<br>
A list of instances found for the given search criteria.

#### Returns

True if found, false otherwise.

### <a id="methods-findindeviceclassbyhardwareid"/>**FindInDeviceClassByHardwareId(Guid, String, ref IEnumerable`1, Boolean, Boolean)**

Attempts to find a device within a specified device class by a given hardware ID.

```csharp
public static bool FindInDeviceClassByHardwareId(Guid target, string hardwareId, ref IEnumerable`1 instanceIds, bool presentOnly, bool allowPartial)
```

#### Parameters

`target` [Guid](https://docs.microsoft.com/en-us/dotnet/api/system.guid)<br>
The device class GUID.

`hardwareId` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
The hardware ID to search for.

`instanceIds` [IEnumerable`1&](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1&)<br>
A list of instances found for the given search criteria.

`presentOnly` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
True to filter currently plugged in devices, false to get all matching devices.

`allowPartial` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
True to match substrings, false to match the exact ID value.

#### Returns

True if found, false otherwise.

### <a id="methods-install"/>**Install(String, ref Boolean)**

Invokes the installation of a driver via provided `.INF` file.

```csharp
public static bool Install(string fullInfPath, ref Boolean rebootRequired)
```

#### Parameters

`fullInfPath` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
An absolute path to the .INF file to install.

`rebootRequired` [Boolean&](https://docs.microsoft.com/en-us/dotnet/api/system.boolean&)<br>
True if a machine reboot is required, false otherwise.

#### Returns

True on success, false otherwise.

### <a id="methods-refresh"/>**Refresh()**

Instructs the system to re-enumerate hardware devices.

```csharp
public static bool Refresh()
```

#### Returns

True on success, false otherwise.

### <a id="methods-refreshphantom"/>**RefreshPhantom()**

Instructs the system to re-enumerate hardware devices, including disconnected ones.

```csharp
public static bool RefreshPhantom()
```

#### Returns

True on success, false otherwise.

### <a id="methods-remove"/>**Remove(Guid, String)**

Removed a device node identified by class GUID, path and instance ID.

```csharp
public static bool Remove(Guid classGuid, string instanceId)
```

#### Parameters

`classGuid` [Guid](https://docs.microsoft.com/en-us/dotnet/api/system.guid)<br>
The device class GUID.

`instanceId` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
The instance ID.

#### Returns

True on success, false otherwise.

### <a id="methods-remove"/>**Remove(Guid, String, ref Boolean)**

Removed a device node identified by interface GUID and instance ID.

```csharp
public static bool Remove(Guid classGuid, string instanceId, ref Boolean rebootRequired)
```

#### Parameters

`classGuid` [Guid](https://docs.microsoft.com/en-us/dotnet/api/system.guid)<br>
The device class GUID.

`instanceId` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
The instance ID.

`rebootRequired` [Boolean&](https://docs.microsoft.com/en-us/dotnet/api/system.boolean&)<br>
True if a reboot is required to complete the uninstallation action, false otherwise.

#### Returns

True on success, false otherwise.

### <a id="methods-update"/>**Update(String, String, ref Boolean)**

Given an INF file and a hardware ID, this function installs updated drivers for devices that match the hardware ID.

```csharp
public static bool Update(string hardwareId, string fullInfPath, ref Boolean rebootRequired)
```

#### Parameters

`hardwareId` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
A string that supplies the hardware identifier to match existing devices on the computer.

`fullInfPath` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
A string that supplies the full path file name of an INF file.

`rebootRequired` [Boolean&](https://docs.microsoft.com/en-us/dotnet/api/system.boolean&)<br>
A variable that indicates whether a restart is required and who should prompt for it.

#### Returns

The function returns TRUE if a device was upgraded to the specified driver.
 Otherwise, it returns FALSE and the logged error can be retrieved with a call to GetLastError.
