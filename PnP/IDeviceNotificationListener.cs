using System;

namespace Nefarius.Utilities.DeviceManagement.PnP
{
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

        void RegisterDeviceArrived(Action<DeviceEventArgs> handler, Guid? interfaceGuid = null);
        void UnregisterDeviceArrived(Action<DeviceEventArgs> handler);
        void RegisterDeviceRemoved(Action<DeviceEventArgs> handler, Guid? interfaceGuid = null);
        void UnregisterDeviceRemoved(Action<DeviceEventArgs> handler);
    }
}