# UsbPnPDevice

Namespace: Nefarius.Utilities.DeviceManagement.PnP

Describes an instance of a USB PNP device.

```csharp
public class UsbPnPDevice : PnPDevice, IPnPDevice, System.IEquatable`1[[Nefarius.Utilities.DeviceManagement.PnP.PnPDevice, Nefarius.Utilities.DeviceManagement, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [PnPDevice](./nefarius.utilities.devicemanagement.pnp.pnpdevice.md) → [UsbPnPDevice](./nefarius.utilities.devicemanagement.pnp.usbpnpdevice.md)<br>
Implements [IPnPDevice](./nefarius.utilities.devicemanagement.pnp.ipnpdevice.md), [IEquatable&lt;PnPDevice&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.iequatable-1)

## Properties

### <a id="properties-children"/>**Children**

Children of this [IPnPDevice](./nefarius.utilities.devicemanagement.pnp.ipnpdevice.md), if any.

```csharp
public IEnumerable<IPnPDevice> Children { get; }
```

#### Property Value

[IEnumerable&lt;IPnPDevice&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br>

### <a id="properties-compatibleids"/>**CompatibleIds**

List of compatible IDs, if any.

```csharp
public IEnumerable<String> CompatibleIds { get; }
```

#### Property Value

[IEnumerable&lt;String&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br>

### <a id="properties-deviceid"/>**DeviceId**

The device ID. Typically built from the hardware ID of the same make and model of hardware.

```csharp
public string DeviceId { get; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### <a id="properties-hardwareids"/>**HardwareIds**

List of hardware IDs, if any.

```csharp
public IEnumerable<String> HardwareIds { get; }
```

#### Property Value

[IEnumerable&lt;String&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br>

### <a id="properties-instanceid"/>**InstanceId**

The instance ID of the device. Uniquely identifies devices of equal make and model on the same machine.

```csharp
public string InstanceId { get; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### <a id="properties-parent"/>**Parent**

The parent of this [IPnPDevice](./nefarius.utilities.devicemanagement.pnp.ipnpdevice.md), if any.

```csharp
public IPnPDevice Parent { get; }
```

#### Property Value

[IPnPDevice](./nefarius.utilities.devicemanagement.pnp.ipnpdevice.md)<br>

### <a id="properties-port"/>**Port**

The port number/index of this device on its root hub.

```csharp
public uint Port { get; }
```

#### Property Value

[UInt32](https://docs.microsoft.com/en-us/dotnet/api/system.uint32)<br>

### <a id="properties-siblings"/>**Siblings**

Siblings of this [IPnPDevice](./nefarius.utilities.devicemanagement.pnp.ipnpdevice.md) sharing the same parent, if any.

```csharp
public IEnumerable<IPnPDevice> Siblings { get; }
```

#### Property Value

[IEnumerable&lt;IPnPDevice&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br>

## Methods

### <a id="methods-cycleport"/>**CyclePort()**

Power-cycles the hub port this device is attached to, causing it to restart.

```csharp
public void CyclePort()
```

**Remarks:**

Requires administrative privileges.
