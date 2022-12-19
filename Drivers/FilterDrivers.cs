using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Microsoft.Win32;

namespace Nefarius.Utilities.DeviceManagement.Drivers;

/// <summary>
///     Utility class to simplify interaction with filter driver entries.
/// </summary>
[SuppressMessage("ReSharper", "UnusedMember.Global")]
public static class FilterDrivers
{
    /// <summary>
    ///     Gets the upper filter service names (if any) for a provided class GUID.
    /// </summary>
    /// <param name="classGuid">The device class GUID.</param>
    /// <returns>A list of filter service names.</returns>
    public static IEnumerable<string> GetDeviceClassUpperFilters(Guid classGuid)
    {
        RegistryKey key = Registry.LocalMachine.OpenSubKey(
            $@"SYSTEM\CurrentControlSet\Control\Class\{classGuid:B}", false);

        if (key is null)
        {
            return Enumerable.Empty<string>();
        }

        return key.GetValue("UpperFilters") is string[] filters
            ? new List<string>(filters)
            : new List<string>();
    }

    /// <summary>
    ///     Gets the lower filter service names (if any) for a provided class GUID.
    /// </summary>
    /// <param name="classGuid">The device class GUID.</param>
    /// <returns>A list of filter service names.</returns>
    public static IEnumerable<string> GetDeviceClassLowerFilters(Guid classGuid)
    {
        RegistryKey key = Registry.LocalMachine.OpenSubKey(
            $@"SYSTEM\CurrentControlSet\Control\Class\{classGuid:B}", false);

        if (key is null)
        {
            return Enumerable.Empty<string>();
        }

        return key.GetValue("LowerFilters") is string[] filters
            ? new List<string>(filters)
            : new List<string>();
    }

    /// <summary>
    ///     Removes a driver service from the upper filters of a provided class GUID.
    /// </summary>
    /// <param name="classGuid">The device class GUID.</param>
    /// <param name="serviceName">The driver service name to remove.</param>
    public static void RemoveDeviceClassUpperFilter(Guid classGuid, string serviceName)
    {
        RegistryKey key = Registry.LocalMachine.OpenSubKey(
            $@"SYSTEM\CurrentControlSet\Control\Class\{classGuid:B}", true);

        if (key is null)
        {
            return;
        }

        List<string> entries = key.GetValue("UpperFilters") is string[] filters
            ? new List<string>(filters)
            : new List<string>();

        if (!entries.Contains(serviceName, StringComparer.OrdinalIgnoreCase))
        {
            return;
        }

        entries.Remove(serviceName);
        key.SetValue("UpperFilters", entries.ToArray(), RegistryValueKind.MultiString);
    }

    /// <summary>
    ///     Removes a driver service from the lower filters of a provided class GUID.
    /// </summary>
    /// <param name="classGuid">The device class GUID.</param>
    /// <param name="serviceName">The driver service name to remove.</param>
    public static void RemoveDeviceClassLowerFilter(Guid classGuid, string serviceName)
    {
        RegistryKey key = Registry.LocalMachine.OpenSubKey(
            $@"SYSTEM\CurrentControlSet\Control\Class\{classGuid:B}", true);

        if (key is null)
        {
            return;
        }

        List<string> entries = key.GetValue("LowerFilters") is string[] filters
            ? new List<string>(filters)
            : new List<string>();

        if (!entries.Contains(serviceName, StringComparer.OrdinalIgnoreCase))
        {
            return;
        }

        entries.Remove(serviceName);
        key.SetValue("LowerFilters", entries.ToArray(), RegistryValueKind.MultiString);
    }

    /// <summary>
    ///     Adds a driver service to the upper filters of a provided class GUID.
    /// </summary>
    /// <param name="classGuid">The device class GUID.</param>
    /// <param name="serviceName">The driver service name to add.</param>
    public static void AddDeviceClassUpperFilter(Guid classGuid, string serviceName)
    {
        RegistryKey key = Registry.LocalMachine.OpenSubKey(
            $@"SYSTEM\CurrentControlSet\Control\Class\{classGuid:B}", true);

        if (key is null)
        {
            return;
        }

        List<string> entries = key.GetValue("UpperFilters") is string[] filters
            ? new List<string>(filters)
            : new List<string>();

        entries.Add(serviceName);
        key.SetValue("UpperFilters", entries.ToArray(), RegistryValueKind.MultiString);
    }

    /// <summary>
    ///     Adds a driver service to the lower filters of a provided class GUID.
    /// </summary>
    /// <param name="classGuid">The device class GUID.</param>
    /// <param name="serviceName">The driver service name to add.</param>
    public static void AddDeviceClassLowerFilter(Guid classGuid, string serviceName)
    {
        RegistryKey key = Registry.LocalMachine.OpenSubKey(
            $@"SYSTEM\CurrentControlSet\Control\Class\{classGuid:B}", true);

        if (key is null)
        {
            return;
        }

        List<string> entries = key.GetValue("LowerFilters") is string[] filters
            ? new List<string>(filters)
            : new List<string>();

        entries.Add(serviceName);
        key.SetValue("LowerFilters", entries.ToArray(), RegistryValueKind.MultiString);
    }
}