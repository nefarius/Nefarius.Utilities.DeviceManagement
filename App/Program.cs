// See https://aka.ms/new-console-template for more information

using Nefarius.Utilities.DeviceManagement.PnP;

const string instanceId = @"USB\VID_054C&PID_0CE6&MI_03\9&DC32669&3&0003";
        
PnPDevice device = PnPDevice.GetDeviceByInstanceId(instanceId);

// swap driver to WinUSB

device.InstallNullDriver();

Thread.Sleep(1000);

device.InstallCustomDriver("winusb.inf");

//Thread.Sleep(1000);

// revert to original driver

device.Uninstall();

//Thread.Sleep(1000);

//device.Remove();

//Devcon.Refresh();

