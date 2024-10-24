using Nefarius.Utilities.DeviceManagement.PnP;

using Spectre.Console;

namespace Tests;
#pragma warning disable CS1591

public class DeviceNotificationListenerTests
{
    [SetUp]
    public void Setup()
    {
    }

    /// <summary>
    ///     Tests detection of device arrival and removal
    /// </summary>
    [Test]
    public void TestDeviceNotificationListener()
    {
        TimeSpan waitTime = TimeSpan.FromSeconds(10);

        using DeviceNotificationListener listener = new();

        // Requires any HID device
        listener.StartListen(DeviceInterfaceIds.HidDevice);

        AutoResetEvent wait = new(false);

        // Arrival
        listener.DeviceArrived += args =>
        {
            // compare interface GUID
            Assert.That(args.InterfaceGuid, Is.EqualTo(DeviceInterfaceIds.HidDevice));

            PnPDevice? device = PnPDevice.GetDeviceByInterfaceId(args.SymLink);

            Assert.That(device, Is.Not.Null);

            Assert.That(device.GetProperty<string>(DevicePropertyKey.Device_EnumeratorName),
                Is.EqualTo("HID").IgnoreCase);

            wait.Set();
        };
        
        AnsiConsole.MarkupLine("[yellow]Connect any HID device now within 10 seconds![/]");

        // plug in HID device now
        Assert.That(wait.WaitOne(waitTime), Is.True);

        // Removal
        listener.DeviceRemoved += args =>
        {
            // compare interface GUID
            Assert.That(args.InterfaceGuid, Is.EqualTo(DeviceInterfaceIds.HidDevice));

            PnPDevice? device = PnPDevice.GetDeviceByInterfaceId(args.SymLink, DeviceLocationFlags.Phantom);

            Assert.That(device, Is.Not.Null);

            Assert.That(device.GetProperty<string>(DevicePropertyKey.Device_EnumeratorName),
                Is.EqualTo("HID").IgnoreCase);

            wait.Set();
        };
        
        AnsiConsole.MarkupLine("[yellow]Unplug the HID device now within 10 seconds![/]");

        // unplug it now
        Assert.That(wait.WaitOne(waitTime), Is.True);

        listener.StopListen();
    }
}