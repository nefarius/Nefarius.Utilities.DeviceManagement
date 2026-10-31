using Nefarius.Utilities.DeviceManagement.Drivers;
using Nefarius.Utilities.DeviceManagement.PnP;

namespace Tests.CI;

[TestFixture]
[Category(TestCategories.CI)]
public class DriverStoreQueryTests
{
    [Test]
    public void EnumerateDevices_All_ReturnsDevicesWithInstanceIds()
    {
        IReadOnlyList<DeviceSummary> devices = Devcon.EnumerateDevices(false);

        Assert.Multiple(() =>
        {
            Assert.That(devices, Is.Not.Empty);
            Assert.That(devices.All(d => !string.IsNullOrEmpty(d.InstanceId)), Is.True);
            Assert.That(devices.Any(d => d.IsPresent), Is.True);
        });
    }

    [Test]
    public void EnumerateDevices_PresentOnly_IsSubsetAndAllPresent()
    {
        IReadOnlyList<DeviceSummary> present = Devcon.EnumerateDevices();
        IReadOnlyList<DeviceSummary> all = Devcon.EnumerateDevices(false);

        Assert.Multiple(() =>
        {
            Assert.That(present, Is.Not.Empty);
            Assert.That(present.All(d => d.IsPresent), Is.True);
            Assert.That(all.Count, Is.GreaterThanOrEqualTo(present.Count));
        });
    }

    [Test]
    public void GetPackages_IncludingInbox_ReturnsPackagesWithStoreInfPaths()
    {
        IReadOnlyList<DriverStorePackage> packages = DriverStore.GetPackages(true);

        Assert.Multiple(() =>
        {
            Assert.That(packages, Is.Not.Empty);
            Assert.That(packages.All(p => p.StoreInfPath.EndsWith(".inf", StringComparison.OrdinalIgnoreCase)),
                Is.True);
            Assert.That(packages.Any(p => p.IsInbox), Is.True);
            Assert.That(packages.Any(p => p.Provider is not null && p.DriverVersion is not null), Is.True);
        });
    }

    [Test]
    public void GetPackages_Default_ExcludesInbox()
    {
        IReadOnlyList<DriverStorePackage> packages = DriverStore.GetPackages();

        Assert.That(packages.Any(p => p.IsInbox), Is.False);
    }

    [Test]
    public void GetPackages_BoundDevices_ReferenceExistingDevices()
    {
        IReadOnlyList<DriverStorePackage> packages = DriverStore.GetPackages(true);
        HashSet<string> known = Devcon.EnumerateDevices(false)
            .Select(d => d.InstanceId)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        List<string> bound = packages.SelectMany(p => p.BoundDeviceInstanceIds).ToList();

        Assert.That(bound.All(known.Contains), Is.True);
    }
}