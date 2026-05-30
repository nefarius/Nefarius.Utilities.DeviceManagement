# DriverStore

Namespace: Nefarius.Utilities.DeviceManagement.Drivers

Driver Store enumeration and manipulation utility.

```csharp
public static class DriverStore
```

Inheritance [Object](https://learn.microsoft.com/dotnet/api/system.object) → [DriverStore](./nefarius.utilities.devicemanagement.drivers.driverstore.md)<br>
Attributes [NullableContextAttribute](./system.runtime.compilerservices.nullablecontextattribute.md), [NullableAttribute](./system.runtime.compilerservices.nullableattribute.md)

## Properties

### <a id="properties-existingdrivers"/>**ExistingDrivers**

Gets a list of existing packages (absolute INF paths) in the local driver store.

```csharp
public static IEnumerable<String> ExistingDrivers { get; }
```

#### Property Value

[IEnumerable](https://learn.microsoft.com/dotnet/api/system.collections.generic.ienumerable-1)<[String](https://learn.microsoft.com/dotnet/api/system.string)><br>

## Methods

### <a id="methods-removedriver"/>**RemoveDriver(String)**

Removes a driver identified by absolute package path.

```csharp
public static void RemoveDriver(string driverStoreFileName)
```

#### Parameters

`driverStoreFileName` [String](https://learn.microsoft.com/dotnet/api/system.string)<br>
The absolute package path to remove.
