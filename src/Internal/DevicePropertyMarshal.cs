using System;
using System.Linq;
using System.Runtime.InteropServices;

using Nefarius.Utilities.DeviceManagement.Util;

namespace Nefarius.Utilities.DeviceManagement.Internal;

/// <summary>
///     Converts between native DEVPROP buffers and managed property values.
/// </summary>
internal static class DevicePropertyMarshal
{
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

        if (managedType == typeof(DateTimeOffset))
        {
            return DateTimeOffset.FromFileTime(Marshal.ReadInt64(buffer));
        }

        if (managedType == typeof(Guid))
        {
            return Marshal.PtrToStructure<Guid>(buffer);
        }

        if (managedType == typeof(bool))
        {
            return Marshal.ReadByte(buffer) != 0;
        }

        throw new NotImplementedException($"Type {managedType} not supported.");
    }

    /// <summary>
    ///     Allocates and fills a native property buffer for the given managed value.
    ///     Caller must free the returned pointer with <see cref="Marshal.FreeHGlobal" />.
    /// </summary>
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

        if (managedType == typeof(bool))
        {
            propBufSize = sizeof(byte);
            IntPtr buffer = Marshal.AllocHGlobal((int)propBufSize);
            Marshal.WriteByte(buffer, (bool)propertyValue ? (byte)1 : (byte)0);
            return buffer;
        }

        throw new NotImplementedException($"Type {managedType} not supported.");
    }
}
