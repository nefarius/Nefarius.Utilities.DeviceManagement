using System;
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
    ///     Attempts to restart this device. Device restart may fail if it has open handles that currently can not be
    ///     force-closed.
    /// </summary>
    void Restart();

    /// <summary>
    ///     Attempts to remove this device node.
    /// </summary>
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
    bool IsVirtual(Func<IPnPDevice, bool> excludeIfMatches = default);

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
    
    void InstallCustomDriver(string infName);
    
    void InstallCustomDriver(string infName, out bool rebootRequired);

    /// <summary>
    ///     Returns a device instance property identified by <see cref="DevicePropertyKey" />.
    /// </summary>
    /// <typeparam name="T">The managed type of the fetched property value.</typeparam>
    /// <param name="propertyKey">The <see cref="DevicePropertyKey" /> to query for.</param>
    /// <returns>On success, the value of the queried property.</returns>
    T GetProperty<T>(DevicePropertyKey propertyKey);

    /// <summary>
    ///     Creates or updates an existing property with a given value.
    /// </summary>
    /// <typeparam name="T">The type of the property.</typeparam>
    /// <param name="propertyKey">The <see cref="DevicePropertyKey" /> to update.</param>
    /// <param name="propertyValue">The value to set.</param>
    void SetProperty<T>(DevicePropertyKey propertyKey, T propertyValue);
}