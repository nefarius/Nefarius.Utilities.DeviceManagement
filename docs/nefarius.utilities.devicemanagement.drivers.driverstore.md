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

### <a id="methods-getpackages"/>**GetPackages(Boolean)**

Enumerates the packages staged in the local driver store (read-only query).

```csharp
public static IReadOnlyList<DriverStorePackage> GetPackages(bool includeInbox)
```

#### Parameters

`includeInbox` [Boolean](https://learn.microsoft.com/dotnet/api/system.boolean)<br>
False (default) to only return third-party packages, true to also include inbox packages shipped with
 Windows.

#### Returns

A snapshot of the staged packages, including the instance IDs of devices bound to each package by their
 installed driver INF name.

#### Exceptions

[Win32Exception](https://learn.microsoft.com/dotnet/api/system.componentmodel.win32exception)<br>
The driver store could not be enumerated.

**Remarks:**

Uses the undocumented `DriverStoreOfflineEnumDriverPackageW` export of `drvstore.dll` against the
 live Windows directory, then reads `Provider`, `DriverVer` and class information from each staged
 INF via SetupAPI. Packages whose INF cannot be opened are still returned with the unreadable fields unset.

### <a id="methods-removedriver"/>**RemoveDriver(String)**

Removes a driver identified by absolute package path.

```csharp
public static void RemoveDriver(string driverStoreFileName)
```

#### Parameters

`driverStoreFileName` [String](https://learn.microsoft.com/dotnet/api/system.string)<br>
The absolute package path to remove.
