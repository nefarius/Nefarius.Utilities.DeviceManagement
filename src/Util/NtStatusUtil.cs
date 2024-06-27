using System;
using System.Runtime.InteropServices;
using System.Threading;

using Windows.Win32;
using Windows.Win32.Foundation;

namespace Nefarius.Utilities.DeviceManagement.Util;

/// <summary>
///     Utility methods for handling NTSTATUS values.
/// </summary>
public static class NtStatusUtil
{
    /// <summary>
    ///     Converts an NTSTATUS value to a <see cref="WIN32_ERROR" />.
    /// </summary>
    /// <remarks>https://stackoverflow.com/a/32205631</remarks>
    /// <param name="ntStatus">The NTSTATUS value to convert.</param>
    /// <returns>The converted Win32 error code.</returns>
    public static int ConvertNtStatusToWin32Error(uint ntStatus)
    {
        NativeOverlapped ol = new() { InternalLow = (IntPtr)ntStatus };
        int oldError = Marshal.GetLastWin32Error();
        PInvoke.GetOverlappedResult(null, ol, out uint _, new BOOL(false));
        int result = Marshal.GetLastWin32Error();
        PInvoke.SetLastError((WIN32_ERROR)oldError);
        return result;
    }
}