<img src="assets/NSS-128x128.png" align="right" />

# Nefarius.Utilities.DeviceManagement

[![Build status](https://ci.appveyor.com/api/projects/status/x6ylnh2c6p3l12pw?svg=true)](https://ci.appveyor.com/project/nefarius/nefarius-utilities-devicemanagement) ![Requirements](https://img.shields.io/badge/Requires-.NET%20Standard%202.0-blue.svg) [![Nuget](https://img.shields.io/nuget/v/Nefarius.Utilities.DeviceManagement)](https://www.nuget.org/packages/Nefarius.Utilities.DeviceManagement/) [![Nuget](https://img.shields.io/nuget/dt/Nefarius.Utilities.DeviceManagement)](https://www.nuget.org/packages/Nefarius.Utilities.DeviceManagement/)

Managed wrappers around SetupAPI, Cfgmgr32, NewDev and DrvStore native APIs on Windows.

## Features

- Listen for device plugin and unplug events **without depending on WinForms or WPF**
- Enumerate devices (present and absent)
- Get and set
  various [unified device properties](https://learn.microsoft.com/en-us/windows-hardware/drivers/install/unified-device-property-model--windows-vista-and-later-)
- Convert various notations (Symlink to Instance ID etc.)
- Enumerate and remove elements form the Driver Store

## Documentation

[Link to API docs](docs/index.md).

### Generating documentation

- `dotnet build -c:Release`
- `dotnet tool install -g XMLDoc2Markdown`
- `xmldoc2md .\bin\netstandard2.0\Nefarius.Utilities.DeviceManagement.dll .\docs\`

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

One or more instances of the `DeviceNotificationListener` can be used to listen for plugin and unplug events of various
devices. This class has no dependency on WinForms or WPF and works in Console Applications and Windows Services alike.

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

### Get Instance ID from a symbolic link (device path)

```csharp
// e.g. the path "\\?\HID#VID_045E&PID_028E&IG_00#3&31f0e99d&0&0000#{4d1e55b2-f16f-11cf-88cb-001111000030}"
// gets translated to instanceId "HID\VID_045E&PID_028E&IG_00\3&31f0e99d&0&0000"
var instanceId = PnPDevice.GetInstanceIdFromInterfaceId(path);
```

### Get PnPDevice object from symbolic link

```csharp
// example path: "\\?\HID#VID_046D&PID_C33F&MI_01&COL02#B&31580538&0&0001#{4D1E55B2-F16F-11CF-88CB-001111000030}"
PnPDevice device = PnPDevice.GetDeviceByInterfaceId(path);
```

## Sources & 3rd party credits

- [ManagedDevcon](https://github.com/nefarius/ManagedDevcon)
- [ViGEm.Management](https://github.com/ViGEm/ViGEm.Management)
- [Driver Store Explorer [RAPR]](https://github.com/lostindark/DriverStoreExplorer)
- [Driver Updater](https://github.com/WOA-Project/DriverUpdater)
- [XMLDoc2Markdown](https://charlesdevandiere.github.io/xmldoc2md/)
- [C#/Win32 P/Invoke Source Generator](https://github.com/microsoft/CsWin32)
- [Tooling to generate metadata for Win32 APIs in the Windows SDK.](https://github.com/microsoft/win32metadata)
- [Disable or Enable Device with Hardware ID](https://gist.github.com/3735943886/f47c355738e3dd7975fe0aa1abd67445)
- [Manage Windows devices in .NET, can enumerate, start, stop, disable, enable, and remove devices](https://gist.github.com/jborean93/01ba060ac9043a7b997d396de7aa009e)
