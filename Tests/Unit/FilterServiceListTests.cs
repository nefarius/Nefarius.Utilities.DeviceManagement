using Nefarius.Utilities.DeviceManagement.Internal;

namespace Tests.Unit;

[TestFixture]
[Category(TestCategories.Unit)]
public class FilterServiceListTests
{
    [Test]
    public void Add_CreatesListFromNull()
    {
        IReadOnlyList<string> result = FilterServiceList.Add(null, "HidHide");

        Assert.That(result, Is.EqualTo(new[] { "HidHide" }));
    }

    [Test]
    public void Add_DoesNotDuplicateExactEntry()
    {
        IReadOnlyList<string> result = FilterServiceList.Add(new[] { "HidHide" }, "HidHide");

        Assert.That(result, Is.EqualTo(new[] { "HidHide" }));
    }

    [Test]
    public void Add_AppendsDistinctService()
    {
        IReadOnlyList<string> result = FilterServiceList.Add(new[] { "HidHide" }, "USBPcap");

        Assert.That(result, Is.EqualTo(new[] { "HidHide", "USBPcap" }));
    }

    [Test]
    public void Add_StripsEmptyEntries()
    {
        IReadOnlyList<string> result = FilterServiceList.Add(new[] { "HidHide", " ", "" }, "USBPcap");

        Assert.That(result, Is.EqualTo(new[] { "HidHide", "USBPcap" }));
    }

    [Test]
    public void Remove_IsCaseInsensitive()
    {
        IReadOnlyList<string> result = FilterServiceList.Remove(new[] { "HidHide", "USBPcap" }, "hidhide");

        Assert.That(result, Is.EqualTo(new[] { "USBPcap" }));
    }

    [Test]
    public void Remove_MissingService_LeavesList()
    {
        IReadOnlyList<string> result = FilterServiceList.Remove(new[] { "HidHide" }, "Nope");

        Assert.That(result, Is.EqualTo(new[] { "HidHide" }));
    }

    [Test]
    public void Remove_NullAndWhitespaceEntries_DoNotThrow()
    {
        IReadOnlyList<string> result =
            FilterServiceList.Remove(new[] { null, "HidHide", " ", "USBPcap" }!, "hidhide");

        Assert.That(result, Is.EqualTo(new[] { "USBPcap" }));
    }
}
