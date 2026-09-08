using System;
using System.Linq;
using System.Runtime.InteropServices;

using Windows.Win32.Foundation;

using Nefarius.Utilities.DeviceManagement.Util;

namespace Nefarius.Utilities.DeviceManagement.Internal;

/// <summary>
///     Converts between native DEVPROP buffers and managed property values.
/// </summary>
internal static class DevicePropertyMarshal
{
    /// <summary>
    ///     Returns whether <paramref name="managedType" /> can be converted by <see cref="Read" /> and <see cref="Write" />.
    /// </summary>
    internal static bool IsSupported(Type managedType)
    {
        return managedType == typeof(string)
               || managedType == typeof(string[])
               || managedType == typeof(sbyte)
               || managedType == typeof(byte)
               || managedType == typeof(short)
               || managedType == typeof(ushort)
               || managedType == typeof(int)
               || managedType == typeof(uint)
               || managedType == typeof(long)
               || managedType == typeof(ulong)
               || managedType == typeof(float)
               || managedType == typeof(double)
               || managedType == typeof(decimal)
               || managedType == typeof(DateTime)
               || managedType == typeof(DateTimeOffset)
               || managedType == typeof(Guid)
               || managedType == typeof(bool)
               || managedType == typeof(byte[])
               || managedType == typeof(DEVPROPKEY);
    }

    /// <summary>
    ///     Reads a managed value from a native property buffer.
    /// </summary>
    public static object Read(IntPtr buffer, uint size, Type managedType)
    {
        if (managedType == typeof(string))
        {
            return Marshal.PtrToStringUni(buffer) ?? string.Empty;
        }

        if (managedType == typeof(string[]))
        {
            return buffer.MultiSzPointerToStringArray((int)size).ToArray();
        }

        if (managedType == typeof(sbyte))
        {
            return (sbyte)Marshal.ReadByte(buffer);
        }

        if (managedType == typeof(byte))
        {
            return Marshal.ReadByte(buffer);
        }

        if (managedType == typeof(short))
        {
            return Marshal.ReadInt16(buffer);
        }

        if (managedType == typeof(ushort))
        {
            return (ushort)Marshal.ReadInt16(buffer);
        }

        if (managedType == typeof(int))
        {
            return Marshal.ReadInt32(buffer);
        }

        if (managedType == typeof(uint))
        {
            return (uint)Marshal.ReadInt32(buffer);
        }

        if (managedType == typeof(long))
        {
            return Marshal.ReadInt64(buffer);
        }

        if (managedType == typeof(ulong))
        {
            return (ulong)Marshal.ReadInt64(buffer);
        }

        if (managedType == typeof(float))
        {
            return BitConverter.ToSingle(ReadBytes(buffer, sizeof(float)), 0);
        }

        if (managedType == typeof(double))
        {
            return BitConverter.ToDouble(ReadBytes(buffer, sizeof(double)), 0);
        }

        if (managedType == typeof(decimal))
        {
            // OLE DECIMAL: wReserved(2) scale(1) sign(1) Hi32(4) Lo64(8).
            byte scale = Marshal.ReadByte(buffer, 2);
            byte sign = Marshal.ReadByte(buffer, 3);
            int hi = Marshal.ReadInt32(buffer, 4);
            long lo64 = Marshal.ReadInt64(buffer, 8);

            return new decimal((int)(lo64 & 0xFFFFFFFF), (int)(lo64 >> 32), hi, (sign & 0x80) != 0, scale);
        }

        if (managedType == typeof(DateTime))
        {
            return DateTime.FromOADate(BitConverter.ToDouble(ReadBytes(buffer, sizeof(double)), 0));
        }

        if (managedType == typeof(DateTimeOffset))
        {
            return DateTimeOffset.FromFileTime(Marshal.ReadInt64(buffer));
        }

        if (managedType == typeof(Guid))
        {
            return Marshal.PtrToStructure<Guid>(buffer);
        }

        if (managedType == typeof(DEVPROPKEY))
        {
            return Marshal.PtrToStructure<DEVPROPKEY>(buffer);
        }

        if (managedType == typeof(bool))
        {
            return Marshal.ReadByte(buffer) != 0;
        }

        if (managedType == typeof(byte[]))
        {
            if (size == 0)
            {
                return Array.Empty<byte>();
            }

            return ReadBytes(buffer, (int)size);
        }

        throw new NotImplementedException($"Type {managedType} not supported.");
    }

