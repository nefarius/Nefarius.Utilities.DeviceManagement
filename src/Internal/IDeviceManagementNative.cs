using System;
using System.Collections.Generic;

namespace Nefarius.Utilities.DeviceManagement.Internal;

/// <summary>
///     Minimal native device-management operations used by high-value library logic.
///     Replaceable in tests via <see cref="DeviceManagementNative" />.
/// </summary>
internal interface IDeviceManagementNative
{
    /// <summary>
    ///     Enumerates instance IDs for devices in the given SetupAPI device class.
    /// </summary>
    /// <param name="classGuid">The device class GUID.</param>
    /// <param name="presentOnly">True to only include currently present devices.</param>
    /// <returns>Instance IDs (typically upper-invariant).</returns>
    IEnumerable<string> EnumerateDeviceInstanceIds(Guid classGuid, bool presentOnly);

    /// <summary>
    ///     Gets the hardware IDs for a device instance, or null if unavailable.
    /// </summary>
    /// <param name="instanceId">The device instance ID.</param>
    /// <param name="presentOnly">True when the caller is restricting to present devices.</param>
    string[]? GetHardwareIds(string instanceId, bool presentOnly);

    /// <summary>
    ///     Gets the parent instance ID for a device, or null/empty if none.
    /// </summary>
    /// <param name="instanceId">The device instance ID.</param>
    string? GetParentInstanceId(string instanceId);
}
