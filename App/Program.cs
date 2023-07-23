// See https://aka.ms/new-console-template for more information

using Nefarius.Utilities.DeviceManagement.PnP;

const string instanceId = @"USB\VID_054C&PID_0CE6&MI_03\9&DC32669&3&0003";
        
PnPDevice device = PnPDevice.GetDeviceByInstanceId(instanceId);
        
device.InstallCustomDriver("winusb.inf");