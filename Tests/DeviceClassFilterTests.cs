using Nefarius.Utilities.DeviceManagement.Exceptions;
using Nefarius.Utilities.DeviceManagement.PnP;

namespace Tests;
#pragma warning disable CS1591

public class DeviceClassFilterTests
{
    /// <summary>
    ///     Requires HidHide to be installed to work!
    /// </summary>
    private const string Service01 = "HidHide";

    /// <summary>
    ///     This service must not exist!
    /// </summary>
    private const string Service02 = "HidVibe";

    /// <summary>
    ///     Requires USBPcap to be installed to work!
    /// </summary>
    private const string Service03 = "USBPcap";

    [SetUp]
    public void Setup()
    {
    }

    /// <summary>
    ///     Tests for <see cref="DeviceClassFilters" />.
    /// </summary>
    [Test]
    public void TestUpperFilter()
    {
        DeviceClassFilters.DeleteUpper(DeviceClassIds.XnaComposite);
        // expect it being gone
        Assert.That(DeviceClassFilters.GetUpper(DeviceClassIds.XnaComposite), Is.Null);

        DeviceClassFilters.AddUpper(DeviceClassIds.XnaComposite, Service01);
        // expect exactly one entry, our added service
        Assert.That(DeviceClassFilters.GetUpper(DeviceClassIds.XnaComposite), Is.Not.Null);
        Assert.That(DeviceClassFilters.GetUpper(DeviceClassIds.XnaComposite)!.Count(), Is.EqualTo(1));
        CollectionAssert.Contains(DeviceClassFilters.GetUpper(DeviceClassIds.XnaComposite), Service01);

        // add the same service again
        DeviceClassFilters.AddUpper(DeviceClassIds.XnaComposite, Service01);
        // must not be added as duplicate
        Assert.That(DeviceClassFilters.GetUpper(DeviceClassIds.XnaComposite)!.Count(), Is.EqualTo(1));
        CollectionAssert.Contains(DeviceClassFilters.GetUpper(DeviceClassIds.XnaComposite), Service01);

        // add invalid service, has to throw exception
        Assert.Throws(typeof(DriverServiceNotFoundException),
            () => DeviceClassFilters.AddUpper(DeviceClassIds.XnaComposite, Service02));

        // add new service
        DeviceClassFilters.AddUpper(DeviceClassIds.XnaComposite, Service03);
        // expect to get 2 services now
        Assert.That(DeviceClassFilters.GetUpper(DeviceClassIds.XnaComposite)!.Count(), Is.EqualTo(2));
        CollectionAssert.Contains(DeviceClassFilters.GetUpper(DeviceClassIds.XnaComposite), Service01);
        CollectionAssert.Contains(DeviceClassFilters.GetUpper(DeviceClassIds.XnaComposite), Service03);

        // remove first service
        DeviceClassFilters.RemoveUpper(DeviceClassIds.XnaComposite, Service01);
        // expect to get one service now, excluding the removed one
        Assert.That(DeviceClassFilters.GetUpper(DeviceClassIds.XnaComposite)!.Count(), Is.EqualTo(1));
        CollectionAssert.DoesNotContain(DeviceClassFilters.GetUpper(DeviceClassIds.XnaComposite), Service01);
        CollectionAssert.Contains(DeviceClassFilters.GetUpper(DeviceClassIds.XnaComposite), Service03);

        // remove remaining service
        DeviceClassFilters.RemoveUpper(DeviceClassIds.XnaComposite, Service03);
        // expect not null, but empty
        Assert.That(DeviceClassFilters.GetUpper(DeviceClassIds.XnaComposite), Is.Not.Null);
        CollectionAssert.IsEmpty(DeviceClassFilters.GetUpper(DeviceClassIds.XnaComposite));

        // remove entirely
        DeviceClassFilters.DeleteUpper(DeviceClassIds.XnaComposite);
        // expect it being gone
        Assert.That(DeviceClassFilters.GetUpper(DeviceClassIds.XnaComposite), Is.Null);
    }

    /// <summary>
    ///     Tests for <see cref="DeviceClassFilters" />.
    /// </summary>
    [Test]
    public void TestLowerFilter()
    {
        DeviceClassFilters.DeleteLower(DeviceClassIds.XnaComposite);
        // expect it being gone
        Assert.That(DeviceClassFilters.GetLower(DeviceClassIds.XnaComposite), Is.Null);

        DeviceClassFilters.AddLower(DeviceClassIds.XnaComposite, Service01);
        // expect exactly one entry, our added service
        Assert.That(DeviceClassFilters.GetLower(DeviceClassIds.XnaComposite), Is.Not.Null);
        Assert.That(DeviceClassFilters.GetLower(DeviceClassIds.XnaComposite)!.Count(), Is.EqualTo(1));
        CollectionAssert.Contains(DeviceClassFilters.GetLower(DeviceClassIds.XnaComposite), Service01);

        // add the same service again
        DeviceClassFilters.AddLower(DeviceClassIds.XnaComposite, Service01);
        // must not be added as duplicate
        Assert.That(DeviceClassFilters.GetLower(DeviceClassIds.XnaComposite)!.Count(), Is.EqualTo(1));
        CollectionAssert.Contains(DeviceClassFilters.GetLower(DeviceClassIds.XnaComposite), Service01);

        // add invalid service, has to throw exception
        Assert.Throws(typeof(DriverServiceNotFoundException),
            () => DeviceClassFilters.AddLower(DeviceClassIds.XnaComposite, Service02));

        // add new service
        DeviceClassFilters.AddLower(DeviceClassIds.XnaComposite, Service03);
        // expect to get 2 services now
        Assert.That(DeviceClassFilters.GetLower(DeviceClassIds.XnaComposite)!.Count(), Is.EqualTo(2));
        CollectionAssert.Contains(DeviceClassFilters.GetLower(DeviceClassIds.XnaComposite), Service01);
        CollectionAssert.Contains(DeviceClassFilters.GetLower(DeviceClassIds.XnaComposite), Service03);

        // remove first service
        DeviceClassFilters.RemoveLower(DeviceClassIds.XnaComposite, Service01);
        // expect to get one service now, excluding the removed one
        Assert.That(DeviceClassFilters.GetLower(DeviceClassIds.XnaComposite)!.Count(), Is.EqualTo(1));
        CollectionAssert.DoesNotContain(DeviceClassFilters.GetLower(DeviceClassIds.XnaComposite), Service01);
        CollectionAssert.Contains(DeviceClassFilters.GetLower(DeviceClassIds.XnaComposite), Service03);

        // remove remaining service
        DeviceClassFilters.RemoveLower(DeviceClassIds.XnaComposite, Service03);
        // expect not null, but empty
        Assert.That(DeviceClassFilters.GetLower(DeviceClassIds.XnaComposite), Is.Not.Null);
        CollectionAssert.IsEmpty(DeviceClassFilters.GetLower(DeviceClassIds.XnaComposite));

        // remove entirely
        DeviceClassFilters.DeleteLower(DeviceClassIds.XnaComposite);
        // expect it being gone
        Assert.That(DeviceClassFilters.GetLower(DeviceClassIds.XnaComposite), Is.Null);
    }
}