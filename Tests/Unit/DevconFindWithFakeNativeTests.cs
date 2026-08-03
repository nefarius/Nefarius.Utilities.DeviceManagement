using Nefarius.Utilities.DeviceManagement.Internal;
using Nefarius.Utilities.DeviceManagement.PnP;

namespace Tests.Unit;

[TestFixture]
[Category(TestCategories.Unit)]
public class DevconFindWithFakeNativeTests
{
    private FakeDeviceManagementNative _fake = null!;

    [SetUp]
    public void SetUp()
    {
        _fake = new FakeDeviceManagementNative();
        DeviceManagementNative.Current = _fake;
    }

    [TearDown]
    public void TearDown()
    {
        DeviceManagementNative.ResetToReal();
    }

    [Test]
    public void FindInDeviceClassByHardwareId_ExactMatch()
    {
        Guid classGuid = DeviceClassIds.System;
        _fake.AddClassDevice(classGuid, @"ACPI\PNP0103\0", new[] { @"ACPI\VEN_PNP&DEV_0103" });
        _fake.AddClassDevice(classGuid, @"ACPI\OTHER\0", new[] { @"ACPI\VEN_PNP&DEV_9999" });

        bool found = Devcon.FindInDeviceClassByHardwareId(classGuid, @"ACPI\VEN_PNP&DEV_0103",
            out IEnumerable<string> instances, presentOnly: true);

        Assert.Multiple(() =>
        {
            Assert.That(found, Is.True);
            Assert.That(instances.Single(), Is.EqualTo(@"ACPI\PNP0103\0"));
        });
    }

    [Test]
    public void FindInDeviceClassByHardwareId_PartialMatch()
    {
        Guid classGuid = DeviceClassIds.Bluetooth;
        _fake.AddClassDevice(classGuid, @"BTHENUM\DEV_1",
            new[] { @"BTHENUM\{1CB831EA-79CD-4508-B0FC-85F7C85AE8E0}_LOCALMFG&0000" });

        bool found = Devcon.FindInDeviceClassByHardwareId(classGuid,
            @"BTHENUM\{1cb831ea-79cd-4508-b0fc-85f7c85ae8e0}",
            out IEnumerable<string> instances, presentOnly: true, allowPartial: true);

        Assert.Multiple(() =>
        {
            Assert.That(found, Is.True);
            Assert.That(instances, Is.Not.Empty);
        });
    }

    [Test]
    public void FindInDeviceClassByHardwareId_NoMatch()
    {
        Guid classGuid = DeviceClassIds.System;
        _fake.AddClassDevice(classGuid, @"ACPI\PNP0103\0", new[] { @"ACPI\VEN_PNP&DEV_0103" });

        bool found = Devcon.FindInDeviceClassByHardwareId(classGuid, "ROOT\\NOPE",
            out IEnumerable<string> instances, presentOnly: true);

        Assert.Multiple(() =>
        {
            Assert.That(found, Is.False);
            Assert.That(instances, Is.Empty);
        });
    }
}
