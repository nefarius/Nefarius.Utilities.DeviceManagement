using Nefarius.Utilities.DeviceManagement.PnP;

namespace Tests;

public class DeviceClassFilterTests
{
    [SetUp]
    public void Setup()
    {
    }

    /// <summary>
    ///     Tests detection of device arrival and removal
    /// </summary>
    [Test]
    public void TestAddUpperFilter()
    {
        DeviceClassFilters.DeleteUpper(DeviceClassIds.XnaComposite);

        DeviceClassFilters.AddUpper(DeviceClassIds.XnaComposite, "HidHide");
        
        DeviceClassFilters.RemoveUpper(DeviceClassIds.XnaComposite, "HidHide");
    }
}