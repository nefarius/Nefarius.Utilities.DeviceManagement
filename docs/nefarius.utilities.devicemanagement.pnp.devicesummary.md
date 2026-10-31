# DeviceSummary

Namespace: Nefarius.Utilities.DeviceManagement.PnP

A lightweight, read-only snapshot of a device node as returned by [Devcon.EnumerateDevices(Boolean)](./nefarius.utilities.devicemanagement.pnp.devcon.md#enumeratedevicesboolean).

```csharp
public record DeviceSummary
```

Inheritance [Object](https://learn.microsoft.com/dotnet/api/system.object) → [DeviceSummary](./nefarius.utilities.devicemanagement.pnp.devicesummary.md)<br>
Attributes [NullableContextAttribute](./system.runtime.compilerservices.nullablecontextattribute.md), [NullableAttribute](./system.runtime.compilerservices.nullableattribute.md)

## Properties

### <a id="properties-classguid"/>**ClassGuid**

The device setup class GUID, or [Guid.Empty](https://learn.microsoft.com/dotnet/api/system.guid.empty) if unavailable.

```csharp
public Guid ClassGuid { get; set; }
```

#### Property Value

[Guid](https://learn.microsoft.com/dotnet/api/system.guid)<br>

### <a id="properties-classname"/>**ClassName**

The device setup class name, or `null` if unavailable.

```csharp
public string ClassName { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/dotnet/api/system.string)<br>

### <a id="properties-description"/>**Description**

The friendly name if available, otherwise the device description, otherwise `null`.

```csharp
public string Description { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/dotnet/api/system.string)<br>

### <a id="properties-driverinfpath"/>**DriverInfPath**

The (published) INF name of the installed driver (e.g. `oem12.inf`), or `null` if no driver is
 installed.

```csharp
public string DriverInfPath { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/dotnet/api/system.string)<br>

### <a id="properties-hardwareids"/>**HardwareIds**

The hardware IDs, empty if the device has none.

```csharp
public IReadOnlyList<String> HardwareIds { get; set; }
```

#### Property Value

[IReadOnlyList](https://learn.microsoft.com/dotnet/api/system.collections.generic.ireadonlylist-1)<[String](https://learn.microsoft.com/dotnet/api/system.string)><br>

### <a id="properties-instanceid"/>**InstanceId**

The device instance ID (upper-invariant).

```csharp
public string InstanceId { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/dotnet/api/system.string)<br>

### <a id="properties-ispresent"/>**IsPresent**

True if the device is currently present, false for phantom (ghost) devices.

```csharp
public bool IsPresent { get; set; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/dotnet/api/system.boolean)<br>

### <a id="properties-service"/>**Service**

The name of the function driver service, or `null` if none is bound.

```csharp
public string Service { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/dotnet/api/system.string)<br>

## Constructors

### <a id="constructors-.ctor"/>**DeviceSummary(String, String, Guid, String, IReadOnlyList&lt;String&gt;, String, String, Boolean)**

A lightweight, read-only snapshot of a device node as returned by [Devcon.EnumerateDevices(Boolean)](./nefarius.utilities.devicemanagement.pnp.devcon.md#enumeratedevicesboolean).

```csharp
public DeviceSummary(string InstanceId, string Description, Guid ClassGuid, string ClassName, IReadOnlyList<String> HardwareIds, string Service, string DriverInfPath, bool IsPresent)
```

#### Parameters

`InstanceId` [String](https://learn.microsoft.com/dotnet/api/system.string)<br>
The device instance ID (upper-invariant).

`Description` [String](https://learn.microsoft.com/dotnet/api/system.string)<br>
The friendly name if available, otherwise the device description, otherwise `null`.

`ClassGuid` [Guid](https://learn.microsoft.com/dotnet/api/system.guid)<br>
The device setup class GUID, or [Guid.Empty](https://learn.microsoft.com/dotnet/api/system.guid.empty) if unavailable.

`ClassName` [String](https://learn.microsoft.com/dotnet/api/system.string)<br>
The device setup class name, or `null` if unavailable.

`HardwareIds` [IReadOnlyList](https://learn.microsoft.com/dotnet/api/system.collections.generic.ireadonlylist-1)<[String](https://learn.microsoft.com/dotnet/api/system.string)><br>
The hardware IDs, empty if the device has none.

`Service` [String](https://learn.microsoft.com/dotnet/api/system.string)<br>
The name of the function driver service, or `null` if none is bound.

`DriverInfPath` [String](https://learn.microsoft.com/dotnet/api/system.string)<br>
The (published) INF name of the installed driver (e.g. `oem12.inf`), or `null` if no driver is
 installed.

`IsPresent` [Boolean](https://learn.microsoft.com/dotnet/api/system.boolean)<br>
True if the device is currently present, false for phantom (ghost) devices.

## Methods

### <a id="methods-equals"/>**Equals(DeviceSummary)**

```csharp
public bool Equals(DeviceSummary other)
```

#### Parameters

`other` [DeviceSummary](./nefarius.utilities.devicemanagement.pnp.devicesummary.md)<br>

#### Returns

[Boolean](https://learn.microsoft.com/dotnet/api/system.boolean)
