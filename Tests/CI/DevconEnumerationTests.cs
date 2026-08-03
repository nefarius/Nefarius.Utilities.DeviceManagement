using Nefarius.Utilities.DeviceManagement.PnP;

namespace Tests.CI;

[TestFixture]
[Category(TestCategories.CI)]
public class DevconEnumerationTests
{
    [Test]
    public void FindByInterfaceGuid_HidDevice_FindsAtLeastOne()
    {
        Assert.That(
            Devcon.FindByInterfaceGuid(DeviceInterfaceIds.HidDevice, out string? path, out string? instanceId),
            Is.True);

        Assert.Multiple(() =>
        {
            Assert.That(path, Is.Not.Null.And.Not.Empty);
            Assert.That(instanceId, Is.Not.Null.And.Not.Empty);
        });
    }

    [Test]
    public void FindByInterfaceGuid_UsbDevice_FindsAtLeastOne()
    {
        // GitHub-hosted Windows runners and typical PCs expose at least one USB device interface
        Assert.That(
            Devcon.FindByInterfaceGuid(DeviceInterfaceIds.UsbDevice, out string? path, out string? instanceId),
            Is.True);

        Assert.Multiple(() =>
        {
            Assert.That(path, Is.Not.Null.And.Not.Empty);
            Assert.That(instanceId, Is.Not.Null.And.Not.Empty);
        });
    }
}
