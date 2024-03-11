# PnPDeviceExtensions

Namespace: Nefarius.Utilities.DeviceManagement.Extensions

Helper methods for [PnPDevice](./nefarius.utilities.devicemanagement.pnp.pnpdevice.md) objects.

```csharp
public static class PnPDeviceExtensions
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [PnPDeviceExtensions](./nefarius.utilities.devicemanagement.extensions.pnpdeviceextensions.md)

## Methods

### <a id="methods-getcurrentdriver"/>**GetCurrentDriver(PnPDevice)**

Fetches meta data about the currently active driver of the [PnPDevice](./nefarius.utilities.devicemanagement.pnp.pnpdevice.md).

```csharp
public static DriverMeta GetCurrentDriver(PnPDevice device)
```

#### Parameters

`device` [PnPDevice](./nefarius.utilities.devicemanagement.pnp.pnpdevice.md)<br>
The [PnPDevice](./nefarius.utilities.devicemanagement.pnp.pnpdevice.md) to fetch driver info for.

#### Returns

The [DriverMeta](./nefarius.utilities.devicemanagement.drivers.drivermeta.md) instance.

#### Exceptions

[InvalidOperationException](https://docs.microsoft.com/en-us/dotnet/api/system.invalidoperationexception)<br>
Thrown if registry access failed.

### <a id="methods-tousbpnpdevice"/>**ToUsbPnPDevice(PnPDevice)**

Creates a [UsbPnPDevice](./nefarius.utilities.devicemanagement.pnp.usbpnpdevice.md) from the provided [PnPDevice](./nefarius.utilities.devicemanagement.pnp.pnpdevice.md).

```csharp
public static UsbPnPDevice ToUsbPnPDevice(PnPDevice device)
```

#### Parameters

`device` [PnPDevice](./nefarius.utilities.devicemanagement.pnp.pnpdevice.md)<br>
The [PnPDevice](./nefarius.utilities.devicemanagement.pnp.pnpdevice.md) to base this USB device on.

#### Returns

The new [UsbPnPDevice](./nefarius.utilities.devicemanagement.pnp.usbpnpdevice.md).
