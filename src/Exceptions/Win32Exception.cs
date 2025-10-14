using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

using Windows.Win32;
using Windows.Win32.System.Diagnostics.Debug;

namespace Nefarius.Utilities.DeviceManagement.Exceptions;

/// <summary>
///     A Win32 API has failed.
/// </summary>
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
[SuppressMessage("ReSharper", "UnusedMember.Global")]
public class Win32Exception : Exception
{
    /// <summary>
    ///     A Win32 API has failed.
    /// </summary>
    /// <param name="message">The error message.</param>
    internal Win32Exception(string message) : base(message)
    {
        ErrorCode ??= Marshal.GetLastWin32Error();
        ErrorMessage = GetMessageFor(ErrorCode);
    }

    /// <summary>
    ///     A Win32 API has failed.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="errorCode">The error code.</param>
    internal Win32Exception(string message, int errorCode) : this(message)
    {
        ErrorCode = errorCode;
    }

    /// <summary>
    ///     The native Windows error code.
    /// </summary>
    public int? ErrorCode { get; }

    /// <summary>
    ///     The Win32 error message.
    /// </summary>
    public string? ErrorMessage { get; }

    /// <summary>
    ///     Translates a Win32 error code to the user-readable message.
    /// </summary>
    /// <param name="errorCode">The Win32 error code. Gets fetched from <see cref="Marshal.GetLastWin32Error" /> if null.</param>
    /// <returns>The message, if any, or null.</returns>
    public static unsafe string? GetMessageFor(int? errorCode = default)
    {
        errorCode ??= Marshal.GetLastWin32Error();

        const int bufLen = 1024;
        char* buffer = stackalloc char[bufLen];

        uint numChars = PInvoke.FormatMessage(
            FORMAT_MESSAGE_OPTIONS.FORMAT_MESSAGE_FROM_SYSTEM |
            FORMAT_MESSAGE_OPTIONS.FORMAT_MESSAGE_IGNORE_INSERTS,
            null,
            (uint)errorCode.Value,
            0,
            buffer,
            bufLen,
            null
        );

        return numChars > 0 ? new string(buffer).Trim('\r', '\n') : null;
    }
}