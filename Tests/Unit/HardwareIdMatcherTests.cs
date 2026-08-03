using Nefarius.Utilities.DeviceManagement.Internal;

namespace Tests.Unit;

[TestFixture]
[Category(TestCategories.Unit)]
public class HardwareIdMatcherTests
{
    [Test]
    public void ExactMatch_IsCaseInsensitive()
    {
        string[] ids = { @"USB\VID_045E&PID_028E", @"USB\VID_045E&PID_028E&REV_0100" };

        Assert.That(HardwareIdMatcher.Matches(ids, @"usb\vid_045e&pid_028e", allowPartial: false), Is.True);
    }

    [Test]
    public void ExactMatch_RejectsSubstring()
    {
        string[] ids = { @"USB\VID_045E&PID_028E&REV_0100" };

        Assert.That(HardwareIdMatcher.Matches(ids, @"USB\VID_045E&PID_028E", allowPartial: false), Is.False);
    }

    [Test]
    public void PartialMatch_AcceptsSubstring()
    {
        string[] ids = { @"BTHENUM\{1CB831EA-79CD-4508-B0FC-85F7C85AE8E0}_LOCALMFG&0000" };

        Assert.That(
            HardwareIdMatcher.Matches(ids, @"BTHENUM\{1cb831ea-79cd-4508-b0fc-85f7c85ae8e0}", allowPartial: true),
            Is.True);
    }

    [Test]
    public void EmptyNeedle_ReturnsFalse()
    {
        Assert.That(HardwareIdMatcher.Matches(new[] { "A" }, "", allowPartial: false), Is.False);
    }

    [Test]
    public void NullIds_ReturnsFalse()
    {
        Assert.That(HardwareIdMatcher.Matches(null!, "A", allowPartial: false), Is.False);
    }

    [Test]
    public void NullElements_AreIgnored()
    {
        string?[] ids = { null, @"USB\VID_045E&PID_028E", null };

        Assert.That(HardwareIdMatcher.Matches(ids!, @"USB\VID_045E&PID_028E", allowPartial: false), Is.True);
    }
}
