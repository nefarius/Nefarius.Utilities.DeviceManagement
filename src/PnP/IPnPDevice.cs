#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Nefarius.Utilities.DeviceManagement.PnP;

/// <summary>
///     Describes an instance of a PNP device.
/// </summary>
[SuppressMessage("ReSharper", "UnusedMemberInSuper.Global")]
[SuppressMessage("ReSharper", "UnusedMember.Global")]
public interface IPnPDevice
{
    /// <summary>
    ///     The instance ID of the device.
    /// </summary>
    string InstanceId { get; }

    /// <summary>
    ///     The device ID.
    /// </summary>
    string DeviceId { get; }

    /// <summary>
    ///     The parent of this <see cref="IPnPDevice" />, if any.
    /// </summary>
    IPnPDevice? Parent { get; }

    /// <summary>
    ///     Siblings of this <see cref="IPnPDevice" /> sharing the same parent, if any.
    /// </summary>
    IEnumerable<IPnPDevice>? Siblings { get; }

    /// <summary>
    ///     Children of this <see cref="IPnPDevice" />, if any.
    /// </summary>
    IEnumerable<IPnPDevice>? Children { get; }

    /// <summary>
    ///     List of hardware IDs, if any.
    /// </summary>
    IEnumerable<string>? HardwareIds { get; }

    /// <summary>
    ///     List of compatible IDs, if any.
    /// </summary>
    IEnumerable<string>? CompatibleIds { get; }

    /// <summary>
    ///     Attempts to restart this device. Device restart may fail if it has open handles that currently can not be
    ///     force-closed.
    /// </summary>
    void Restart();

    /// <summary>
    ///     Attempts to remove this device node.
    /// </summary>
    /// <remarks>
    ///     This call DOES NOT invoke device and driver uninstall routines, as soon as the device is re-enumerated, it
    ///     will reappear and become online.
    /// </remarks>
    void Remove();

    /// <summary>
    ///     Walks up the <see cref="PnPDevice" />s parents chain to determine if the top most device is root enumerated.
    /// </summary>
    /// <remarks>
    ///     This is achieved by walking up the node tree until the top most parent and check if the last parent below the
    ///     tree root is a software device. Hardware devices originate from a PCI(e) bus while virtual devices originate from a
    ///     root enumerated device.
    /// </remarks>
    /// <param name="excludeIfMatches">Returns false if the given predicate is true.</param>
    /// <returns>True if this devices originates from an emulator, false otherwise.</returns>
    bool IsVirtual(Func<IPnPDevice, bool>? excludeIfMatches = default);

    /// <summary>
    ///     Installs the NULL-driver on this device instance.
    /// </summary>
    /// <remarks>
    ///     This will tear down the current device stack (no matter how many open handles exist), remove the existing function
    ///     driver and reboot the device in "raw" or "driverless" mode. Some USB devices may require a port-cycle afterwards
    ///     for the change to take effect without requiring a reboot.
    /// </remarks>
    void InstallNullDriver();

    /// <summary>
    ///     Installs the NULL-driver on this device instance.
    /// </summary>
    /// <remarks>
    ///     This will tear down the current device stack (no matter how many open handles exist), remove the existing function
    ///     driver and reboot the device in "raw" or "driverless" mode. Some USB devices may require a port-cycle afterwards
    ///     for the change to take effect without requiring a reboot.
    /// </remarks>
    void InstallNullDriver(out bool rebootRequired);

    /// <summary>
    ///     Installs a custom driver identified by the provided INF name on this device instance.
    /// </summary>
    /// <remarks>
    ///     This method force-installs a given INF file on this device instance, even if no matching hardware or compatible IDs
    ///     are found. This method can only succeed if <see cref="InstallNullDriver()" /> is called prior.
    /// </remarks>
    /// <param name="infName">
    ///     The INF file name as found in C:\Windows\INF directory. It must be the name only, not a relative
    ///     or absolute path.
    /// </param>
    void InstallCustomDriver(string infName);

    /// <summary>
    ///     Installs a custom driver identified by the provided INF name on this device instance.
    /// </summary>
    /// <remarks>
    ///     This method force-installs a given INF file on this device instance, even if no matching hardware or compatible IDs
    ///     are found. This method can only succeed if <see cref="InstallNullDriver()" /> is called prior.
    /// </remarks>
    /// <param name="infName">
    ///     The INF file name as found in C:\Windows\INF directory. It must be the name only, not a relative
    ///     or absolute path.
    /// </param>
    /// <param name="rebootRequired">
    ///     Gets whether a reboot is required for the changes to take effect or not.
    /// </param>
    void InstallCustomDriver(string infName, out bool rebootRequired);
    
    /// <summary>
    ///     Uninstalls this device instance. Unlike <see cref="Remove" /> this call will unload and revert the device function
    ///     driver to the best available compatible candidate on next device boot.
    /// </summary>
    /// <remarks>
    ///     If this is used in combination with <see cref="InstallNullDriver()" /> or
    ///     <see cref="InstallCustomDriver(string)" />, you can call <see cref="Devcon.Refresh" /> afterwards to trigger device
    ///     installation.
    /// </remarks>
    /// <exception cref="Win32Exception"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    void Uninstall();
    
    /// <summary>
    ///     Uninstalls this device instance. Unlike <see cref="Remove" /> this call will unload and revert the device function
    ///     driver to the best available compatible candidate on next device boot.
    /// </summary>
    /// <remarks>
    ///     If this is used in combination with <see cref="InstallNullDriver()" /> or
    ///     <see cref="InstallCustomDriver(string)" />, you can call <see cref="Devcon.Refresh" /> afterwards to trigger device
    ///     installation.
    /// </remarks>
    /// <param name="rebootRequired">
    ///     Gets whether a reboot is required for the changes to take effect or not.
    /// </param>
    /// <exception cref="Win32Exception"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    void Uninstall(out bool rebootRequired);

    /// <summary>
    ///     Returns a device instance property identified by <see cref="DevicePropertyKey" />.
    /// </summary>
    /// <typeparam name="T">The managed type of the fetched property value.</typeparam>
    /// <param name="propertyKey">The <see cref="DevicePropertyKey" /> to query for.</param>
    /// <returns>On success, the value of the queried property.</returns>
    /// <remarks>If the queried property doesn't exist, the default value of the managed type is returned.</remarks>
    T? GetProperty<T>(DevicePropertyKey propertyKey);

    /// <summary>
    ///     Creates or updates an existing property with a given value.
    /// </summary>
    /// <typeparam name="T">The type of the property.</typeparam>
    /// <param name="propertyKey">The <see cref="DevicePropertyKey" /> to update.</param>
    /// <param name="propertyValue">The value to set.</param>
    void SetProperty<T>(DevicePropertyKey propertyKey, T propertyValue);
}