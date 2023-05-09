using System;

namespace Nefarius.Utilities.DeviceManagement.PnP;

/// <summary>
///     Utility class to listen for system-wide device arrivals and removals based on a provided device interface GUID.
/// </summary>
/// <remarks>Original source: https://gist.github.com/emoacht/73eff195317e387f4cda</remarks>
public interface IDeviceNotificationListener
{
    /// <summary>
    ///     Gets invoked when a new device has arrived (plugged in).
    /// </summary>
    event Action<DeviceEventArgs> DeviceArrived;

    /// <summary>
    ///     Gets invoked when an existing device has been removed (unplugged).
    /// </summary>
    event Action<DeviceEventArgs> DeviceRemoved;

    /// <summary>
    ///     Start listening for device arrivals/removals using the provided <see cref="Guid" />. Call this after you've
    ///     subscribed to <see cref="DeviceArrived" /> and <see cref="DeviceRemoved" /> events.
    /// </summary>
    /// <param name="interfaceGuid">The device interface GUID to listen for.</param>
    void StartListen(Guid interfaceGuid);

    /// <summary>
    ///     Stop listening. The events <see cref="DeviceArrived" /> and <see cref="DeviceRemoved" /> will not get invoked
    ///     anymore after this call. If no <see cref="Guid" /> is specified, all currently registered interfaces will get
    ///     unsubscribed.
    /// </summary>
    void StopListen(Guid? interfaceGuid = null);

    /// <summary>
    ///     Subscribe a custom event handler to device arrival events.
    /// </summary>
    /// <param name="handler">The event handler to invoke.</param>
    /// <param name="interfaceGuid">The interface GUID to get notified for or null to get notified for all listening GUIDs.</param>
    void RegisterDeviceArrived(Action<DeviceEventArgs> handler, Guid? interfaceGuid = null);

    /// <summary>
    ///     Unsubscribe a previously registered event handler.
    /// </summary>
    /// <param name="handler">The event handler to unsubscribe.</param>
    void UnregisterDeviceArrived(Action<DeviceEventArgs> handler);

    /// <summary>
    ///     Subscribe a custom event handler to device removal events.
    /// </summary>
    /// <param name="handler">The event handler to invoke.</param>
    /// <param name="interfaceGuid">The interface GUID to get notified for or null to get notified for all listening GUIDs.</param>
    void RegisterDeviceRemoved(Action<DeviceEventArgs> handler, Guid? interfaceGuid = null);

    /// <summary>
    ///     Unsubscribe a previously registered event handler.
    /// </summary>
    /// <param name="handler">The event handler to unsubscribe.</param>
    void UnregisterDeviceRemoved(Action<DeviceEventArgs> handler);
}