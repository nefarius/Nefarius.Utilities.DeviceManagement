using System;
using System.Runtime.InteropServices;

namespace Nefarius.Utilities.DeviceManagement.Exceptions;

/// <summary>
///     A Win32 API has failed.
/// </summary>
public class Win32Exception : Exception
{
    /// <summary>
    ///     A Win32 API has failed.
    /// </summary>
    /// <param name="message">The error message.</param>
    internal Win32Exception(string message) : base(message)
    {
    }

    /// <summary>
    ///     A Win32 API has failed.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="errorCode">The error code.</param>
    internal Win32Exception(string message, int errorCode) : this(message)
    {
    }

    /// <summary>
    ///     The native Windows error code.
    /// </summary>
    public int ErrorCode { get; set; } = Marshal.GetLastWin32Error();
}