#nullable enable
using System;
using System.Diagnostics.CodeAnalysis;

namespace Nefarius.Utilities.DeviceManagement.Exceptions;

/// <summary>
///     The desired device instance was not found on the system.
/// </summary>
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public class PnPDeviceNotFoundException : Exception
{
    internal PnPDeviceNotFoundException() : base("The desired device instance was not found on the system.")
    {
    }
    
    internal PnPDeviceNotFoundException(string instanceId) : this()
    {
        InstanceId = instanceId;
    }

    /// <summary>
    ///     The instance ID of the device queried for.
    /// </summary>
    public string? InstanceId { get; set; }
}