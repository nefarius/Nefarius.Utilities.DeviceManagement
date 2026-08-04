using System;
using System.Globalization;

using Microsoft.Win32;

using Nefarius.Utilities.DeviceManagement.Drivers;
using Nefarius.Utilities.DeviceManagement.PnP;

namespace Nefarius.Utilities.DeviceManagement.Extensions;

/// <summary>
///     Helper methods for <see cref="PnPDevice" /> objects.
/// </summary>
public static class PnPDeviceExtensions
{
    /// <summary>
    ///     Creates a <see cref="UsbPnPDevice" /> from the provided <see cref="PnPDevice" />.
    /// </summary>
    /// <param name="device">The <see cref="PnPDevice" /> to base this USB device on.</param>
    /// <returns>The new <see cref="UsbPnPDevice" />.</returns>
    public static UsbPnPDevice ToUsbPnPDevice(this PnPDevice device)
    {
        return new UsbPnPDevice(device.InstanceId, DeviceLocationFlags.Phantom);
    }

    /// <summary>
    ///     Fetches meta data about the currently active driver of the <see cref="PnPDevice" />.
    /// </summary>
    /// <param name="device">The <see cref="PnPDevice" /> to fetch driver info for.</param>
    /// <returns>The <see cref="DriverMeta" /> instance.</returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if registry access failed or required driver values are missing/malformed.
    /// </exception>
    public static DriverMeta GetCurrentDriver(this PnPDevice device)
    {
        string driverKey = device.GetProperty<string>(DevicePropertyKey.Device_Driver);

        using RegistryKey key =
            Registry.LocalMachine.OpenSubKey($@"SYSTEM\CurrentControlSet\Control\Class\{driverKey}");

        if (key is null)
        {
            throw new InvalidOperationException("Failed to open driver registry key.");
        }

        string? driverDate = key.GetValue("DriverDate") as string;
        string? driverVersion = key.GetValue("DriverVersion") as string;

        if (string.IsNullOrWhiteSpace(driverDate) || string.IsNullOrWhiteSpace(driverVersion))
        {
            throw new InvalidOperationException("Driver registry key is missing DriverDate or DriverVersion.");
        }

        try
        {
            return new DriverMeta
            {
                DriverDate =
                    DateTime.ParseExact(driverDate, "M-d-yyyy", CultureInfo.InvariantCulture),
                DriverDescription = key.GetValue("DriverDesc") as string,
                DriverVersion = Version.Parse(driverVersion),
                InfPath = key.GetValue("InfPath") as string,
                InfSection = key.GetValue("InfSection") as string,
                MatchingDeviceId = key.GetValue("MatchingDeviceId") as string,
                ProviderName = key.GetValue("ProviderName") as string
            };
        }
        catch (Exception ex) when (ex is FormatException or ArgumentException or OverflowException)
        {
            throw new InvalidOperationException("Driver registry key contains malformed DriverDate or DriverVersion.",
                ex);
        }
    }
}
