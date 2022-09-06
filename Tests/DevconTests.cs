using Nefarius.Utilities.DeviceManagement.PnP;
using Guid = System.Guid;

namespace Tests
{
    public class DevconTests
    {
        [SetUp]
        public void Setup()
        {
        }

        /// <summary>
        ///     Tests <see cref="Devcon.FindInDeviceClassByHardwareId(System.Guid,string)"/>.
        /// </summary>
        [Test]
        public void TestFindInDeviceClassByHardwareId()
        {
            // system devices class guid
            var systemClass = Guid.Parse("{4d36e97d-e325-11ce-bfc1-08002be10318}");
            // High precision event timer
            var hardwareId = @"ACPI\VEN_PNP&DEV_0103";

            Assert.True(Devcon.FindInDeviceClassByHardwareId(systemClass, hardwareId, out var instances));
            Assert.That(instances.Count(), Is.EqualTo(1));

            // not a class GUID
            var nonexistent = Guid.Parse("{1A9D912E-A993-4BFF-9891-B15B7A8BC32B}");

            Assert.False(Devcon.FindInDeviceClassByHardwareId(nonexistent, string.Empty));
            Assert.False(Devcon.FindInDeviceClassByHardwareId(Guid.Empty, hardwareId));
        }

        [Test]
        public void TestFindByInterfaceGuid()
        {
            // Requires two Xbox controllers, either 360 or One or mixed
            var xusbInterfaceGuid = Guid.Parse("{EC87F1E3-C13B-4100-B5F7-8B84D54260CB}");

            // 1st controller
            Assert.True(Devcon.FindByInterfaceGuid(xusbInterfaceGuid, out var device01));

            // check Service property against expected values
            Assert.That(device01.GetProperty<string>(DevicePropertyDevice.EnumeratorName), Is.EqualTo("USB").Or.EqualTo("HID"));

            // 2nd controller
            Assert.True(Devcon.FindByInterfaceGuid(xusbInterfaceGuid, out var device02, instance:1));

            // check Service property against expected values
            Assert.That(device02.GetProperty<string>(DevicePropertyDevice.EnumeratorName), Is.EqualTo("USB").Or.EqualTo("HID"));
        }
    }
}