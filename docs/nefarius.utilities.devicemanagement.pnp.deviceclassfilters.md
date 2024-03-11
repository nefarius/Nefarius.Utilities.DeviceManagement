# DeviceClassFilters

Namespace: Nefarius.Utilities.DeviceManagement.PnP

Utility class to adjust class filter settings.

```csharp
public sealed class DeviceClassFilters
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [DeviceClassFilters](./nefarius.utilities.devicemanagement.pnp.deviceclassfilters.md)

## Methods

### <a id="methods-addlower"/>**AddLower(Guid, String)**

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

### <a id="methods-addupper"/>**AddUpper(Guid, String)**

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

### <a id="methods-deletelower"/>**DeleteLower(Guid)**

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

### <a id="methods-deleteupper"/>**DeleteUpper(Guid)**

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

### <a id="methods-getlower"/>**GetLower(Guid)**

Returns the list of device class lower filter services configured, or null of the value doesn't exist at all.

```csharp
public static IEnumerable<String> GetLower(Guid classGuid)
```

#### Parameters

`classGuid` [Guid](https://docs.microsoft.com/en-us/dotnet/api/system.guid)<br>
The device class GUID to query.

#### Returns

A list of service names or null.

#### Exceptions

[Win32Exception](./nefarius.utilities.devicemanagement.exceptions.win32exception.md)<br>

### <a id="methods-getupper"/>**GetUpper(Guid)**

Returns the list of device class upper filter services configured, or null of the value doesn't exist at all.

```csharp
public static IEnumerable<String> GetUpper(Guid classGuid)
```

#### Parameters

`classGuid` [Guid](https://docs.microsoft.com/en-us/dotnet/api/system.guid)<br>
The device class GUID to query.

#### Returns

A list of service names or null.

#### Exceptions

[Win32Exception](./nefarius.utilities.devicemanagement.exceptions.win32exception.md)<br>

### <a id="methods-removelower"/>**RemoveLower(Guid, String)**

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

### <a id="methods-removeupper"/>**RemoveUpper(Guid, String)**

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