    /// <summary>
    ///     Allocates and fills a native property buffer for the given managed value.
    ///     Caller must free the returned pointer with <see cref="Marshal.FreeHGlobal" />.
    /// </summary>
    /// <remarks>
    ///     A zero-length <see cref="byte" />[] returns <see cref="IntPtr.Zero" /> and size 0. Combined with
    ///     <c>DEVPROP_TYPE_EMPTY</c> that is the native contract for deleting the property.
    /// </remarks>
    public static IntPtr Write(object propertyValue, Type managedType, out uint propBufSize)
    {
        if (managedType == typeof(string))
        {
            string value = (string)propertyValue;
            IntPtr buffer = Marshal.StringToHGlobalUni(value);
            propBufSize = (uint)((value.Length + 1) * 2);
            return buffer;
        }

        if (managedType == typeof(string[]))
        {
            string[] value = (string[])propertyValue;
            IntPtr buffer = value.StringArrayToMultiSzPointer(out int length);
            propBufSize = (uint)length;
            return buffer;
        }

        if (managedType == typeof(sbyte))
        {
            propBufSize = sizeof(sbyte);
            IntPtr buffer = Marshal.AllocHGlobal((int)propBufSize);
            Marshal.WriteByte(buffer, (byte)(sbyte)propertyValue);
            return buffer;
        }

        if (managedType == typeof(byte))
        {
            propBufSize = sizeof(byte);
            IntPtr buffer = Marshal.AllocHGlobal((int)propBufSize);
            Marshal.WriteByte(buffer, (byte)propertyValue);
            return buffer;
        }

        if (managedType == typeof(short))
        {
            propBufSize = sizeof(short);
            IntPtr buffer = Marshal.AllocHGlobal((int)propBufSize);
            Marshal.WriteInt16(buffer, (short)propertyValue);
            return buffer;
        }

        if (managedType == typeof(ushort))
        {
            propBufSize = sizeof(ushort);
            IntPtr buffer = Marshal.AllocHGlobal((int)propBufSize);
            Marshal.WriteInt16(buffer, (short)(ushort)propertyValue);
            return buffer;
        }

        if (managedType == typeof(int))
        {
            propBufSize = sizeof(int);
            IntPtr buffer = Marshal.AllocHGlobal((int)propBufSize);
            Marshal.WriteInt32(buffer, (int)propertyValue);
            return buffer;
        }

        if (managedType == typeof(uint))
        {
            propBufSize = sizeof(uint);
            IntPtr buffer = Marshal.AllocHGlobal((int)propBufSize);
            Marshal.WriteInt32(buffer, (int)(uint)propertyValue);
            return buffer;
        }

        if (managedType == typeof(long))
        {
            propBufSize = sizeof(long);
            IntPtr buffer = Marshal.AllocHGlobal((int)propBufSize);
            Marshal.WriteInt64(buffer, (long)propertyValue);
            return buffer;
        }

        if (managedType == typeof(ulong))
        {
            propBufSize = sizeof(ulong);
            IntPtr buffer = Marshal.AllocHGlobal((int)propBufSize);
            Marshal.WriteInt64(buffer, (long)(ulong)propertyValue);
            return buffer;
        }

        if (managedType == typeof(float))
        {
            return WriteBytes(BitConverter.GetBytes((float)propertyValue), out propBufSize);
        }

        if (managedType == typeof(double))
        {
            return WriteBytes(BitConverter.GetBytes((double)propertyValue), out propBufSize);
        }

        if (managedType == typeof(decimal))
        {
            decimal value = (decimal)propertyValue;
            int[] bits = decimal.GetBits(value);
            int lo = bits[0];
            int mid = bits[1];
            int hi = bits[2];
            byte scale = (byte)((bits[3] >> 16) & 0xFF);
            byte sign = (bits[3] & unchecked((int)0x80000000)) != 0 ? (byte)0x80 : (byte)0;

            propBufSize = 16;
            IntPtr buffer = Marshal.AllocHGlobal((int)propBufSize);
            Marshal.WriteInt16(buffer, 0, 0);
            Marshal.WriteByte(buffer, 2, scale);
            Marshal.WriteByte(buffer, 3, sign);
            Marshal.WriteInt32(buffer, 4, hi);
            Marshal.WriteInt64(buffer, 8, ((long)mid << 32) | (uint)lo);
            return buffer;
        }

        if (managedType == typeof(DateTime))
        {
            DateTime value = (DateTime)propertyValue;
            return WriteBytes(BitConverter.GetBytes(value.ToOADate()), out propBufSize);
        }

        if (managedType == typeof(DateTimeOffset))
        {
            DateTimeOffset value = (DateTimeOffset)propertyValue;
            propBufSize = sizeof(long);
            IntPtr buffer = Marshal.AllocHGlobal((int)propBufSize);
            Marshal.WriteInt64(buffer, value.ToFileTime());
            return buffer;
        }

        if (managedType == typeof(Guid))
        {
            Guid value = (Guid)propertyValue;
            propBufSize = (uint)Marshal.SizeOf<Guid>();
            IntPtr buffer = Marshal.AllocHGlobal((int)propBufSize);
            Marshal.StructureToPtr(value, buffer, false);
            return buffer;
        }

        if (managedType == typeof(DEVPROPKEY))
        {
            DEVPROPKEY value = (DEVPROPKEY)propertyValue;
            propBufSize = (uint)Marshal.SizeOf<DEVPROPKEY>();
            IntPtr buffer = Marshal.AllocHGlobal((int)propBufSize);
            Marshal.StructureToPtr(value, buffer, false);
            return buffer;
        }

        if (managedType == typeof(bool))
        {
            propBufSize = sizeof(byte);
            IntPtr buffer = Marshal.AllocHGlobal((int)propBufSize);
            Marshal.WriteByte(buffer, (bool)propertyValue ? (byte)1 : (byte)0);
            return buffer;
        }

        if (managedType == typeof(byte[]))
        {
            byte[] value = (byte[])propertyValue;
            if (value.Length == 0)
            {
                propBufSize = 0;
                return IntPtr.Zero;
            }

            return WriteBytes(value, out propBufSize);
        }

        throw new NotImplementedException($"Type {managedType} not supported.");
    }

    private static byte[] ReadBytes(IntPtr buffer, int length)
    {
        byte[] value = new byte[length];
        Marshal.Copy(buffer, value, 0, length);
        return value;
    }

    private static IntPtr WriteBytes(byte[] bytes, out uint propBufSize)
    {
        propBufSize = (uint)bytes.Length;
        IntPtr buffer = Marshal.AllocHGlobal(bytes.Length == 0 ? 1 : bytes.Length);
        if (bytes.Length > 0)
        {
            Marshal.Copy(bytes, 0, buffer, bytes.Length);
        }

        return buffer;
    }
}
