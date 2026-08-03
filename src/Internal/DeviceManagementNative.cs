using System;

namespace Nefarius.Utilities.DeviceManagement.Internal;

/// <summary>
///     Ambient holder for <see cref="IDeviceManagementNative" />. Defaults to the real Windows implementation.
/// </summary>
internal static class DeviceManagementNative
{
    private static IDeviceManagementNative _current = RealDeviceManagementNative.Instance;

    /// <summary>
    ///     The active native implementation.
    /// </summary>
    public static IDeviceManagementNative Current
    {
        get => _current;
        set => _current = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary>
    ///     Restores the real Windows implementation.
    /// </summary>
    public static void ResetToReal()
    {
        _current = RealDeviceManagementNative.Instance;
    }
}
