using Nefarius.Utilities.DeviceManagement.PnP;

namespace Tests;

public class PnPDeviceTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void TestGetInstanceIdFromInterfaceId()
    {
        // Requires one Xbox controller, either 360 or One or compatible
        var xusbInterfaceGuid = Guid.Parse("{EC87F1E3-C13B-4100-B5F7-8B84D54260CB}");

        // 1st controller
        Assert.True(Devcon.FindByInterfaceGuid(xusbInterfaceGuid, out var path, out var instanceId));

        // compare IDs
        Assert.That(PnPDevice.GetInstanceIdFromInterfaceId(path), Is.EqualTo(instanceId).IgnoreCase);
    }

    /// <summary>
    ///     Requires one emulated X360 controller.
    /// </summary>
    [Test]
    public void TestPnPDeviceIsVirtual()
    {
        var xnaCompositeClass = Guid.Parse("{d61ca365-5af4-4486-998b-9db4734c6ca3}");
        var hardwareId = "USB\\VID_045E&PID_028E";

        Assert.True(Devcon.FindInDeviceClassByHardwareId(xnaCompositeClass, hardwareId, out var instances, true));
        
        var list = instances.ToList();

        Assert.That(list.Count, Is.EqualTo(1));

        var device = PnPDevice.GetDeviceByInstanceId(list.First());

        Assert.That(device, Is.Not.Null);

        Assert.That(device.IsVirtual, Is.True);
    }
}