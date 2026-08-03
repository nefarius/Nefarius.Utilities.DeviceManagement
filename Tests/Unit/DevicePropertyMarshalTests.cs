using System.Runtime.InteropServices;

using Nefarius.Utilities.DeviceManagement.Internal;

namespace Tests.Unit;

[TestFixture]
[Category(TestCategories.Unit)]
public class DevicePropertyMarshalTests
{
    [Test]
    public void RoundTrip_Bool()
    {
        RoundTrip(true, typeof(bool));
        RoundTrip(false, typeof(bool));
    }

    [Test]
    public void RoundTrip_Guid()
    {
        RoundTrip(Guid.Parse("{83DA6326-97A6-4088-9453-A1923F573B29}"), typeof(Guid));
    }

    [Test]
    public void RoundTrip_SignedAndUnsignedIntegers()
    {
        RoundTrip((sbyte)-12, typeof(sbyte));
        RoundTrip((byte)200, typeof(byte));
        RoundTrip((short)-32000, typeof(short));
        RoundTrip((ushort)65000, typeof(ushort));
        RoundTrip(-42, typeof(int));
        RoundTrip(4000000000u, typeof(uint));
        RoundTrip(-9_000_000_000L, typeof(long));
        RoundTrip(18_000_000_000UL, typeof(ulong));
    }

    [Test]
    public void RoundTrip_StringAndMultiSz()
    {
        RoundTrip("Hello", typeof(string));
        RoundTrip(new[] { "HidHide", "USBPcap" }, typeof(string[]));
    }

    [Test]
    public void RoundTrip_DateTimeOffset()
    {
        DateTimeOffset value = new(2024, 6, 1, 12, 0, 0, TimeSpan.Zero);
        RoundTrip(value, typeof(DateTimeOffset));
    }

    [Test]
    public void Write_UnsupportedType_Throws()
    {
        Assert.That(
            () => DevicePropertyMarshal.Write(1.5f, typeof(float), out _),
            Throws.TypeOf<NotImplementedException>());
    }

    private static void RoundTrip(object value, Type managedType)
    {
        IntPtr buffer = DevicePropertyMarshal.Write(value, managedType, out uint size);

        try
        {
            object read = DevicePropertyMarshal.Read(buffer, size, managedType);

            if (managedType == typeof(string[]))
            {
                Assert.That(read, Is.EqualTo(value));
            }
            else if (managedType == typeof(DateTimeOffset))
            {
                Assert.That(((DateTimeOffset)read).ToFileTime(), Is.EqualTo(((DateTimeOffset)value).ToFileTime()));
            }
            else
            {
                Assert.That(read, Is.EqualTo(value));
            }
        }
        finally
        {
            Marshal.FreeHGlobal(buffer);
        }
    }
}
