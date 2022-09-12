using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Windows.Win32;
using Windows.Win32.Devices.DeviceAndDriverInstallation;
using Windows.Win32.Devices.Properties;
using Nefarius.Utilities.DeviceManagement.Exceptions;
using Nefarius.Utilities.DeviceManagement.Util;

namespace Nefarius.Utilities.DeviceManagement.PnP;

public partial class PnPDevice
{
    private static readonly IDictionary<uint, Type> NativeToManagedTypeMap =
        new Dictionary<uint, Type>
        {
            { PInvoke.DEVPROP_TYPE_SBYTE, typeof(sbyte) },
            { PInvoke.DEVPROP_TYPE_BYTE, typeof(byte) },
            { PInvoke.DEVPROP_TYPE_INT16, typeof(short) },
            { PInvoke.DEVPROP_TYPE_UINT16, typeof(ushort) },
            { PInvoke.DEVPROP_TYPE_INT32, typeof(int) },
            { PInvoke.DEVPROP_TYPE_UINT32, typeof(uint) },
            { PInvoke.DEVPROP_TYPE_INT64, typeof(long) },
            { PInvoke.DEVPROP_TYPE_UINT64, typeof(ulong) },
            { PInvoke.DEVPROP_TYPE_FLOAT, typeof(float) },
            { PInvoke.DEVPROP_TYPE_DOUBLE, typeof(double) },
            { PInvoke.DEVPROP_TYPE_DECIMAL, typeof(decimal) },
            { PInvoke.DEVPROP_TYPE_GUID, typeof(Guid) },
            // DEVPROP_TYPE_CURRENCY
            { PInvoke.DEVPROP_TYPE_DATE, typeof(DateTime) },
            { PInvoke.DEVPROP_TYPE_FILETIME, typeof(DateTimeOffset) },
            { PInvoke.DEVPROP_TYPE_BOOLEAN, typeof(bool) },
            { PInvoke.DEVPROP_TYPE_STRING, typeof(string) },
            { PInvoke.DEVPROP_TYPE_STRING | PInvoke.DEVPROP_TYPEMOD_LIST, typeof(string[]) },
            // DEVPROP_TYPE_SECURITY_DESCRIPTOR
            // DEVPROP_TYPE_SECURITY_DESCRIPTOR_STRING
            { PInvoke.DEVPROP_TYPE_DEVPROPKEY, typeof(DEVPROPKEY) },
            { PInvoke.DEVPROP_TYPE_DEVPROPTYPE, typeof(uint) },
            { PInvoke.DEVPROP_TYPE_BYTE | PInvoke.DEVPROP_TYPEMOD_ARRAY, typeof(byte[]) },
            { PInvoke.DEVPROP_TYPE_ERROR, typeof(int) },
            { PInvoke.DEVPROP_TYPE_NTSTATUS, typeof(int) }
            // DEVPROP_TYPE_STRING_INDIRECT
        };

    /// <summary>
    ///     Returns a device instance property identified by <see cref="DevicePropertyKey" />.
    /// </summary>
    /// <typeparam name="T">The managed type of the fetched porperty value.</typeparam>
    /// <param name="propertyKey">The <see cref="DevicePropertyKey" /> to query for.</param>
    /// <returns>On success, the value of the queried property.</returns>
    public T GetProperty<T>(DevicePropertyKey propertyKey)
    {
        if (typeof(T) != propertyKey.PropertyType)
            throw new ArgumentException(
                "The supplied object type doesn't match the property type.",
                nameof(propertyKey)
            );

        var buffer = IntPtr.Zero;

        try
        {
            var ret = GetProperty(
                propertyKey.ToCsWin32Type(),
                out var propertyType,
                out buffer,
                out var size
            );

            if (ret == CONFIGRET.CR_NO_SUCH_VALUE
                || propertyType == 0)
                return default;

            if (ret == CONFIGRET.CR_BUFFER_SMALL)
                throw new ConfigManagerException("The buffer supplied to a function was too small", ret);

            if (ret != CONFIGRET.CR_SUCCESS)
                throw new ConfigManagerException("Failed to get property.", ret);

            if (!NativeToManagedTypeMap.TryGetValue(propertyType, out var managedType))
                throw new ArgumentException(
                    "Unknown property type.",
                    nameof(propertyKey)
                );

            if (typeof(T) != managedType)
                throw new ArgumentException(
                    "The supplied object type doesn't match the property type.",
                    nameof(propertyKey)
                );

            #region Don't look, nasty trickery

            /*
             * Handle some native to managed conversions
             */

            // Regular strings
            if (managedType == typeof(string))
            {
                var value = Marshal.PtrToStringUni(buffer);
                return (T)Convert.ChangeType(value, typeof(T));
            }

            // Double-null-terminated string to list
            if (managedType == typeof(string[]))
                return (T)(object)Marshal.PtrToStringUni(buffer, (int)size / 2).TrimEnd('\0').Split('\0')
                    .ToArray();

            // Byte & SByte
            if (managedType == typeof(sbyte)
                || managedType == typeof(byte))
                return (T)(object)Marshal.ReadByte(buffer);

            // (U)Int16
            if (managedType == typeof(short)
                || managedType == typeof(ushort))
                return (T)(object)(ushort)Marshal.ReadInt16(buffer);

            // (U)Int32
            if (managedType == typeof(int)
                || managedType == typeof(uint))
                return (T)Convert.ChangeType(Marshal.ReadInt32(buffer), managedType);

            // (U)Int64
            if (managedType == typeof(long)
                || managedType == typeof(ulong))
                return (T)(object)(ulong)Marshal.ReadInt64(buffer);

            // FILETIME/DateTimeOffset
            if (managedType == typeof(DateTimeOffset))
                return (T)(object)DateTimeOffset.FromFileTime(Marshal.ReadInt64(buffer));

            // GUID
            if (managedType == typeof(Guid))
                return (T)(object)Marshal.PtrToStructure<Guid>(buffer);

            #endregion

            throw new NotImplementedException("Type not supported.");
        }
        finally
        {
            Marshal.FreeHGlobal(buffer);
        }
    }

