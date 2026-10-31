# DriverStorePackage

Namespace: Nefarius.Utilities.DeviceManagement.Drivers

A read-only description of a driver package staged in the Windows driver store.

```csharp
public record DriverStorePackage
```

Inheritance [Object](https://learn.microsoft.com/dotnet/api/system.object) → [DriverStorePackage](./nefarius.utilities.devicemanagement.drivers.driverstorepackage.md)<br>
Attributes [NullableContextAttribute](./system.runtime.compilerservices.nullablecontextattribute.md), [NullableAttribute](./system.runtime.compilerservices.nullableattribute.md)

## Properties

### <a id="properties-architecture"/>**Architecture**

The target architecture of the package (e.g. `amd64`, `x86`, `arm64`, `neutral`).

```csharp
public string Architecture { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/dotnet/api/system.string)<br>

### <a id="properties-bounddeviceinstanceids"/>**BoundDeviceInstanceIds**

Instance IDs of devices (including phantom ones) whose installed driver INF matches this package.

```csharp
public IReadOnlyList<String> BoundDeviceInstanceIds { get; set; }
```

#### Property Value

[IReadOnlyList](https://learn.microsoft.com/dotnet/api/system.collections.generic.ireadonlylist-1)<[String](https://learn.microsoft.com/dotnet/api/system.string)><br>

### <a id="properties-classguid"/>**ClassGuid**

The device setup class GUID, or [Guid.Empty](https://learn.microsoft.com/dotnet/api/system.guid.empty) if unknown.

```csharp
public Guid ClassGuid { get; set; }
```

#### Property Value

[Guid](https://learn.microsoft.com/dotnet/api/system.guid)<br>

### <a id="properties-classname"/>**ClassName**

The device setup class name, if known.

```csharp
public string ClassName { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/dotnet/api/system.string)<br>

### <a id="properties-driverdate"/>**DriverDate**

The driver date from the INF `DriverVer` directive, if parsable.

```csharp
public Nullable<DateTime> DriverDate { get; set; }
```

#### Property Value

[Nullable](https://learn.microsoft.com/dotnet/api/system.nullable-1)<[DateTime](https://learn.microsoft.com/dotnet/api/system.datetime)><br>

### <a id="properties-driverversion"/>**DriverVersion**

The driver version from the INF `DriverVer` directive, if parsable.

```csharp
public Version DriverVersion { get; set; }
```

#### Property Value

[Version](https://learn.microsoft.com/dotnet/api/system.version)<br>

### <a id="properties-isinbox"/>**IsInbox**

True if the package ships with Windows (inbox), false for third-party packages.

```csharp
public bool IsInbox { get; set; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/dotnet/api/system.boolean)<br>

### <a id="properties-originalinfname"/>**OriginalInfName**

The original INF file name (e.g. `mydriver.inf`) derived from the package directory name, or
 `null` if it could not be determined.

```csharp
public string OriginalInfName { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/dotnet/api/system.string)<br>

### <a id="properties-provider"/>**Provider**

The `Provider` value of the INF `[Version]` section, if readable.

```csharp
public string Provider { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/dotnet/api/system.string)<br>

### <a id="properties-publishedname"/>**PublishedName**

The published INF name: `oemNN.inf` for third-party packages, the plain INF file name for inbox packages.
 May be empty if the driver store reports none.

```csharp
public string PublishedName { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/dotnet/api/system.string)<br>

### <a id="properties-storeinfpath"/>**StoreInfPath**

The absolute path of the staged INF inside the driver store file repository.

```csharp
public string StoreInfPath { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/dotnet/api/system.string)<br>

## Constructors

### <a id="constructors-.ctor"/>**DriverStorePackage(String, String, String, String, String, Guid, Version, Nullable&lt;DateTime&gt;, String, Boolean, IReadOnlyList&lt;String&gt;)**

A read-only description of a driver package staged in the Windows driver store.

```csharp
public DriverStorePackage(string PublishedName, string StoreInfPath, string OriginalInfName, string Provider, string ClassName, Guid ClassGuid, Version DriverVersion, Nullable<DateTime> DriverDate, string Architecture, bool IsInbox, IReadOnlyList<String> BoundDeviceInstanceIds)
```

#### Parameters

`PublishedName` [String](https://learn.microsoft.com/dotnet/api/system.string)<br>
The published INF name: `oemNN.inf` for third-party packages, the plain INF file name for inbox packages.
 May be empty if the driver store reports none.

`StoreInfPath` [String](https://learn.microsoft.com/dotnet/api/system.string)<br>
The absolute path of the staged INF inside the driver store file repository.

`OriginalInfName` [String](https://learn.microsoft.com/dotnet/api/system.string)<br>
The original INF file name (e.g. `mydriver.inf`) derived from the package directory name, or
 `null` if it could not be determined.

`Provider` [String](https://learn.microsoft.com/dotnet/api/system.string)<br>
The `Provider` value of the INF `[Version]` section, if readable.

`ClassName` [String](https://learn.microsoft.com/dotnet/api/system.string)<br>
The device setup class name, if known.

`ClassGuid` [Guid](https://learn.microsoft.com/dotnet/api/system.guid)<br>
The device setup class GUID, or [Guid.Empty](https://learn.microsoft.com/dotnet/api/system.guid.empty) if unknown.

`DriverVersion` [Version](https://learn.microsoft.com/dotnet/api/system.version)<br>
The driver version from the INF `DriverVer` directive, if parsable.

`DriverDate` [Nullable](https://learn.microsoft.com/dotnet/api/system.nullable-1)<[DateTime](https://learn.microsoft.com/dotnet/api/system.datetime)><br>
The driver date from the INF `DriverVer` directive, if parsable.

`Architecture` [String](https://learn.microsoft.com/dotnet/api/system.string)<br>
The target architecture of the package (e.g. `amd64`, `x86`, `arm64`, `neutral`).

`IsInbox` [Boolean](https://learn.microsoft.com/dotnet/api/system.boolean)<br>
True if the package ships with Windows (inbox), false for third-party packages.

`BoundDeviceInstanceIds` [IReadOnlyList](https://learn.microsoft.com/dotnet/api/system.collections.generic.ireadonlylist-1)<[String](https://learn.microsoft.com/dotnet/api/system.string)><br>
Instance IDs of devices (including phantom ones) whose installed driver INF matches this package.

## Methods

### <a id="methods-equals"/>**Equals(DriverStorePackage)**

```csharp
public bool Equals(DriverStorePackage other)
```

#### Parameters

`other` [DriverStorePackage](./nefarius.utilities.devicemanagement.drivers.driverstorepackage.md)<br>

#### Returns

[Boolean](https://learn.microsoft.com/dotnet/api/system.boolean)
