using Nefarius.Utilities.DeviceManagement.Internal;

namespace Tests.Unit;

[TestFixture]
[Category(TestCategories.Unit)]
public class VirtualDeviceEvaluatorTests
{
    [Test]
    public void RootSystem_IsVirtual()
    {
        Assert.That(VirtualDeviceEvaluator.IsRootEnumeratedVirtual(@"ROOT\SYSTEM\0000"), Is.True);
    }

    [Test]
    public void RootUsb_IsVirtual()
    {
        Assert.That(VirtualDeviceEvaluator.IsRootEnumeratedVirtual(@"ROOT\USB\0000"), Is.True);
    }

    [Test]
    public void PciDevice_IsNotVirtual()
    {
        Assert.That(VirtualDeviceEvaluator.IsRootEnumeratedVirtual(@"PCI\VEN_8086&DEV_1234"), Is.False);
    }

    [Test]
    public void Walk_VirtualChain_ReturnsTrue()
    {
        // leaf -> ROOT\SYSTEM\0001 -> HTREE\ROOT\0
        Dictionary<string, string?> parents = new(StringComparer.OrdinalIgnoreCase)
        {
            ["USB\\VID_045E&PID_028E\\1"] = @"ROOT\SYSTEM\0001",
            ["ROOT\\SYSTEM\\0001"] = VirtualDeviceEvaluator.TreeRootParentId
        };

        bool result = VirtualDeviceEvaluator.WalkAndEvaluate(
            @"USB\VID_045E&PID_028E\1",
            id => parents.TryGetValue(id, out string? parent) ? parent : null);

        Assert.That(result, Is.True);
    }

    [Test]
    public void Walk_HardwareChain_ReturnsFalse()
    {
        Dictionary<string, string?> parents = new(StringComparer.OrdinalIgnoreCase)
        {
            ["USB\\VID_045E&PID_028E\\1"] = @"PCI\VEN_8086&DEV_15E9\0",
            ["PCI\\VEN_8086&DEV_15E9\\0"] = VirtualDeviceEvaluator.TreeRootParentId
        };

        bool result = VirtualDeviceEvaluator.WalkAndEvaluate(
            @"USB\VID_045E&PID_028E\1",
            id => parents.TryGetValue(id, out string? parent) ? parent : null);

        Assert.That(result, Is.False);
    }

    [Test]
    public void Walk_ExcludePredicate_ReturnsFalse()
    {
        Dictionary<string, string?> parents = new(StringComparer.OrdinalIgnoreCase)
        {
            ["USB\\VID_045E&PID_028E\\1"] = @"ROOT\SYSTEM\0001",
            ["ROOT\\SYSTEM\\0001"] = VirtualDeviceEvaluator.TreeRootParentId
        };

        bool result = VirtualDeviceEvaluator.WalkAndEvaluate(
            @"USB\VID_045E&PID_028E\1",
            id => parents.TryGetValue(id, out string? parent) ? parent : null,
            id => id.Contains("VID_045E", StringComparison.OrdinalIgnoreCase));

        Assert.That(result, Is.False);
    }

    [Test]
    public void Walk_MissingParent_EvaluatesLastKnownNode()
    {
        bool result = VirtualDeviceEvaluator.WalkAndEvaluate(
            @"ROOT\SYSTEM\0001",
            _ => null);

        Assert.That(result, Is.True);
    }
}
