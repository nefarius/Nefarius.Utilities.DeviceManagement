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
}