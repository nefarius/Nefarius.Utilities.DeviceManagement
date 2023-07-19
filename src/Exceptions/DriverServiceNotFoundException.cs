using System.Diagnostics.CodeAnalysis;

namespace Nefarius.Utilities.DeviceManagement.Exceptions;

/// <summary>
///     A driver service wasn't found on the local machine.
/// </summary>
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
[SuppressMessage("ReSharper", "UnusedMember.Global")]
public class DriverServiceNotFoundException : Win32Exception
{
    /// <inheritdoc />
    internal DriverServiceNotFoundException(string message) : base(message)
    {
    }

    internal DriverServiceNotFoundException(string message, int errorCode) : base(message, errorCode)
    {
    }
}