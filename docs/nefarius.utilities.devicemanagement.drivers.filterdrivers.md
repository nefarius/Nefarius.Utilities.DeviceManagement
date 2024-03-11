# FilterDrivers

Namespace: Nefarius.Utilities.DeviceManagement.Drivers

Utility class to simplify interaction with filter driver entries.

```csharp
public static class FilterDrivers
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [FilterDrivers](./nefarius.utilities.devicemanagement.drivers.filterdrivers.md)

## Methods

### <a id="methods-adddeviceclasslowerfilter"/>**AddDeviceClassLowerFilter(Guid, String)**

Adds a driver service to the lower filters of a provided class GUID.

```csharp
public static void AddDeviceClassLowerFilter(Guid classGuid, string serviceName)
```

#### Parameters

`classGuid` [Guid](https://docs.microsoft.com/en-us/dotnet/api/system.guid)<br>
The device class GUID.

`serviceName` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
The driver service name to add.

### <a id="methods-adddeviceclassupperfilter"/>**AddDeviceClassUpperFilter(Guid, String)**

Adds a driver service to the upper filters of a provided class GUID.

```csharp
public static void AddDeviceClassUpperFilter(Guid classGuid, string serviceName)
```

#### Parameters

`classGuid` [Guid](https://docs.microsoft.com/en-us/dotnet/api/system.guid)<br>
The device class GUID.

`serviceName` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
The driver service name to add.

### <a id="methods-getdeviceclasslowerfilters"/>**GetDeviceClassLowerFilters(Guid)**

Gets the lower filter service names (if any) for a provided class GUID.

```csharp
public static IEnumerable<String> GetDeviceClassLowerFilters(Guid classGuid)
```

#### Parameters

`classGuid` [Guid](https://docs.microsoft.com/en-us/dotnet/api/system.guid)<br>
The device class GUID.

#### Returns

A list of filter service names.

### <a id="methods-getdeviceclassupperfilters"/>**GetDeviceClassUpperFilters(Guid)**

Gets the upper filter service names (if any) for a provided class GUID.

```csharp
public static IEnumerable<String> GetDeviceClassUpperFilters(Guid classGuid)
```

#### Parameters

`classGuid` [Guid](https://docs.microsoft.com/en-us/dotnet/api/system.guid)<br>
The device class GUID.

#### Returns

A list of filter service names.

### <a id="methods-removedeviceclasslowerfilter"/>**RemoveDeviceClassLowerFilter(Guid, String)**

Removes a driver service from the lower filters of a provided class GUID.

```csharp
public static void RemoveDeviceClassLowerFilter(Guid classGuid, string serviceName)
```

#### Parameters

`classGuid` [Guid](https://docs.microsoft.com/en-us/dotnet/api/system.guid)<br>
The device class GUID.

`serviceName` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
The driver service name to remove.

### <a id="methods-removedeviceclassupperfilter"/>**RemoveDeviceClassUpperFilter(Guid, String)**

Removes a driver service from the upper filters of a provided class GUID.

```csharp
public static void RemoveDeviceClassUpperFilter(Guid classGuid, string serviceName)
```

#### Parameters

`classGuid` [Guid](https://docs.microsoft.com/en-us/dotnet/api/system.guid)<br>
The device class GUID.

`serviceName` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
The driver service name to remove.
