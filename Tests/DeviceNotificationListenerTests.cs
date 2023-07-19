using Nefarius.Utilities.DeviceManagement.PnP;

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

            Assert.IsNotNull(device);

            Assert.That(device.GetProperty<string>(DevicePropertyKey.Device_EnumeratorName),
                Is.EqualTo("HID").IgnoreCase);

            wait.Set();
        };

        // plug in HID device now
        Assert.IsTrue(wait.WaitOne(waitTime));

        // Removal
        listener.DeviceRemoved += args =>
        {
            // compare interface GUID
            Assert.That(args.InterfaceGuid, Is.EqualTo(DeviceInterfaceIds.HidDevice));

            PnPDevice? device = PnPDevice.GetDeviceByInterfaceId(args.SymLink, DeviceLocationFlags.Phantom);

            Assert.IsNotNull(device);

            Assert.That(device.GetProperty<string>(DevicePropertyKey.Device_EnumeratorName),
                Is.EqualTo("HID").IgnoreCase);

            wait.Set();
        };

        // unplug it now
        Assert.IsTrue(wait.WaitOne(waitTime));

        listener.StopListen();
    }
}