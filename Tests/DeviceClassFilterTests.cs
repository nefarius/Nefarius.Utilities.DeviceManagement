using Nefarius.Utilities.DeviceManagement.Exceptions;
using Nefarius.Utilities.DeviceManagement.PnP;

namespace Tests;

public class DeviceClassFilterTests
{
    [SetUp]
    public void Setup()
    {
    }

    /// <summary>
    ///     Requires HidHide to be installed to work!
    /// </summary>
    private const string Service01 = "HidHide";

    /// <summary>
    ///     This service must not exist!
    /// </summary>
    private const string Service02 = "HidVibe";

    /// <summary>
    ///     Tests detection of device arrival and removal
    /// </summary>
    [Test]
    public void TestAddUpperFilter()
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
        Assert.Throws(typeof(DriverServiceNotFoundException), () => DeviceClassFilters.AddUpper(DeviceClassIds.XnaComposite, Service02));
    }
}