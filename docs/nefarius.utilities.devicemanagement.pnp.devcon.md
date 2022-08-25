# Devcon

Namespace: Nefarius.Utilities.DeviceManagement.PnP

Managed wrapper for SetupAPI.

```csharp
public static class Devcon
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [Devcon](./nefarius.utilities.devicemanagement.pnp.devcon.md)

## Methods

### **FindInDeviceClassByHardwareId(Guid, String)**

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

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
True if found, false otherwise.

### **FindInDeviceClassByHardwareId(Guid, String, IEnumerable`1&)**

Attempts to find a device within a specified device class by a given hardware ID.

```csharp
public static bool FindInDeviceClassByHardwareId(Guid target, string hardwareId, IEnumerable`1& instanceIds)
```

#### Parameters

`target` [Guid](https://docs.microsoft.com/en-us/dotnet/api/system.guid)<br>
The device class GUID.

`hardwareId` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
The hardware ID to search for.

`instanceIds` [IEnumerable`1&](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1&)<br>
A list of instances found for the given search criteria.

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
True if found, false otherwise.

### **FindByInterfaceGuid(Guid, String&, String&, Int32, Boolean)**

Searches for devices matching the provided interface GUID and returns the device path and instance ID.

```csharp
public static bool FindByInterfaceGuid(Guid target, String& path, String& instanceId, int instance, bool presentOnly)
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

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
True if at least one device was found with the provided class, false otherwise.

### **FindByInterfaceGuid(Guid, PnPDevice&, Int32, Boolean)**

Searches for devices matching the provided interface GUID and returns a [PnPDevice](./nefarius.utilities.devicemanagement.pnp.pnpdevice.md).

```csharp
public static bool FindByInterfaceGuid(Guid target, PnPDevice& device, int instance, bool presentOnly)
```

#### Parameters

`target` [Guid](https://docs.microsoft.com/en-us/dotnet/api/system.guid)<br>
The interface GUID to enumerate.

`device` [PnPDevice&](./nefarius.utilities.devicemanagement.pnp.pnpdevice&.md)<br>
The  wrapper object.

`instance` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
Optional instance ID (zero-based) specifying the device to process on multiple matches.

`presentOnly` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

                Only enumerate currently connected devices by default, set to False to also include phantom
                devices.

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
True if at least one device was found with the provided class, false otherwise.

### **Find(Guid, String&, String&, Int32)**

Searches for devices matching the provided interface GUID and returns the device path and instance ID.

```csharp
public static bool Find(Guid target, String& path, String& instanceId, int instance)
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

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
True if at least one device was found with the provided class, false otherwise.

### **Install(String, Boolean&)**

Invokes the installation of a driver via provided .INF file.

```csharp
public static bool Install(string fullInfPath, Boolean& rebootRequired)
```

#### Parameters

`fullInfPath` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
An absolute path to the .INF file to install.

`rebootRequired` [Boolean&](https://docs.microsoft.com/en-us/dotnet/api/system.boolean&)<br>
True if a machine reboot is required, false otherwise.

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
True on success, false otherwise.

### **Create(String, Guid, String)**

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

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
True on success, false otherwise.

### **Remove(Guid, String)**

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

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
True on success, false otherwise.

### **Remove(Guid, String, Boolean&)**

Removed a device node identified by interface GUID and instance ID.

```csharp
public static bool Remove(Guid classGuid, string instanceId, Boolean& rebootRequired)
```

#### Parameters

`classGuid` [Guid](https://docs.microsoft.com/en-us/dotnet/api/system.guid)<br>
The device class GUID.

`instanceId` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
The instance ID.

`rebootRequired` [Boolean&](https://docs.microsoft.com/en-us/dotnet/api/system.boolean&)<br>
True if a reboot is required to complete the uninstall action, false otherwise.

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
True on success, false otherwise.

### **Refresh()**

Instructs the system to re-enumerate hardware devices.

```csharp
public static bool Refresh()
```

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
True on success, false otherwise.

### **RefreshPhantom()**

Instructs the system to re-enumerate hardware devices including disconnected ones.

```csharp
public static bool RefreshPhantom()
```

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
True on success, false otherwise.

### **Update(String, String, Boolean&)**

Given an INF file and a hardware ID, this function installs updated drivers for devices that match the hardware ID.

```csharp
public static bool Update(string hardwareId, string fullInfPath, Boolean& rebootRequired)
```

#### Parameters

`hardwareId` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
A string that supplies the hardware identifier to match existing devices on the computer.

`fullInfPath` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
A string that supplies the full path file name of an INF file.

`rebootRequired` [Boolean&](https://docs.microsoft.com/en-us/dotnet/api/system.boolean&)<br>
A variable that indicates whether a restart is required and who should prompt for it.

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

                The function returns TRUE if a device was upgraded to the specified driver.
                Otherwise, it returns FALSE and the logged error can be retrieved with a call to GetLastError.

### **DeleteDriver(String, String, Boolean)**

Uninstalls a driver identified via a given INF and optionally removes it from the driver store as well.

```csharp
public static void DeleteDriver(string oemInfName, string fullInfPath, bool forceDelete)
```

#### Parameters

`oemInfName` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
The OEM INF name (name and extension only).

`fullInfPath` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
The fully qualified absolute path to the INF to remove from driver store.

`forceDelete` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
Remove driver store copy, if true.
