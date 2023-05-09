# PnPDeviceExtensions

Namespace: Nefarius.Utilities.DeviceManagement.Extensions

Helper methods for [PnPDevice](./nefarius.utilities.devicemanagement.pnp.pnpdevice.md) objects.

```csharp
public static class PnPDeviceExtensions
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [PnPDeviceExtensions](./nefarius.utilities.devicemanagement.extensions.pnpdeviceextensions.md)

## Methods

### **ToUsbPnPDevice(PnPDevice)**

Creates a [UsbPnPDevice](./nefarius.utilities.devicemanagement.pnp.usbpnpdevice.md) from the provided [PnPDevice](./nefarius.utilities.devicemanagement.pnp.pnpdevice.md).

```csharp
public static UsbPnPDevice ToUsbPnPDevice(PnPDevice device)
```

#### Parameters

`device` [PnPDevice](./nefarius.utilities.devicemanagement.pnp.pnpdevice.md)<br>
The  to base this USB device on.

#### Returns

[UsbPnPDevice](./nefarius.utilities.devicemanagement.pnp.usbpnpdevice.md)<br>
The new .

### **GetCurrentDriver(PnPDevice)**

Fetches meta data about the currently active driver of the [PnPDevice](./nefarius.utilities.devicemanagement.pnp.pnpdevice.md).

```csharp
public static DriverMeta GetCurrentDriver(PnPDevice device)
```

#### Parameters

`device` [PnPDevice](./nefarius.utilities.devicemanagement.pnp.pnpdevice.md)<br>
The  to fetch driver info for.

#### Returns

[DriverMeta](./nefarius.utilities.devicemanagement.drivers.drivermeta.md)<br>
The  instance.

#### Exceptions

[InvalidOperationException](https://docs.microsoft.com/en-us/dotnet/api/system.invalidoperationexception)<br>
Thrown if registry access failed.
