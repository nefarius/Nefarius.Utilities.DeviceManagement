using Nefarius.Utilities.DeviceManagement.Drivers;
using Nefarius.Utilities.DeviceManagement.Extensions;
using Nefarius.Utilities.DeviceManagement.PnP;

using Spectre.Console;

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
    //[Test]
    public void TestPnPDeviceInstallNullDriver()
    {
        Assert.That(Devcon.FindByInterfaceGuid(DeviceInterfaceIds.XUsbDevice, out string? path, out string? instanceId),
            Is.True);

        PnPDevice device = PnPDevice.GetDeviceByInstanceId(instanceId);

        device.InstallNullDriver();
    }

    /// <summary>
    ///     Tests grabbing driver metadata from the first found HID device.
    /// </summary>
    [Test]
    public void TestGetDriverMeta()
    {
        Assert.Multiple(() =>
        {
            Assert.That(
                Devcon.FindByInterfaceGuid(DeviceInterfaceIds.HidDevice, out string? path, out string? instanceId),
                Is.True);
            Assert.That(instanceId, Is.Not.Null.Or.Empty);

            PnPDevice? device = PnPDevice.GetDeviceByInterfaceId(path);
            Assert.That(device, Is.Not.Null);

            DriverMeta? meta = device.GetCurrentDriver();
            Assert.That(meta, Is.Not.Null);
        });
    }

    [Test]
    public void TestGetInstanceIdFromInterfaceId()
    {
        AnsiConsole.MarkupLine("[yellow]Connect ONE Xbox Controller for this test![/]");

        // Requires one Xbox controller, either 360 or One or compatible
        Assert.Multiple(() =>
        {
            // 1st controller
            Assert.That(
                Devcon.FindByInterfaceGuid(DeviceInterfaceIds.XUsbDevice, out string? path, out string? instanceId),
                Is.True);

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
        AnsiConsole.MarkupLine("[yellow]Connect ONE VIRTUAL Xbox 360 Controller for this test![/]");

        const string hardwareId = "USB\\VID_045E&PID_028E";

        Assert.Multiple(() =>
        {
            Assert.That(Devcon.FindInDeviceClassByHardwareId(DeviceClassIds.XnaComposite, hardwareId,
                out IEnumerable<string>? instances, true), Is.True);

            List<string> list = instances.ToList();

            Assert.That(list, Is.Not.Empty);

            PnPDevice? device = list.Select(e => PnPDevice.GetDeviceByInstanceId(e))
                .FirstOrDefault(dev => dev.IsVirtual());

            Assert.That(device, Is.Not.Null);

            Assert.That(device!.IsVirtual(), Is.True);
        });
    }
}