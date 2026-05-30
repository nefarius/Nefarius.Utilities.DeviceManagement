# DeviceEventArgs

Namespace: Nefarius.Utilities.DeviceManagement.PnP

Device change event arguments.

```csharp
public class DeviceEventArgs
```

Inheritance [Object](https://learn.microsoft.com/dotnet/api/system.object) → [DeviceEventArgs](./nefarius.utilities.devicemanagement.pnp.deviceeventargs.md)<br>
Attributes [NullableContextAttribute](./system.runtime.compilerservices.nullablecontextattribute.md), [NullableAttribute](./system.runtime.compilerservices.nullableattribute.md)

## Properties

### <a id="properties-interfaceguid"/>**InterfaceGuid**

The [Guid](https://learn.microsoft.com/dotnet/api/system.guid) of the device interface.

```csharp
public Guid InterfaceGuid { get; set; }
```

#### Property Value

[Guid](https://learn.microsoft.com/dotnet/api/system.guid)<br>

### <a id="properties-symlink"/>**SymLink**

The symbolic link path.

```csharp
public string SymLink { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/dotnet/api/system.string)<br>

## Constructors

### <a id="constructors-.ctor"/>**DeviceEventArgs()**

```csharp
public DeviceEventArgs()
```