    /// <summary>
    ///     Creates or updates an existing property with a given value.
    /// </summary>
    /// <typeparam name="T">The type of the property.</typeparam>
    /// <param name="propertyKey">The <see cref="DevicePropertyKey" /> to update.</param>
    /// <param name="propertyValue">The value to set.</param>
    public unsafe void SetProperty<T>(DevicePropertyKey propertyKey, T propertyValue)
    {
        if (typeof(T) != propertyKey.PropertyType)
            throw new ArgumentException(
                "The supplied object type doesn't match the property type.",
                nameof(propertyKey)
            );

        var managedType = typeof(T);

        var nativePropKey = propertyKey.ToCsWin32Type();

        var nativePropType = NativeToManagedTypeMap.FirstOrDefault(t => t.Value == managedType).Key;

        uint propBufSize = 0;

        var buffer = IntPtr.Zero;

        #region Don't look, nasty trickery

        /*
         * Handle some native to managed conversions
         */

        // Regular strings
        if (managedType == typeof(string))
        {
            var value = (string)(object)propertyValue;
            buffer = Marshal.StringToHGlobalUni(value);
            propBufSize = (uint)((value.Length + 1) * 2);
        }

        // Double-null-terminated string to list
        if (managedType == typeof(string[]))
        {
            var value = (string[])(object)propertyValue;
            buffer = value.StringArrayToMultiSzPointer(out var length);
            propBufSize = (uint)length;
        }

        // Byte & SByte
        if (managedType == typeof(sbyte)
            || managedType == typeof(byte))
        {
            var value = (byte)(object)propertyValue;
            propBufSize = (uint)Marshal.SizeOf(managedType);
            buffer = Marshal.AllocHGlobal((int)propBufSize);
            Marshal.WriteByte(buffer, value);
        }

        /*
        // (U)Int16
        if (managedType == typeof(short)
            || managedType == typeof(ushort))
            return (T) (object) (ushort) Marshal.ReadInt16(buffer);
        */
        // (U)Int32
        if (managedType == typeof(int)
            || managedType == typeof(uint))
        {
            var value = (uint)(object)propertyValue;
            propBufSize = (uint)Marshal.SizeOf(managedType);
            buffer = Marshal.AllocHGlobal((int)propBufSize);
            Marshal.WriteInt32(buffer, (int)value);
        }
        /*
        // (U)Int64
        if (managedType == typeof(long)
            || managedType == typeof(ulong))
            return (T) (object) (ulong) Marshal.ReadInt64(buffer);

        // FILETIME/DateTimeOffset
        if (managedType == typeof(DateTimeOffset))
            return (T) (object) DateTimeOffset.FromFileTime(Marshal.ReadInt64(buffer));
        */

        if (managedType == typeof(Guid))
        {
            var value = (Guid)(object)propertyValue;
            Marshal.StructureToPtr(value, buffer, false);
            propBufSize = (uint)Marshal.SizeOf(managedType);
        }

        #endregion

        if (buffer == IntPtr.Zero)
            throw new NotImplementedException("Type not supported.");

        try
        {
            var ret = PInvoke.CM_Set_DevNode_Property(
                _instanceHandle,
                &nativePropKey,
                nativePropType,
                (byte*)buffer.ToPointer(),
                propBufSize,
                0
            );

            if (ret != CONFIGRET.CR_SUCCESS)
                throw new ConfigManagerException("Failed to set property.", ret);
        }
        finally
        {
            Marshal.FreeHGlobal(buffer);
        }
    }

    private unsafe CONFIGRET GetProperty(
        DEVPROPKEY propertyKey,
        out uint propertyType,
        out IntPtr valueBuffer,
        out uint valueBufferSize
    )
    {
        valueBufferSize = 0;

        var ret = PInvoke.CM_Get_DevNode_Property(
            _instanceHandle,
            propertyKey,
            out _,
            null,
            ref valueBufferSize,
            0
        );

        if (ret == CONFIGRET.CR_NO_SUCH_VALUE)
        {
            propertyType = 0;
            valueBuffer = IntPtr.Zero;
            return ret;
        }

        if (ret != CONFIGRET.CR_BUFFER_SMALL)
            throw new ConfigManagerException("Failed to get property size.", ret);

        valueBuffer = Marshal.AllocHGlobal((int)valueBufferSize);

        ret = PInvoke.CM_Get_DevNode_Property(
            _instanceHandle,
            propertyKey,
            out propertyType,
            (byte*)valueBuffer.ToPointer(),
            ref valueBufferSize,
            0
        );

        if (ret == CONFIGRET.CR_SUCCESS) return ret;

        Marshal.FreeHGlobal(valueBuffer);
        valueBuffer = IntPtr.Zero;
        return ret;
    }
}