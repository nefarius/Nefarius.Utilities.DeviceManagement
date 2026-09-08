using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

using Windows.Win32;
using Windows.Win32.Devices.DeviceAndDriverInstallation;
using Windows.Win32.Devices.Properties;
using Windows.Win32.Foundation;

using Nefarius.Utilities.DeviceManagement.Exceptions;
using Nefarius.Utilities.DeviceManagement.Internal;

namespace Nefarius.Utilities.DeviceManagement.PnP;

public partial class PnPDevice
{
    internal static readonly IDictionary<DEVPROPTYPE, Type> NativeToManagedTypeMap =
        new Dictionary<DEVPROPTYPE, Type>
        {
            { DEVPROPTYPE.DEVPROP_TYPE_SBYTE, typeof(sbyte) },
            { DEVPROPTYPE.DEVPROP_TYPE_BYTE, typeof(byte) },
            { DEVPROPTYPE.DEVPROP_TYPE_INT16, typeof(short) },
            { DEVPROPTYPE.DEVPROP_TYPE_UINT16, typeof(ushort) },
            { DEVPROPTYPE.DEVPROP_TYPE_INT32, typeof(int) },
            { DEVPROPTYPE.DEVPROP_TYPE_UINT32, typeof(uint) },
            { DEVPROPTYPE.DEVPROP_TYPE_INT64, typeof(long) },
            { DEVPROPTYPE.DEVPROP_TYPE_UINT64, typeof(ulong) },
            { DEVPROPTYPE.DEVPROP_TYPE_FLOAT, typeof(float) },
            { DEVPROPTYPE.DEVPROP_TYPE_DOUBLE, typeof(double) },
            { DEVPROPTYPE.DEVPROP_TYPE_DECIMAL, typeof(decimal) },
            { DEVPROPTYPE.DEVPROP_TYPE_GUID, typeof(Guid) },
            // DEVPROP_TYPE_CURRENCY
            { DEVPROPTYPE.DEVPROP_TYPE_DATE, typeof(DateTime) },
            { DEVPROPTYPE.DEVPROP_TYPE_FILETIME, typeof(DateTimeOffset) },
            { DEVPROPTYPE.DEVPROP_TYPE_BOOLEAN, typeof(bool) },
            { DEVPROPTYPE.DEVPROP_TYPE_STRING, typeof(string) },
            { DEVPROPTYPE.DEVPROP_TYPE_STRING_LIST, typeof(string[]) },
            // DEVPROP_TYPE_SECURITY_DESCRIPTOR
            // DEVPROP_TYPE_SECURITY_DESCRIPTOR_STRING
            { DEVPROPTYPE.DEVPROP_TYPE_DEVPROPKEY, typeof(DEVPROPKEY) },
            { DEVPROPTYPE.DEVPROP_TYPE_DEVPROPTYPE, typeof(uint) },
            { DEVPROPTYPE.DEVPROP_TYPE_BINARY, typeof(byte[]) },
            { DEVPROPTYPE.DEVPROP_TYPE_ERROR, typeof(int) },
            { DEVPROPTYPE.DEVPROP_TYPE_NTSTATUS, typeof(int) }
            // DEVPROP_TYPE_STRING_INDIRECT
        };

    /// <summary>
    ///     Returns a device instance property identified by <see cref="DevicePropertyKey" />.
    /// </summary>
    /// <typeparam name="T">The managed type of the fetched property value.</typeparam>
    /// <param name="propertyKey">The <see cref="DevicePropertyKey" /> to query for.</param>
    /// <returns>On success, the value of the queried property.</returns>
    /// <remarks>If the queried property doesn't exist, the default value of the managed type is returned.</remarks>
    public T? GetProperty<T>(DevicePropertyKey propertyKey)
    {
        if (typeof(T) != propertyKey.PropertyType)
        {
            throw new ArgumentException(
                "The supplied object type doesn't match the property type.",
                nameof(propertyKey)
            );
        }

        IntPtr buffer = IntPtr.Zero;

        try
        {
            CONFIGRET ret = GetProperty(
                propertyKey.ToCsWin32Type(),
                out DEVPROPTYPE propertyType,
                out buffer,
                out uint size
            );

            if (ret == CONFIGRET.CR_NO_SUCH_VALUE
                || propertyType == 0)
            {
                return default;
            }

            if (ret == CONFIGRET.CR_BUFFER_SMALL)
            {
                throw new ConfigManagerException("The buffer supplied to a function was too small", ret);
            }

            if (ret != CONFIGRET.CR_SUCCESS)
            {
                throw new ConfigManagerException("Failed to get property.", ret);
            }

            if (!NativeToManagedTypeMap.TryGetValue(propertyType, out Type managedType))
            {
                throw new ArgumentException(
                    "Unknown property type.",
                    nameof(propertyKey)
                );
            }

            if (typeof(T) != managedType)
            {
                throw new ArgumentException(
                    "The supplied object type doesn't match the property type.",
                    nameof(propertyKey)
                );
            }

            return (T)DevicePropertyMarshal.Read(buffer, size, managedType);
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
    /// <remarks>
    ///     Passing an empty <see cref="byte" />[] deletes the property via <c>DEVPROP_TYPE_EMPTY</c>, a null buffer, and
    ///     size 0. Non-empty arrays are written as <c>DEVPROP_TYPE_BINARY</c>.
    /// </remarks>
    public unsafe void SetProperty<T>(DevicePropertyKey propertyKey, T propertyValue)
    {
        if (typeof(T) != propertyKey.PropertyType)
        {
            throw new ArgumentException(
                "The supplied object type doesn't match the property type.",
                nameof(propertyKey)
            );
        }

        Type managedType = typeof(T);

        DEVPROPKEY nativePropKey = propertyKey.ToCsWin32Type();

        DEVPROPTYPE nativePropType = NativeToManagedTypeMap.FirstOrDefault(t => t.Value == managedType).Key;

        IntPtr buffer = DevicePropertyMarshal.Write(propertyValue!, managedType, out uint propBufSize);

        if (managedType == typeof(byte[]) && propBufSize == 0)
        {
            nativePropType = DEVPROPTYPE.DEVPROP_TYPE_EMPTY;
        }

        try
        {
            CONFIGRET ret = PInvoke.CM_Set_DevNode_Property(
                _instanceHandle,
                &nativePropKey,
                nativePropType,
                buffer == IntPtr.Zero ? null : (byte*)buffer.ToPointer(),
                propBufSize,
                0
            );

            if (ret != CONFIGRET.CR_SUCCESS)
            {
                throw new ConfigManagerException("Failed to set property.", ret);
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

    private unsafe CONFIGRET GetProperty(
        DEVPROPKEY propertyKey,
        out DEVPROPTYPE propertyType,
        out IntPtr valueBuffer,
        out uint valueBufferSize
    )
    {
        valueBufferSize = 0;

        CONFIGRET ret = PInvoke.CM_Get_DevNode_Property(
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
        {
            throw new ConfigManagerException("Failed to get property size.", ret);
        }

        valueBuffer = Marshal.AllocHGlobal((int)valueBufferSize);

        ret = PInvoke.CM_Get_DevNode_Property(
            _instanceHandle,
            propertyKey,
            out propertyType,
            new Span<byte>((byte*)valueBuffer.ToPointer(), (int)valueBufferSize),
            ref valueBufferSize,
            0
        );

        if (ret == CONFIGRET.CR_SUCCESS)
        {
            return ret;
        }

        Marshal.FreeHGlobal(valueBuffer);
        valueBuffer = IntPtr.Zero;
        return ret;
    }
}