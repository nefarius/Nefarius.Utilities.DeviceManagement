using Nefarius.Utilities.DeviceManagement.Drivers;
using Nefarius.Utilities.DeviceManagement.Extensions;
using Nefarius.Utilities.DeviceManagement.PnP;

namespace Tests;
#pragma warning disable CS1591

public class PnPDeviceTests
{
    [SetUp]
    public void Setup()
    {
    }
    
    /// <summary>
    ///     Requires one physical (or virtual) DualSense controller.
    /// </summary>
    [Test]
    public void TestPnPDeviceInstallCustomDriver()
    {
        const string instanceId = @"USB\VID_054C&PID_0CE6&MI_03\9&DC32669&3&0003";
        
        PnPDevice device = PnPDevice.GetDeviceByInstanceId(instanceId);
        
        device.InstallCustomDriver("winusb.inf");
    }

    /// <summary>
    ///     Requires one emulated X360 controller.
    /// </summary>
    [Test]
    public void TestPnPDeviceInstallNullDriver()
    {
        Assert.That(Devcon.FindByInterfaceGuid(DeviceInterfaceIds.XUsbDevice, out string? path, out string? instanceId), Is.True);

        var device = PnPDevice.GetDeviceByInstanceId(instanceId);
        
        device.InstallNullDriver();
    }
    
    [Test]
    public void TestGetDriverMeta()
    {
        const string path = @"\\?\usb#vid_08bb&pid_29c0#6&35844985&0&4#{a5dcbf10-6530-11d2-901f-00c04fb951ed}";

        PnPDevice? device = PnPDevice.GetDeviceByInterfaceId(path);

        DriverMeta? meta = device.GetCurrentDriver();
    }

    [Test]
    public void TestGetInstanceIdFromInterfaceId()
    {
        // Requires one Xbox controller, either 360 or One or compatible
        Assert.Multiple(() =>
        {
            // 1st controller
            Assert.That(Devcon.FindByInterfaceGuid(DeviceInterfaceIds.XUsbDevice, out string? path, out string? instanceId), Is.True);

            // compare IDs
            Assert.That(PnPDevice.GetInstanceIdFromInterfaceId(path), Is.EqualTo(instanceId).IgnoreCase);
        });
    }

    /// <summary>
    ///     Requires one emulated X360 controller.
    /// </summary>
    [Test]
    public void TestPnPDeviceIsVirtual()
    {
        string hardwareId = "USB\\VID_045E&PID_028E";

        Assert.That(Devcon.FindInDeviceClassByHardwareId(DeviceClassIds.XnaComposite, hardwareId,
            out IEnumerable<string>? instances, true), Is.True);

        List<string> list = instances.ToList();

        Assert.That(list, Is.Not.Empty);

        PnPDevice? device = list.Select(e => PnPDevice.GetDeviceByInstanceId(e)).FirstOrDefault(dev => dev.IsVirtual());

        Assert.That(device, Is.Not.Null);

        Assert.That(device!.IsVirtual(), Is.True);
    }
}