# Nefarius.Utilities.DeviceManagement

Managed wrappers around SetupAPI, Cfgmgr32, NewDev and DrvStore native APIs on Windows.

## Examples

Some usage examples of the core library features are presented below.

### Enumerate all USB devices

The `Devcon` utility class offers helper methods to find devices.

```csharp
var instance = 0;
// enumerate all devices that export the GUID_DEVINTERFACE_USB_DEVICE interface
while (Devcon.FindByInterfaceGuid(DeviceInterfaceIds.UsbDevice, out var path,
           out var instanceId, instance++))
{
    Console.WriteLine($"Path: {path}, InstanceId: {instanceId}");

    var usbDevice = PnPDevice
        .GetDeviceByInterfaceId(path)
        .ToUsbPnPDevice();

    Console.WriteLine($"Got USB device {usbDevice.InstanceId}");
}
```

### Listen for new and removed USB devices

One or more instances of the `DeviceNotificationListener` can be used to listen for plugin and unplug events of various devices. This class has no dependency on WinForms or WPF and works in Console Applications and Windows Services alike.

```csharp
var listener = new DeviceNotificationListener();

listener.DeviceArrived += Console.WriteLine;
listener.DeviceRemoved += Console.WriteLine;

// start listening for plugins or unplugs of GUID_DEVINTERFACE_USB_DEVICE interface devices
listener.StartListen(DeviceInterfaceIds.UsbDevice);
```

### Get all driver packages in the Driver Store

```csharp
var allDriverPackages = DriverStore.ExistingDrivers.ToList();
```

### Remove all copies of `mydriver.inf` from the Driver Store

```csharp
foreach (var driverPackage in allDriverPackages.Where(p => p.Contains("mydriver.inf", StringComparison.OrdinalIgnoreCase)))
{
    DriverStore.RemoveDriver(driverPackage);
}
```
