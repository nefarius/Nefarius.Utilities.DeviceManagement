using Nefarius.Utilities.DeviceManagement.Drivers;
using Nefarius.Utilities.DeviceManagement.Extensions;
using Nefarius.Utilities.DeviceManagement.PnP;

using Spectre.Console;

namespace Tests;
#pragma warning disable CS1591

public class PnPDeviceTests
{
    private const string DualSenseHardwareId = @"USB\VID_054C&PID_0CE6";

    /// <summary>
    ///     Requires one physical (or virtual) DualSense controller.
    /// </summary>
    [Test]
    [Explicit]
    [Category(TestCategories.Hardware)]
    [Category(TestCategories.Destructive)]
    [Category(TestCategories.Admin)]
    public void TestPnPDeviceInstallCustomDriver()
    {
        Assert.That(
            Devcon.FindInDeviceClassByHardwareId(DeviceClassIds.Usb, DualSenseHardwareId,
                out IEnumerable<string>? instances, true, true),
            Is.True,
            "Connect a DualSense (VID_054C&PID_0CE6) for this test.");

        string? instanceId = instances.FirstOrDefault();
        Assert.That(instanceId, Is.Not.Null.And.Not.Empty);

        PnPDevice device = PnPDevice.GetDeviceByInstanceId(instanceId!);
        device.InstallCustomDriver("winusb.inf");
    }

    /// <summary>
    ///     Requires one emulated X360 controller.
    /// </summary>
    [Test]
    [Explicit]
    [Category(TestCategories.Hardware)]
    [Category(TestCategories.Destructive)]
    [Category(TestCategories.Admin)]
    public void TestPnPDeviceInstallNullDriver()
    {
        Assert.That(Devcon.FindByInterfaceGuid(DeviceInterfaceIds.XUsbDevice, out string? path, out string? instanceId),
            Is.True,
            "Connect an emulated X360 controller for this test.");

        PnPDevice device = PnPDevice.GetDeviceByInstanceId(instanceId);
        device.InstallNullDriver();
    }

    /// <summary>
    ///     Tests grabbing driver metadata from the first found HID device.
    /// </summary>
    [Test]
    [Category(TestCategories.CI)]
    public void TestGetDriverMeta()
    {
        Assert.Multiple(() =>
        {
            Assert.That(
                Devcon.FindByInterfaceGuid(DeviceInterfaceIds.HidDevice, out string? path, out string? instanceId),
                Is.True);
            Assert.That(instanceId, Is.Not.Null.And.Not.Empty);

            PnPDevice? device = PnPDevice.GetDeviceByInterfaceId(path);
            Assert.That(device, Is.Not.Null);

            DriverMeta? meta = device.GetCurrentDriver();
            Assert.That(meta, Is.Not.Null);
        });
    }

    [Test]
    [Explicit]
    [Category(TestCategories.Hardware)]
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
    [Explicit]
    [Category(TestCategories.Hardware)]
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

    /// <summary>
    ///     https://github.com/nefarius/Nefarius.Utilities.DeviceManagement/issues/85
    /// </summary>
    [Test]
    [Explicit]
    [Category(TestCategories.Hardware)]
    public void TestPnPDeviceGetBooleanBluetoothProperty()
    {
        int instances = 0;

        Guid bluetoothDeviceInterface = Guid.Parse("{00f40965-e89d-4487-9890-87c3abb211f4}");
        while (Devcon.FindByInterfaceGuid(bluetoothDeviceInterface, out PnPDevice? device, instances++))
        {
            DevicePropertyKey? connectedProperty =
                CustomDeviceProperty.CreateCustomDeviceProperty(Guid.Parse("{83DA6326-97A6-4088-9453-A1923F573B29}"),
                    15, typeof(bool));
            bool connected = device.GetProperty<bool>(connectedProperty);

            Assert.That(connected, Is.True);
        }
    }
}
