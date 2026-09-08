using System.Runtime.InteropServices;

using Windows.Win32.Foundation;

using Nefarius.Utilities.DeviceManagement.Internal;
using Nefarius.Utilities.DeviceManagement.PnP;

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
    public void RoundTrip_Binary()
    {
        RoundTrip(new byte[] { 0x00, 0x01, 0x03, 0x00, 0x05 }, typeof(byte[]));
        RoundTrip(Array.Empty<byte>(), typeof(byte[]));
    }

    [Test]
    public void Write_EmptyByteArray_ReturnsNullBuffer()
    {
        IntPtr buffer = DevicePropertyMarshal.Write(Array.Empty<byte>(), typeof(byte[]), out uint size);

        Assert.Multiple(() =>
        {
            Assert.That(buffer, Is.EqualTo(IntPtr.Zero));
            Assert.That(size, Is.Zero);
        });
    }

    [Test]
    public void RoundTrip_FloatingPoint()
    {
        RoundTrip(-1.5f, typeof(float));
        RoundTrip(3.14159f, typeof(float));
        RoundTrip(-2.5d, typeof(double));
        RoundTrip(6.02214076d, typeof(double));
    }

    [Test]
    public void RoundTrip_Decimal()
    {
        RoundTrip(0m, typeof(decimal));
        RoundTrip(1234.5678m, typeof(decimal));
        RoundTrip(-1234.5678m, typeof(decimal));
    }

    [Test]
    public void RoundTrip_DateTime()
    {
        DateTime value = new(2024, 6, 1, 12, 0, 0, DateTimeKind.Unspecified);
        RoundTrip(value, typeof(DateTime));
    }

    [Test]
    public void RoundTrip_DevPropKey()
    {
        DEVPROPKEY value = new()
        {
            fmtid = Guid.Parse("{83DA6326-97A6-4088-9453-A1923F573B29}"),
            pid = 15
        };
        RoundTrip(value, typeof(DEVPROPKEY));
    }

    [Test]
    public void NativeToManagedTypeMap_IsFullySupported()
    {
        foreach (Type managedType in PnPDevice.NativeToManagedTypeMap.Values.Distinct())
        {
            Assert.That(DevicePropertyMarshal.IsSupported(managedType), Is.True,
                $"NativeToManagedTypeMap advertises {managedType} but DevicePropertyMarshal cannot convert it.");
        }
    }

    [Test]
    public void Write_UnsupportedType_Throws()
    {
        Assert.Multiple(() =>
        {
            Assert.That(
                () => DevicePropertyMarshal.Write(TimeSpan.Zero, typeof(TimeSpan), out _),
                Throws.TypeOf<NotImplementedException>());
            Assert.That(
                () => DevicePropertyMarshal.Read(IntPtr.Zero, 0, typeof(TimeSpan)),
                Throws.TypeOf<NotImplementedException>());
        });
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
            else if (managedType == typeof(DateTime))
            {
                Assert.That(((DateTime)read).ToOADate(), Is.EqualTo(((DateTime)value).ToOADate()));
            }
            else
            {
                Assert.That(read, Is.EqualTo(value));
            }
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
