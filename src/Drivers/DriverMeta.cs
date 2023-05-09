using System;
using System.Diagnostics.CodeAnalysis;

namespace Nefarius.Utilities.DeviceManagement.Drivers;

/// <summary>
///     Driver meta data fetched from the registry.
/// </summary>
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public sealed class DriverMeta
{
    internal DriverMeta() { }

    /// <summary>
    ///     Gets the date of the driver.
    /// </summary>
    public DateTime DriverDate { get; internal set; }

    /// <summary>
    ///     Gets the description the device got from the function driver.
    /// </summary>
    public string DriverDescription { get; internal set; }

    /// <summary>
    ///     Gets the driver version.
    /// </summary>
    public Version DriverVersion { get; internal set; }

    /// <summary>
    ///     Gets the active INF name/sub-path. Typically resides in C:\Windows\INF.
    /// </summary>
    public string InfPath { get; internal set; }

    /// <summary>
    ///     Gets the section of the INF which applied on driver installation.
    /// </summary>
    public string InfSection { get; internal set; }

    /// <summary>
    ///     Gets the device ID this driver is active on.
    /// </summary>
    public string MatchingDeviceId { get; internal set; }

    /// <summary>
    ///     Gets the provider (manufacturer) name of the driver.
    /// </summary>
    public string ProviderName { get; internal set; }
}