# DriverMeta

Namespace: Nefarius.Utilities.DeviceManagement.Drivers

Driver meta data fetched from the registry.

```csharp
public sealed class DriverMeta
```

Inheritance [Object](https://learn.microsoft.com/dotnet/api/system.object) → [DriverMeta](./nefarius.utilities.devicemanagement.drivers.drivermeta.md)<br>
Attributes [NullableContextAttribute](./system.runtime.compilerservices.nullablecontextattribute.md), [NullableAttribute](./system.runtime.compilerservices.nullableattribute.md)

## Properties

### <a id="properties-driverdate"/>**DriverDate**

Gets the date of the driver.

```csharp
public DateTime DriverDate { get; internal set; }
```

#### Property Value

[DateTime](https://learn.microsoft.com/dotnet/api/system.datetime)<br>

### <a id="properties-driverdescription"/>**DriverDescription**

Gets the description the device got from the function driver.

```csharp
public string DriverDescription { get; internal set; }
```

#### Property Value

[String](https://learn.microsoft.com/dotnet/api/system.string)<br>

### <a id="properties-driverversion"/>**DriverVersion**

Gets the driver version.

```csharp
public Version DriverVersion { get; internal set; }
```

#### Property Value

[Version](https://learn.microsoft.com/dotnet/api/system.version)<br>

### <a id="properties-infpath"/>**InfPath**

Gets the active INF name/sub-path. Typically resides in C:\Windows\INF.

```csharp
public string InfPath { get; internal set; }
```

#### Property Value

[String](https://learn.microsoft.com/dotnet/api/system.string)<br>

### <a id="properties-infsection"/>**InfSection**

Gets the section of the INF which applied on driver installation.

```csharp
public string InfSection { get; internal set; }
```

#### Property Value

[String](https://learn.microsoft.com/dotnet/api/system.string)<br>

### <a id="properties-matchingdeviceid"/>**MatchingDeviceId**

Gets the device ID this driver is active on.

```csharp
public string MatchingDeviceId { get; internal set; }
```

#### Property Value

[String](https://learn.microsoft.com/dotnet/api/system.string)<br>

### <a id="properties-providername"/>**ProviderName**

Gets the provider (manufacturer) name of the driver.

```csharp
public string ProviderName { get; internal set; }
```

#### Property Value

[String](https://learn.microsoft.com/dotnet/api/system.string)<br>
