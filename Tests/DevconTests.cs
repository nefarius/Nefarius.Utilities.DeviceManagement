using Nefarius.Utilities.DeviceManagement.PnP;

#pragma warning disable CS1591

namespace Tests;

public class DevconTests
{
    [SetUp]
    public void Setup()
    {
    }

    /// <summary>
    ///     Tests <see cref="Devcon.FindInDeviceClassByHardwareId(System.Guid,string)" />.
    /// </summary>
    [Test]
    public void TestFindInDeviceClassByHardwareId()
    {
        // High precision event timer
        const string hardwareId = @"ACPI\VEN_PNP&DEV_0103";
        Assert.Multiple(() =>
        {
            Assert.That(
                Devcon.FindInDeviceClassByHardwareId(DeviceClassIds.System, hardwareId,
                    out IEnumerable<string>? instances), Is.True);
            Assert.That(instances.Count(), Is.EqualTo(1));
        });

        // not a class GUID
        Guid nonexistent = Guid.Parse("{1A9D912E-A993-4BFF-9891-B15B7A8BC32B}");
        Assert.Multiple(() =>
        {
            Assert.That(Devcon.FindInDeviceClassByHardwareId(nonexistent, string.Empty), Is.False);
            Assert.That(Devcon.FindInDeviceClassByHardwareId(Guid.Empty, hardwareId), Is.False);
        });
    }

    [Test]
    public void TestFindInDeviceClassByHardwareIdWithNonexistent()
    {
        Assert.Multiple(() =>
        {
            Assert.That(Devcon.FindInDeviceClassByHardwareId(DeviceClassIds.System, "ROOT\\NOPE",
                out IEnumerable<string>? instances), Is.False);
            Assert.That(instances, Is.Empty);
        });
    }

    /// <summary>
    ///     Requires BthPS3 being installed for this test to work.
    /// </summary>
    [Test]
    public void TestFindInDeviceClassByHardwareIdWithPartial()
    {
        string partialHardwareId = @"BTHENUM\{1cb831ea-79cd-4508-b0fc-85f7c85ae8e0}";

        Assert.Multiple(() =>
        {
            Assert.That(Devcon.FindInDeviceClassByHardwareId(DeviceClassIds.Bluetooth, partialHardwareId,
                out IEnumerable<string>? instances, true, true), Is.True);
            Assert.That(instances, Is.Not.Empty);
        });
    }

    /// <summary>
    ///     Requires two Xbox controllers, either 360 or One or mixed, connected for this test to work.
    /// </summary>
    [Test]
    public void TestFindXusbByInterfaceGuid()
    {
        // Requires two Xbox controllers, either 360 or One or mixed
        Guid xusbInterfaceGuid = Guid.Parse("{EC87F1E3-C13B-4100-B5F7-8B84D54260CB}");
        Assert.Multiple(() =>
        {
            // 1st controller
            Assert.That(Devcon.FindByInterfaceGuid(xusbInterfaceGuid, out PnPDevice? device01), Is.True);

            // check Service property against expected values
            Assert.That(device01.GetProperty<string>(DevicePropertyKey.Device_EnumeratorName),
                Is.EqualTo("USB").Or.EqualTo("HID"));
        });
        Assert.Multiple(() =>
        {
            // 2nd controller
            Assert.That(Devcon.FindByInterfaceGuid(xusbInterfaceGuid, out PnPDevice? device02, 1), Is.True);

            // check Service property against expected values
            Assert.That(device02.GetProperty<string>(DevicePropertyKey.Device_EnumeratorName),
                Is.EqualTo("USB").Or.EqualTo("HID"));
        });
    }
}