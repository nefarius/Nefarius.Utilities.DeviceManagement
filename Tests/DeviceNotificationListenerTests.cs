using Nefarius.Utilities.DeviceManagement.PnP;

namespace Tests;

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

        // Requires any HID device
        Guid xusbInterfaceGuid =
            new Guid((int)0x4D1E55B2L, 0xF16F, 0x11CF, 0x88, 0xCB, 0x00, 0x11, 0x11, 0x00, 0x00, 0x30);

        using DeviceNotificationListener listener = new DeviceNotificationListener();

        listener.StartListen(xusbInterfaceGuid);

        AutoResetEvent wait = new AutoResetEvent(false);

        // Arrival
        listener.DeviceArrived += args =>
        {
            // compare interface GUID
            Assert.That(args.InterfaceGuid, Is.EqualTo(xusbInterfaceGuid));

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
            Assert.That(args.InterfaceGuid, Is.EqualTo(xusbInterfaceGuid));

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