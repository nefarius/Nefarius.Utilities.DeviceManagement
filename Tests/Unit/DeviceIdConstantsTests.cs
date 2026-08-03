using Nefarius.Utilities.DeviceManagement.PnP;

namespace Tests.Unit;

[TestFixture]
[Category(TestCategories.Unit)]
public class DeviceIdConstantsTests
{
    [Test]
    public void DeviceClassIds_AreStable()
    {
        Assert.Multiple(() =>
        {
            Assert.That(DeviceClassIds.System, Is.EqualTo(Guid.Parse("{4d36e97d-e325-11ce-bfc1-08002be10318}")));
            Assert.That(DeviceClassIds.Usb, Is.EqualTo(Guid.Parse("{36FC9E60-C465-11CF-8056-444553540000}")));
            Assert.That(DeviceClassIds.Bluetooth, Is.EqualTo(Guid.Parse("{e0cbf06c-cd8b-4647-bb8a-263b43f0f974}")));
            Assert.That(DeviceClassIds.XnaComposite, Is.EqualTo(Guid.Parse("{d61ca365-5af4-4486-998b-9db4734c6ca3}")));
            Assert.That(DeviceClassIds.XboxComposite, Is.EqualTo(Guid.Parse("{05f5cfe2-4733-4950-a6bb-07aad01a3a84}")));
            Assert.That(DeviceClassIds.HumanInterfaceDevices,
                Is.EqualTo(Guid.Parse("{745a17a0-74d3-11d0-b6fe-00a0c90f57da}")));
        });
    }

    [Test]
    public void DeviceInterfaceIds_AreStable()
    {
        Assert.Multiple(() =>
        {
            Assert.That(DeviceInterfaceIds.UsbHostController,
                Is.EqualTo(Guid.Parse("{3abf6f2d-71c4-462a-8a92-1e6861e6af27}")));
            Assert.That(DeviceInterfaceIds.UsbHub,
                Is.EqualTo(Guid.Parse("{f18a0e88-c30c-11d0-8815-00a0c906bed8}")));
            Assert.That(DeviceInterfaceIds.UsbDevice,
                Is.EqualTo(Guid.Parse("{a5dcbf10-6530-11d2-901f-00c04fb951ed}")));
            Assert.That(DeviceInterfaceIds.XUsbDevice,
                Is.EqualTo(Guid.Parse("{EC87F1E3-C13B-4100-B5F7-8B84D54260CB}")));
        });
    }
}
