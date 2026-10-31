using Nefarius.Utilities.DeviceManagement.Internal;

namespace Tests.Unit;

[TestFixture]
[Category(TestCategories.Unit)]
public class DriverStoreParsingTests
{
    [TestCase(@"C:\Windows\System32\DriverStore\FileRepository\foo.inf_amd64_0123abcd\foo.inf", "foo.inf")]
    [TestCase(@"C:\Windows\System32\DriverStore\FileRepository\my.driver.inf_arm64_01\my.driver.inf",
        "my.driver.inf")]
    [TestCase(@"C:\Windows\System32\DriverStore\FileRepository\noseparator\x.inf", null)]
    [TestCase(@"x.inf", null)]
    public void GetOriginalInfName_ParsesPackageDirectory(string path, string? expected)
    {
        Assert.That(InfVersionReader.GetOriginalInfName(path), Is.EqualTo(expected));
    }

    [Test]
    public void ParseDriverVer_DateAndVersion()
    {
        InfVersionReader.ParseDriverVer("04/21/2009,6.1.7600.16385", out DateTime? date, out Version? version);

        Assert.Multiple(() =>
        {
            Assert.That(date, Is.EqualTo(new DateTime(2009, 4, 21)));
            Assert.That(version, Is.EqualTo(new Version(6, 1, 7600, 16385)));
        });
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase("garbage")]
    public void ParseDriverVer_Invalid_ReturnsNulls(string? value)
    {
        InfVersionReader.ParseDriverVer(value, out DateTime? date, out Version? version);

        Assert.Multiple(() =>
        {
            Assert.That(date, Is.Null);
            Assert.That(version, Is.Null);
        });
    }

    [Test]
    public void ParseMultiSz_SplitsAtNulAndStopsAtDoubleNul()
    {
        List<string> ids = DeviceEnumerator.ParseMultiSz("A\\B\0C\\D\0\0\0".ToCharArray());

        Assert.That(ids, Is.EqualTo(new[] { "A\\B", "C\\D" }));
    }
}