# UsbPnPDevice

Namespace: Nefarius.Utilities.DeviceManagement.PnP

Describes an instance of a USB PNP device.

```csharp
public class UsbPnPDevice : PnPDevice
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [PnPDevice](./nefarius.utilities.devicemanagement.pnp.pnpdevice.md) → [UsbPnPDevice](./nefarius.utilities.devicemanagement.pnp.usbpnpdevice.md)

## Properties

### **Port**

The port number/index of this device on its root hub.

```csharp
public uint Port { get; }
```

#### Property Value

[UInt32](https://docs.microsoft.com/en-us/dotnet/api/system.uint32)<br>

### **InstanceId**

The instance ID of the device.

```csharp
public string InstanceId { get; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **DeviceId**

The device ID.

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
