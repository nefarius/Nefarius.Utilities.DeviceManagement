# DriverStore

Namespace: Nefarius.Utilities.DeviceManagement.Drivers

Driver Store enumeration and manipulation utility.

```csharp
public static class DriverStore
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [DriverStore](./nefarius.utilities.devicemanagement.drivers.driverstore.md)

## Properties

### <a id="properties-existingdrivers"/>**ExistingDrivers**

Gets a list of existing packages (absolute INF paths) in the local driver store.

```csharp
public static IEnumerable<String> ExistingDrivers { get; }
```

#### Property Value

[IEnumerable&lt;String&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br>

## Methods

### <a id="methods-removedriver"/>**RemoveDriver(String)**

Removes a driver identified by absolute package path.

```csharp
public static void RemoveDriver(string driverStoreFileName)
```

#### Parameters

`driverStoreFileName` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
The absolute package path to remove.
