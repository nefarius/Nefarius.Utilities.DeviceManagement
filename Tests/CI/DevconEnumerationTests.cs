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
    public void FindByInterfaceGuid_UsbHostController_FindsAtLeastOne()
    {
        // Prefer host controllers over GUID_DEVINTERFACE_USB_DEVICE: GitHub-hosted Windows VMs
        // often expose a virtual XHCI controller without any USB device interfaces.
        Assert.That(
            Devcon.FindByInterfaceGuid(DeviceInterfaceIds.UsbHostController, out string? path,
                out string? instanceId),
            Is.True);

        Assert.Multiple(() =>
        {
            Assert.That(path, Is.Not.Null.And.Not.Empty);
            Assert.That(instanceId, Is.Not.Null.And.Not.Empty);
        });
    }
}
