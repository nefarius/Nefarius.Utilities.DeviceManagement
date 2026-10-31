using System;
using System.Collections.Generic;

namespace Nefarius.Utilities.DeviceManagement.PnP;

/// <summary>
///     A lightweight, read-only snapshot of a device node as returned by <see cref="Devcon.EnumerateDevices" />.
/// </summary>
/// <param name="InstanceId">The device instance ID (upper-invariant).</param>
/// <param name="Description">
///     The friendly name if available, otherwise the device description, otherwise <c>null</c>.
/// </param>
/// <param name="ClassGuid">The device setup class GUID, or <see cref="Guid.Empty" /> if unavailable.</param>
/// <param name="ClassName">The device setup class name, or <c>null</c> if unavailable.</param>
/// <param name="HardwareIds">The hardware IDs, empty if the device has none.</param>
/// <param name="Service">The name of the function driver service, or <c>null</c> if none is bound.</param>
/// <param name="DriverInfPath">
///     The (published) INF name of the installed driver (e.g. <c>oem12.inf</c>), or <c>null</c> if no driver is
///     installed.
/// </param>
/// <param name="IsPresent">True if the device is currently present, false for phantom (ghost) devices.</param>
public sealed record DeviceSummary(
    string InstanceId,
    string? Description,
    Guid ClassGuid,
    string? ClassName,
    IReadOnlyList<string> HardwareIds,
    string? Service,
    string? DriverInfPath,
    bool IsPresent);