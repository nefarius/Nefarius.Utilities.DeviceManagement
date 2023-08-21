#nullable enable
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
public class Win32Exception : Exception
{
    /// <summary>
    ///     A Win32 API has failed.
    /// </summary>
    /// <param name="message">The error message.</param>
    internal unsafe Win32Exception(string message) : base(message)
    {
        ErrorCode ??= Marshal.GetLastWin32Error();

        const int bufLen = 1024;
        char* buffer = stackalloc char[bufLen];

        uint numChars = PInvoke.FormatMessage(
            FORMAT_MESSAGE_OPTIONS.FORMAT_MESSAGE_FROM_SYSTEM |
            FORMAT_MESSAGE_OPTIONS.FORMAT_MESSAGE_IGNORE_INSERTS,
            null,
            (uint)ErrorCode,
            0,
            buffer,
            bufLen,
            null
        );

        if (numChars > 0)
        {
            ErrorMessage = new string(buffer).Trim('\r', '\n');
        }
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
}