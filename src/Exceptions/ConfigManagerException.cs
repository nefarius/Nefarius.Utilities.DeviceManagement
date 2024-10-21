using System;
using System.Diagnostics.CodeAnalysis;

using Windows.Win32.Devices.DeviceAndDriverInstallation;

namespace Nefarius.Utilities.DeviceManagement.Exceptions;

/// <summary>
///     A Configuration Manager API has failed.
/// </summary>
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public class ConfigManagerException : Exception
{
    internal ConfigManagerException(string message) : base(message)
    {
    }

    internal ConfigManagerException(string message, CONFIGRET result) : this(message)
    {
        Value = (uint)result;
    }

    /// <inheritdoc />
    public override string Message =>
        Value == (uint)CONFIGRET.CR_SUCCESS ? base.Message : $"{base.Message} (CONFIGRET: {Value})";

    /// <summary>
    ///     The CONFIGRET value of the error.
    /// </summary>
    public uint Value { get; } = (uint)CONFIGRET.CR_SUCCESS;
}