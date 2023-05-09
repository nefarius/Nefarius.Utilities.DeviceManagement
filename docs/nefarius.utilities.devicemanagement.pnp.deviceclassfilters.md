# DeviceClassFilters

Namespace: Nefarius.Utilities.DeviceManagement.PnP

Utility class to adjust class filter settings.

```csharp
public sealed class DeviceClassFilters
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [DeviceClassFilters](./nefarius.utilities.devicemanagement.pnp.deviceclassfilters.md)

## Methods

### **AddUpper(Guid, String)**

Adds an entry to the device class upper filters.

```csharp
public static void AddUpper(Guid classGuid, string service)
```

#### Parameters

`classGuid` [Guid](https://docs.microsoft.com/en-us/dotnet/api/system.guid)<br>
The device class GUID to modify.

`service` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
The driver service name to add.

#### Exceptions

[Win32Exception](./nefarius.utilities.devicemanagement.exceptions.win32exception.md)<br>

[DriverServiceNotFoundException](./nefarius.utilities.devicemanagement.exceptions.driverservicenotfoundexception.md)<br>

**Remarks:**

If the filters value doesn't exist, it will get created. If the provided service entry already exists, it will not
 get added again. Throws a [DriverServiceNotFoundException](./nefarius.utilities.devicemanagement.exceptions.driverservicenotfoundexception.md) if the provided service doesn't exist.

### **RemoveUpper(Guid, String)**

Removes an entry from the device class upper filters.

```csharp
public static void RemoveUpper(Guid classGuid, string service)
```

#### Parameters

`classGuid` [Guid](https://docs.microsoft.com/en-us/dotnet/api/system.guid)<br>
The device class GUID to modify.

`service` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
The driver service name to add.

#### Exceptions

[Win32Exception](./nefarius.utilities.devicemanagement.exceptions.win32exception.md)<br>

**Remarks:**

If the provided service entry doesn't exist or the entire filter value is not present, this method does
 nothing.

### **GetUpper(Guid)**

Returns the list of device class upper filter services configured, or null of the value doesn't exist at all.

```csharp
public static IEnumerable<string> GetUpper(Guid classGuid)
```

#### Parameters

`classGuid` [Guid](https://docs.microsoft.com/en-us/dotnet/api/system.guid)<br>
The device class GUID to query.

#### Returns

[IEnumerable&lt;String&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br>
A list of service names or null.

#### Exceptions

[Win32Exception](./nefarius.utilities.devicemanagement.exceptions.win32exception.md)<br>

### **DeleteUpper(Guid)**

Deletes the entire upper filters value for the provided device class.

```csharp
public static void DeleteUpper(Guid classGuid)
```

#### Parameters

`classGuid` [Guid](https://docs.microsoft.com/en-us/dotnet/api/system.guid)<br>
The device class GUID to delete the value for.

#### Exceptions

[Win32Exception](./nefarius.utilities.devicemanagement.exceptions.win32exception.md)<br>

**Remarks:**

If the value doesn't exist, this method does nothing.

### **AddLower(Guid, String)**

Adds an entry to the device class lower filters.

```csharp
public static void AddLower(Guid classGuid, string service)
```

#### Parameters

`classGuid` [Guid](https://docs.microsoft.com/en-us/dotnet/api/system.guid)<br>
The device class GUID to modify.

`service` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
The driver service name to add.

#### Exceptions

[Win32Exception](./nefarius.utilities.devicemanagement.exceptions.win32exception.md)<br>

[DriverServiceNotFoundException](./nefarius.utilities.devicemanagement.exceptions.driverservicenotfoundexception.md)<br>

**Remarks:**

If the filters value doesn't exist, it will get created. If the provided service entry already exists, it will not
 get added again. Throws a [DriverServiceNotFoundException](./nefarius.utilities.devicemanagement.exceptions.driverservicenotfoundexception.md) if the provided service doesn't exist.

### **RemoveLower(Guid, String)**

Removes an entry from the device class lower filters.

```csharp
public static void RemoveLower(Guid classGuid, string service)
```

#### Parameters

`classGuid` [Guid](https://docs.microsoft.com/en-us/dotnet/api/system.guid)<br>
The device class GUID to modify.

`service` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
The driver service name to add.

#### Exceptions

[Win32Exception](./nefarius.utilities.devicemanagement.exceptions.win32exception.md)<br>

**Remarks:**

If the provided service entry doesn't exist or the entire filter value is not present, this method does
 nothing.

### **GetLower(Guid)**

Returns the list of device class lower filter services configured, or null of the value doesn't exist at all.

```csharp
public static IEnumerable<string> GetLower(Guid classGuid)
```

#### Parameters

`classGuid` [Guid](https://docs.microsoft.com/en-us/dotnet/api/system.guid)<br>
The device class GUID to query.

#### Returns

[IEnumerable&lt;String&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br>
A list of service names or null.

#### Exceptions

[Win32Exception](./nefarius.utilities.devicemanagement.exceptions.win32exception.md)<br>

### **DeleteLower(Guid)**

Deletes the entire lower filters value for the provided device class.

```csharp
public static void DeleteLower(Guid classGuid)
```

#### Parameters

`classGuid` [Guid](https://docs.microsoft.com/en-us/dotnet/api/system.guid)<br>
The device class GUID to delete the value for.

#### Exceptions

[Win32Exception](./nefarius.utilities.devicemanagement.exceptions.win32exception.md)<br>

**Remarks:**

If the value doesn't exist, this method does nothing.
