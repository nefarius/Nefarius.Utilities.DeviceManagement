using Nefarius.Utilities.DeviceManagement.Drivers;

namespace Tests.CI;

[TestFixture]
[Category(TestCategories.CI)]
public class DriverStoreTests
{
    [Test]
    public void ExistingDrivers_IsNonEmpty_WithInfPaths()
    {
        List<string> packages = DriverStore.ExistingDrivers.ToList();

        Assert.Multiple(() =>
        {
            Assert.That(packages, Is.Not.Empty);
            Assert.That(packages.Any(p => p.EndsWith(".inf", StringComparison.OrdinalIgnoreCase)), Is.True);
        });
    }
}
