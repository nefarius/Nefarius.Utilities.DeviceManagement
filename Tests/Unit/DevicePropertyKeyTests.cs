using Nefarius.Utilities.DeviceManagement.PnP;

namespace Tests.Unit;

[TestFixture]
[Category(TestCategories.Unit)]
public class DevicePropertyKeyTests
{
    [Test]
    public void CustomProperty_Equality_ByCategoryPidAndType()
    {
        Guid category = Guid.Parse("{83DA6326-97A6-4088-9453-A1923F573B29}");
        DevicePropertyKey a = CustomDeviceProperty.CreateCustomDeviceProperty(category, 15, typeof(bool));
        DevicePropertyKey b = CustomDeviceProperty.CreateCustomDeviceProperty(category, 15, typeof(bool));
        DevicePropertyKey differentPid = CustomDeviceProperty.CreateCustomDeviceProperty(category, 16, typeof(bool));
        DevicePropertyKey differentType = CustomDeviceProperty.CreateCustomDeviceProperty(category, 15, typeof(int));

        Assert.Multiple(() =>
        {
            Assert.That(a, Is.EqualTo(b));
            Assert.That(a.GetHashCode(), Is.EqualTo(b.GetHashCode()));
            Assert.That(a, Is.Not.EqualTo(differentPid));
            Assert.That(a, Is.Not.EqualTo(differentType));
        });
    }

    [Test]
    public void WellKnownProperty_HasExpectedIdentity()
    {
        DevicePropertyKey key = DevicePropertyKey.Device_HardwareIds;

        Assert.Multiple(() =>
        {
            Assert.That(key.CategoryGuid, Is.Not.EqualTo(Guid.Empty));
            Assert.That(key.PropertyIdentifier, Is.GreaterThan(0u));
            Assert.That(key.PropertyType, Is.EqualTo(typeof(string[])));
        });
    }
}
