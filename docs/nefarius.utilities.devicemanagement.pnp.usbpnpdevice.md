# UsbPnPDevice

Namespace: Nefarius.Utilities.DeviceManagement.PnP

Describes an instance of a USB PNP device.

```csharp
public class UsbPnPDevice : PnPDevice, IPnPDevice, System.IEquatable`1[[Nefarius.Utilities.DeviceManagement.PnP.PnPDevice, Nefarius.Utilities.DeviceManagement, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]]
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [PnPDevice](./nefarius.utilities.devicemanagement.pnp.pnpdevice.md) → [UsbPnPDevice](./nefarius.utilities.devicemanagement.pnp.usbpnpdevice.md)<br>
Implements [IPnPDevice](./nefarius.utilities.devicemanagement.pnp.ipnpdevice.md), [IEquatable&lt;PnPDevice&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.iequatable-1)

## Properties

### **Port**

The port number/index of this device on its root hub.

```csharp
public uint Port { get; }
```

#### Property Value

[UInt32](https://docs.microsoft.com/en-us/dotnet/api/system.uint32)<br>

### **InstanceId**

The instance ID of the device. Uniquely identifies devices of equal make and model on the same machine.

```csharp
public string InstanceId { get; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **DeviceId**

The device ID. Typically built from the hardware ID of the same make and model of hardware.

```csharp
public string DeviceId { get; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

## Methods

### **CyclePort()**

Power-cycles the hub port this device is attached to, causing it to restart.

```csharp
public void CyclePort()
```

**Remarks:**

Requires administrative privileges.
