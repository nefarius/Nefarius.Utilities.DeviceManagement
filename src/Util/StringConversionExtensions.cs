using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Nefarius.Utilities.DeviceManagement.Util;

[SuppressMessage("ReSharper", "UnusedMember.Global")]
[SuppressMessage("ReSharper", "InconsistentNaming")]
internal static class StringConversionExtensions
{
    public static string ToUTF8String(this byte[] buffer)
    {
        string value = Encoding.UTF8.GetString(buffer);
        return value.Remove(value.IndexOf((char)0));
    }

    public static string ToUTF16String(this byte[] buffer)
    {
        string value = Encoding.Unicode.GetString(buffer);
        return value.Remove(value.IndexOf((char)0));
    }

    /// <summary>
    ///     Converts an array of <see cref="string" /> into a double-null-terminated multi-byte character memory block.
    /// </summary>
    /// <param name="instances">Source array of strings.</param>
    /// <param name="length">The length of the resulting byte array.</param>
    /// <returns>The allocated memory buffer.</returns>
    public static IntPtr StringArrayToMultiSzPointer(this IEnumerable<string> instances, out int length)
    {
        // Temporary byte array
        IEnumerable<byte> multiSz = new List<byte>();

        // Convert each string into wide multi-byte and add NULL-terminator in between
        multiSz = instances.Aggregate(multiSz,
            (current, entry) => current.Concat(Encoding.Unicode.GetBytes(entry))
                .Concat(Encoding.Unicode.GetBytes(new[] { char.MinValue })));

        // Add another NULL-terminator to signal end of the list
        multiSz = multiSz.Concat(Encoding.Unicode.GetBytes(new[] { char.MinValue }));

        // Convert expression to array
        byte[] multiSzArray = multiSz.ToArray();

        // Convert array to managed native buffer
        IntPtr buffer = Marshal.AllocHGlobal(multiSzArray.Length);
        Marshal.Copy(multiSzArray, 0, buffer, multiSzArray.Length);

        length = multiSzArray.Length;

        // Return usable buffer, don't forget to free!
        return buffer;
    }

    /// <summary>
    ///     Converts a double-null-terminated multi-byte character memory block into a string array.
    /// </summary>
    /// <param name="buffer">The memory buffer.</param>
    /// <param name="length">The size in bytes of the memory buffer.</param>
    /// <returns>The extracted string array.</returns>
    public static IEnumerable<string> MultiSzPointerToStringArray(this IntPtr buffer, int length)
    {
        // Temporary byte array
        byte[] rawBuffer = new byte[length];

        // Grab data from buffer
        Marshal.Copy(buffer, rawBuffer, 0, length);

        // Trims away potential redundant NULL-characters and splits at NULL-terminator
        return Encoding.Unicode.GetString(rawBuffer).TrimEnd(char.MinValue).Split(char.MinValue);
    }
}