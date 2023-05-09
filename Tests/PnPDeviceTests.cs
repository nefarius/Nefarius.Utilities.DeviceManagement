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

    [Test]
    public void TestGetDriverMeta()
    {
        string path = @"\\?\usb#vid_08bb&pid_29c0#6&35844985&0&4#{a5dcbf10-6530-11d2-901f-00c04fb951ed}";

        PnPDevice? device = PnPDevice.GetDeviceByInterfaceId(path);

        DriverMeta? meta = device.GetCurrentDriver();
    }

    [Test]
    public void TestGetInstanceIdFromInterfaceId()
    {
        // Requires one Xbox controller, either 360 or One or compatible
        Guid xusbInterfaceGuid = Guid.Parse("{EC87F1E3-C13B-4100-B5F7-8B84D54260CB}");

        // 1st controller
        Assert.True(Devcon.FindByInterfaceGuid(xusbInterfaceGuid, out string? path, out string? instanceId));

        // compare IDs
        Assert.That(PnPDevice.GetInstanceIdFromInterfaceId(path), Is.EqualTo(instanceId).IgnoreCase);
    }

    /// <summary>
    ///     Requires one emulated X360 controller.
    /// </summary>
    [Test]
    public void TestPnPDeviceIsVirtual()
    {
        Guid xnaCompositeClass = Guid.Parse("{d61ca365-5af4-4486-998b-9db4734c6ca3}");
        string hardwareId = "USB\\VID_045E&PID_028E";

        Assert.True(Devcon.FindInDeviceClassByHardwareId(xnaCompositeClass, hardwareId,
            out IEnumerable<string>? instances, true));

        List<string> list = instances.ToList();

        Assert.That(list.Count, Is.EqualTo(1));

        PnPDevice? device = PnPDevice.GetDeviceByInstanceId(list.First());

        Assert.That(device, Is.Not.Null);

        Assert.That(device.IsVirtual, Is.True);
    }
}