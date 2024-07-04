// See https://aka.ms/new-console-template for more information

using Nefarius.Utilities.DeviceManagement.Drivers;
using Nefarius.Utilities.DeviceManagement.PnP;

Guid DeviceInterfaceGuid = Guid.Parse("{399ED672-E0BD-4FB3-AB0C-4955B56FB86A}");
bool rebootRequired = false;
int instance = 0;
// uninstall live copies of drivers in use by connected or orphaned devices
while (Devcon.FindByInterfaceGuid(DeviceInterfaceGuid, out PnPDevice dev, instance++, false))
{
    try
    {
        dev.Uninstall(out bool reboot);

        if (reboot)
        {
            rebootRequired = true;
        }
    }
    catch (Exception ex)
    {
        
    }
}

DriverStore.RemoveDriver(@"C:\temp\nonexistent");

const string instanceId = @"USB\VID_054C&PID_0CE6&MI_03\9&DC32669&3&0003";
        
PnPDevice device = PnPDevice.GetDeviceByInstanceId(instanceId);

// swap driver to WinUSB

device.InstallNullDriver();

Thread.Sleep(1000);

device.InstallCustomDriver("winusb.inf");

Thread.Sleep(1000);

// revert to original driver

device.Uninstall();

Thread.Sleep(5000);

Devcon.Refresh();
