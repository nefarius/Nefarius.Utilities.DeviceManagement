using System.Runtime.InteropServices;

using Nefarius.Utilities.DeviceManagement.Util;

namespace Tests.Unit;

[TestFixture]
[Category(TestCategories.Unit)]
public class MultiSzConversionTests
{
    [Test]
    public void RoundTrip_PreservesEntries()
    {
        string[] expected = { "HidHide", "USBPcap" };
        IntPtr buffer = IntPtr.Zero;

        try
        {
            buffer = expected.StringArrayToMultiSzPointer(out int length);
            Assert.That(buffer, Is.Not.EqualTo(IntPtr.Zero));
            Assert.That(length, Is.GreaterThan(0));

            string[] actual = buffer.MultiSzPointerToStringArray(length).ToArray();
            Assert.That(actual, Is.EqualTo(expected));
        }
        finally
        {
            if (buffer != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(buffer);
            }
        }
    }

    [Test]
    public void RoundTrip_SingleEntry()
    {
        string[] expected = { "HidHide" };
        IntPtr buffer = IntPtr.Zero;

        try
        {
            buffer = expected.StringArrayToMultiSzPointer(out int length);
            string[] actual = buffer.MultiSzPointerToStringArray(length).ToArray();
            Assert.That(actual, Is.EqualTo(expected));
        }
        finally
        {
            if (buffer != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(buffer);
            }
        }
    }
}
