using System;
using System.Diagnostics.CodeAnalysis;
using Windows.Win32.Devices.DeviceAndDriverInstallation;

namespace Nefarius.Utilities.DeviceManagement.Exceptions;

/// <summary>
///     A Configuration Manager API has failed.
/// </summary>
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
public class ConfigManagerException : Exception
{
    internal ConfigManagerException(string message) : base(message)
    {
    }

    internal ConfigManagerException(string message, CONFIGRET result) : this(message)
    {
        Value = (uint)result;
    }

    /// <summary>
    ///     The CONFIGRET value of the error.
    /// </summary>
    public uint Value { get; }
}