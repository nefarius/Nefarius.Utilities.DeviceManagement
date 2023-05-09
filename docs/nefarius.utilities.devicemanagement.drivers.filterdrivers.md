# FilterDrivers

Namespace: Nefarius.Utilities.DeviceManagement.Drivers

Utility class to simplify interaction with filter driver entries.

```csharp
public static class FilterDrivers
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [FilterDrivers](./nefarius.utilities.devicemanagement.drivers.filterdrivers.md)

## Methods

### **GetDeviceClassUpperFilters(Guid)**

Gets the upper filter service names (if any) for a provided class GUID.

```csharp
public static IEnumerable<string> GetDeviceClassUpperFilters(Guid classGuid)
```

#### Parameters

`classGuid` [Guid](https://docs.microsoft.com/en-us/dotnet/api/system.guid)<br>
The device class GUID.

#### Returns

[IEnumerable&lt;String&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br>
A list of filter service names.

### **GetDeviceClassLowerFilters(Guid)**

Gets the lower filter service names (if any) for a provided class GUID.

```csharp
public static IEnumerable<string> GetDeviceClassLowerFilters(Guid classGuid)
```

#### Parameters

`classGuid` [Guid](https://docs.microsoft.com/en-us/dotnet/api/system.guid)<br>
The device class GUID.

#### Returns

[IEnumerable&lt;String&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br>
A list of filter service names.

### **RemoveDeviceClassUpperFilter(Guid, String)**

Removes a driver service from the upper filters of a provided class GUID.

```csharp
public static void RemoveDeviceClassUpperFilter(Guid classGuid, string serviceName)
```

#### Parameters

`classGuid` [Guid](https://docs.microsoft.com/en-us/dotnet/api/system.guid)<br>
The device class GUID.

`serviceName` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
The driver service name to remove.

### **RemoveDeviceClassLowerFilter(Guid, String)**

Removes a driver service from the lower filters of a provided class GUID.

```csharp
public static void RemoveDeviceClassLowerFilter(Guid classGuid, string serviceName)
```

#### Parameters

`classGuid` [Guid](https://docs.microsoft.com/en-us/dotnet/api/system.guid)<br>
The device class GUID.

`serviceName` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
The driver service name to remove.

### **AddDeviceClassUpperFilter(Guid, String)**

Adds a driver service to the upper filters of a provided class GUID.

```csharp
public static void AddDeviceClassUpperFilter(Guid classGuid, string serviceName)
```

#### Parameters

`classGuid` [Guid](https://docs.microsoft.com/en-us/dotnet/api/system.guid)<br>
The device class GUID.

`serviceName` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
The driver service name to add.

### **AddDeviceClassLowerFilter(Guid, String)**

Adds a driver service to the lower filters of a provided class GUID.

```csharp
public static void AddDeviceClassLowerFilter(Guid classGuid, string serviceName)
```

#### Parameters

`classGuid` [Guid](https://docs.microsoft.com/en-us/dotnet/api/system.guid)<br>
The device class GUID.

`serviceName` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
The driver service name to add.
