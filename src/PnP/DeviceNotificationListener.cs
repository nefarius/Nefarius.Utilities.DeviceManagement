using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;

using Nefarius.Utilities.DeviceManagement.Exceptions;

namespace Nefarius.Utilities.DeviceManagement.PnP;

/// <summary>
///     Utility class to listen for system-wide device arrivals and removals based on a provided device interface GUID.
/// </summary>
/// <remarks>Original source: https://gist.github.com/emoacht/73eff195317e387f4cda</remarks>
public sealed class DeviceNotificationListener : IDeviceNotificationListener, IDisposable
{
    private readonly List<DeviceEventRegistration> _arrivedRegistrations = new();
    private readonly object _sync = new();

    private readonly List<ListenerItem> _listeners = new();
    private readonly List<DeviceEventRegistration> _removedRegistrations = new();
    private bool _disposed;

    /// <summary>
    ///     Gets invoked when a new device has arrived (plugged in).
    /// </summary>
    public event Action<DeviceEventArgs> DeviceArrived;

    /// <summary>
    ///     Gets invoked when an existing device has been removed (unplugged).
    /// </summary>
    public event Action<DeviceEventArgs> DeviceRemoved;

    /// <inheritdoc />
    public void Dispose()
    {
        // Dispose of unmanaged resources.
        Dispose(true);
        // Suppress finalization.
        GC.SuppressFinalize(this);
    }

    #region Util

    private static string GenerateRandomString()
    {
        // Creating object of random class
        Random rand = new();

        // Choosing the size of string
        // Using Next() string
        int stringlen = rand.Next(4, 10);
        int randValue;
        StringBuilder sb = new();
        char letter;
        for (int i = 0; i < stringlen; i++)
        {
            // Generating a random number.
            randValue = rand.Next(0, 26);

            // Generating random character by converting
            // the random number into character.
            letter = Convert.ToChar(randValue + 65);

            // Appending the letter to string.
            sb.Append(letter);
        }

        return sb.ToString();
    }

    #endregion

    private void Dispose(bool disposing)
    {
        if (!disposing)
        {
            return;
        }

        lock (_sync)
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
        }

        StopListenCore(null);

        lock (_sync)
        {
            _arrivedRegistrations.Clear();
            _removedRegistrations.Clear();
        }
    }

    private class ListenerItem
    {
        public Guid InterfaceGuid { get; set; }
        public Thread Thread { get; set; }
        public HWND WindowHandle { get; set; }
        public HDEVNOTIFY NotificationHandle { get; set; }
        public CancellationTokenSource Cancellation { get; } = new();
        public ManualResetEventSlim StartupCompleted { get; } = new(false);
        public Exception? StartupException { get; set; }
        public string? ClassName { get; set; }
        public HMODULE ModuleHandle { get; set; }
        /// <summary>
        ///     Keeps the native window procedure reachable until the window class is unregistered.
        /// </summary>
        public WNDPROC? WindowProc { get; set; }
        /// <summary>
        ///     True after <see cref="Thread.Join(TimeSpan)" /> timed out; excluded from active GUID checks
        ///     so a replacement listener may start while this thread still winds down.
        /// </summary>
        public bool IsStopping { get; set; }
    }

    private class DeviceEventRegistration
    {
        public Action<DeviceEventArgs> Handler { get; set; }
        public Guid InterfaceGuid { get; set; }
    }

    #region Registration

    /// <summary>
    ///     Subscribe a custom event handler to device arrival events.
    /// </summary>
    /// <param name="handler">The event handler to invoke.</param>
    /// <param name="interfaceGuid">The interface GUID to get notified for or null to get notified for all listening GUIDs.</param>
    public void RegisterDeviceArrived(Action<DeviceEventArgs> handler, Guid? interfaceGuid = null)
    {
        lock (_sync)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(DeviceNotificationListener));
            }

            if (_arrivedRegistrations.All(i => i.Handler != handler))
            {
                _arrivedRegistrations.Add(new DeviceEventRegistration
                {
                    Handler = handler, InterfaceGuid = interfaceGuid ?? Guid.Empty
                });
            }
        }
    }

    /// <summary>
    ///     Unsubscribe a previously registered event handler.
    /// </summary>
    /// <param name="handler">The event handler to unsubscribe.</param>
    public void UnregisterDeviceArrived(Action<DeviceEventArgs> handler)
    {
        lock (_sync)
        {
            _arrivedRegistrations.RemoveAll(i => i.Handler == handler);
        }
    }

    /// <summary>
    ///     Subscribe a custom event handler to device removal events.
    /// </summary>
    /// <param name="handler">The event handler to invoke.</param>
    /// <param name="interfaceGuid">The interface GUID to get notified for or null to get notified for all listening GUIDs.</param>
    public void RegisterDeviceRemoved(Action<DeviceEventArgs> handler, Guid? interfaceGuid = null)
    {
        lock (_sync)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(DeviceNotificationListener));
            }

            if (_removedRegistrations.All(i => i.Handler != handler))
            {
                _removedRegistrations.Add(new DeviceEventRegistration
                {
                    Handler = handler, InterfaceGuid = interfaceGuid ?? Guid.Empty
                });
            }
        }
    }

    /// <summary>
    ///     Unsubscribe a previously registered event handler.
    /// </summary>
    /// <param name="handler">The event handler to unsubscribe.</param>
    public void UnregisterDeviceRemoved(Action<DeviceEventArgs> handler)
    {
        lock (_sync)
        {
            _removedRegistrations.RemoveAll(i => i.Handler == handler);
        }
    }

    #endregion

    #region Processing

    private LRESULT WndProc(Guid interfaceGuid, HWND hwnd, uint msg, WPARAM wParam, LPARAM lParam)
    {
        if (msg == PInvoke.WM_DEVICECHANGE)
        {
            DEV_BROADCAST_HDR hdr;

            switch (wParam.Value)
            {
                case PInvoke.DBT_DEVICEARRIVAL:
                    hdr = (DEV_BROADCAST_HDR)Marshal.PtrToStructure(lParam, typeof(DEV_BROADCAST_HDR));

                    if (hdr.dbch_devicetype == DEV_BROADCAST_HDR_DEVICE_TYPE.DBT_DEVTYP_DEVICEINTERFACE)
                    {
                        DEV_BROADCAST_DEVICEINTERFACE deviceInterface =
                            (DEV_BROADCAST_DEVICEINTERFACE)Marshal.PtrToStructure(lParam,
                                typeof(DEV_BROADCAST_DEVICEINTERFACE));

                        DeviceEventArgs arrivedEvent = new()
                        {
                            InterfaceGuid = interfaceGuid, SymLink = deviceInterface.dbcc_name
                        };

                        FireDeviceArrived(arrivedEvent);
                    }

                    break;

                case PInvoke.DBT_DEVICEREMOVECOMPLETE:
                    hdr = (DEV_BROADCAST_HDR)Marshal.PtrToStructure(lParam, typeof(DEV_BROADCAST_HDR));

                    if (hdr.dbch_devicetype == DEV_BROADCAST_HDR_DEVICE_TYPE.DBT_DEVTYP_DEVICEINTERFACE)
                    {
                        DEV_BROADCAST_DEVICEINTERFACE deviceInterface =
                            (DEV_BROADCAST_DEVICEINTERFACE)Marshal.PtrToStructure(lParam,
                                typeof(DEV_BROADCAST_DEVICEINTERFACE));

                        DeviceEventArgs removedEvent = new()
                        {
                            InterfaceGuid = interfaceGuid, SymLink = deviceInterface.dbcc_name
                        };

                        FireDeviceRemoved(removedEvent);
                    }

                    break;
            }
        }

        return new LRESULT(0);
    }

    private void FireDeviceArrived(DeviceEventArgs args)
    {
        DeviceArrived?.Invoke(args);

        List<DeviceEventRegistration> snapshot;
        lock (_sync)
        {
            snapshot = _arrivedRegistrations.ToList();
        }

        foreach (DeviceEventRegistration arrivedRegistration in snapshot)
        {
            if (arrivedRegistration.InterfaceGuid == args.InterfaceGuid ||
                arrivedRegistration.InterfaceGuid == Guid.Empty)
            {
                arrivedRegistration.Handler(args);
            }
        }
    }

    private void FireDeviceRemoved(DeviceEventArgs args)
    {
        DeviceRemoved?.Invoke(args);

        List<DeviceEventRegistration> snapshot;
        lock (_sync)
        {
            snapshot = _removedRegistrations.ToList();
        }

        foreach (DeviceEventRegistration removedRegistration in snapshot)
        {
            if (removedRegistration.InterfaceGuid == args.InterfaceGuid ||
                removedRegistration.InterfaceGuid == Guid.Empty)
            {
                removedRegistration.Handler(args);
            }
        }
    }

    #endregion

    #region Start/End

    /// <summary>
    ///     Start listening for device arrivals/removals using the provided <see cref="Guid" />. Call this after you've
    ///     subscribed to <see cref="DeviceArrived" /> and <see cref="DeviceRemoved" /> events.
    /// </summary>
    /// <param name="interfaceGuid">The device interface GUID to listen for.</param>
    public void StartListen(Guid interfaceGuid)
    {
        ListenerItem listenerItem;

        lock (_sync)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(DeviceNotificationListener));
            }

            if (_listeners.Any(i => i.InterfaceGuid == interfaceGuid && !i.IsStopping))
            {
                return;
            }

            Thread listenerThread = new(Start);
            listenerItem = new ListenerItem { InterfaceGuid = interfaceGuid, Thread = listenerThread };
            _listeners.Add(listenerItem);
            listenerThread.Start(listenerItem);
        }

        listenerItem.StartupCompleted.Wait();

        Exception? startupException;
        lock (_sync)
        {
            startupException = listenerItem.StartupException;
        }

        if (startupException is not null)
        {
            listenerItem.Cancellation.Dispose();
            listenerItem.StartupCompleted.Dispose();
            ExceptionDispatchInfo.Capture(startupException).Throw();
        }
    }

    private unsafe void Start(object parameter)
    {
        ListenerItem listenerItem = (ListenerItem)parameter;
        bool startupSucceeded = false;
        string className = GenerateRandomString();
        string windowName = GenerateRandomString();
        FreeLibrarySafeHandle? moduleHandle = null;
        bool classRegistered = false;
        HWND windowHandle = HWND.Null;

        try
        {
            moduleHandle = PInvoke.GetModuleHandle(null);
            HMODULE hModule = (HMODULE)moduleHandle.DangerousGetHandle();

            // Root the WNDPROC for the lifetime of the registered class; an ephemeral
            // lambda can be collected while native code still holds the function pointer.
            listenerItem.WindowProc = (wnd, msg, wParam, lParam) =>
                WndProc2(listenerItem.InterfaceGuid, wnd, msg, wParam, lParam);

            WNDCLASSEXW windowClass = new()
            {
                cbSize = (uint)Marshal.SizeOf<WNDCLASSEXW>(),
                style = WNDCLASS_STYLES.CS_HREDRAW | WNDCLASS_STYLES.CS_VREDRAW,
                cbClsExtra = 0,
                cbWndExtra = 0,
                hInstance = hModule,
                lpfnWndProc = listenerItem.WindowProc
            };

            fixed (char* pClassName = className)
            fixed (char* pWindowName = windowName)
            {
                windowClass.lpszClassName = pClassName;

                ushort atom = PInvoke.RegisterClassEx(windowClass);
                if (atom == 0)
                {
                    throw new Win32Exception("Failed to register device notification window class.");
                }

                classRegistered = true;
                listenerItem.ClassName = className;
                listenerItem.ModuleHandle = hModule;

                if (listenerItem.Cancellation.IsCancellationRequested)
                {
                    return;
                }

                windowHandle = PInvoke.CreateWindowEx(
                    0,
                    pClassName,
                    pWindowName,
                    0,
                    0, 0, 0, 0,
                    HWND.Null,
                    HMENU.Null,
                    hModule
                );

                if (windowHandle.IsNull)
                {
                    throw new Win32Exception("Failed to create device notification message window.");
                }

                lock (_sync)
                {
                    listenerItem.WindowHandle = windowHandle;

                    if (listenerItem.Cancellation.IsCancellationRequested)
                    {
                        return;
                    }
                }
            }

            startupSucceeded = true;
            listenerItem.StartupCompleted.Set();

            MessagePump(listenerItem);
        }
        catch (Exception ex)
        {
            lock (_sync)
            {
                listenerItem.StartupException = ex;
                _listeners.Remove(listenerItem);
            }

            CleanupListenerResources(listenerItem, moduleHandle, classRegistered, className, windowHandle);
        }
        finally
        {
            if (!startupSucceeded)
            {
                if (listenerItem.StartupException is null)
                {
                    // Stopped during startup before the message pump began.
                    lock (_sync)
                    {
                        _listeners.Remove(listenerItem);
                    }

                    CleanupListenerResources(listenerItem, moduleHandle, classRegistered, className, windowHandle);
                }

                listenerItem.StartupCompleted.Set();
            }
            else
            {
                // Normal shutdown (or pump exit): release window/class on this thread.
                CleanupListenerResources(listenerItem, moduleHandle, classRegistered, className,
                    listenerItem.WindowHandle);

                // Timed-out stops leave the item registered as IsStopping; finish cleanup here.
                bool disposeManaged = false;
                lock (_sync)
                {
                    if (listenerItem.IsStopping)
                    {
                        _listeners.Remove(listenerItem);
                        disposeManaged = true;
                    }
                }

                if (disposeManaged)
                {
                    listenerItem.Cancellation.Dispose();
                    listenerItem.StartupCompleted.Dispose();
                }
            }

            moduleHandle?.Dispose();
        }
    }

    private unsafe void CleanupListenerResources(
        ListenerItem listenerItem,
        FreeLibrarySafeHandle? moduleHandle,
        bool classRegistered,
        string className,
        HWND windowHandle)
    {
        UnregisterNotificationHandle(listenerItem);

        if (!windowHandle.IsNull)
        {
            PInvoke.DestroyWindow(windowHandle);
            lock (_sync)
            {
                listenerItem.WindowHandle = HWND.Null;
            }
        }

        if (classRegistered && moduleHandle is not null)
        {
            fixed (char* pClassName = className)
            {
                PInvoke.UnregisterClass(pClassName, listenerItem.ModuleHandle);
            }
        }

        // Only release after UnregisterClass; native code may invoke the proc until then.
        listenerItem.WindowProc = null;
    }

    /// <summary>
    ///     Atomically takes ownership of <see cref="ListenerItem.NotificationHandle" /> under
    ///     <see cref="_sync" /> so UnregisterDeviceNotification runs at most once per handle.
    /// </summary>
    private void UnregisterNotificationHandle(ListenerItem listenerItem)
    {
        HDEVNOTIFY notificationHandle;
        lock (_sync)
        {
            notificationHandle = listenerItem.NotificationHandle;
            listenerItem.NotificationHandle = HDEVNOTIFY.Null;
        }

        if (!notificationHandle.IsNull)
        {
            PInvoke.UnregisterDeviceNotification(notificationHandle);
        }
    }

    /// <summary>
    ///     Stop listening. The events <see cref="DeviceArrived" /> and <see cref="DeviceRemoved" /> will not get invoked
    ///     anymore after this call. If no <see cref="Guid" /> is specified, all currently registered interfaces will get
    ///     unsubscribed.
    /// </summary>
    public void StopListen(Guid? interfaceGuid = null)
    {
        lock (_sync)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(DeviceNotificationListener));
            }
        }

        StopListenCore(interfaceGuid);
    }

    private void StopListenCore(Guid? interfaceGuid)
    {
        List<ListenerItem> toStop;
        lock (_sync)
        {
            toStop = _listeners
                .Where(i => !i.IsStopping &&
                            (interfaceGuid == null || i.InterfaceGuid == interfaceGuid))
                .ToList();

            foreach (ListenerItem listenerItem in toStop)
            {
                if (!listenerItem.Cancellation.IsCancellationRequested)
                {
                    listenerItem.Cancellation.Cancel();
                }
            }
        }

        foreach (ListenerItem listenerItem in toStop)
        {
            UnregisterNotificationHandle(listenerItem);

            HWND windowHandle;
            lock (_sync)
            {
                windowHandle = listenerItem.WindowHandle;
            }

            if (!windowHandle.IsNull)
            {
                PInvoke.PostMessage(windowHandle, PInvoke.WM_QUIT, new WPARAM(0), new LPARAM(0));
            }

            if (!listenerItem.Thread.Join(TimeSpan.FromSeconds(3)))
            {
                // Keep the item so the thread can finish cleanup, but allow StartListen to replace it.
                lock (_sync)
                {
                    listenerItem.IsStopping = true;
                }

                continue;
            }

            lock (_sync)
            {
                _listeners.Remove(listenerItem);
            }

            listenerItem.Cancellation.Dispose();
            listenerItem.StartupCompleted.Dispose();
        }
    }

    private unsafe LRESULT WndProc2(Guid interfaceGuid, HWND hwnd, uint msg, WPARAM wParam, LPARAM lParam)
    {
        switch (msg)
        {
            case PInvoke.WM_CREATE:
                {
                    RegisterUsbDeviceNotification(interfaceGuid, new HANDLE(hwnd.Value));
                    break;
                }
            case PInvoke.WM_DEVICECHANGE:
                {
                    return WndProc(interfaceGuid, hwnd, msg, wParam, lParam);
                }
        }

        return PInvoke.DefWindowProc(hwnd, msg, wParam, lParam);
    }

    private void MessagePump(ListenerItem listenerItem)
    {
        int retVal;
        while ((retVal = PInvoke.GetMessage(out MSG msg, HWND.Null, 0, 0)) != 0 &&
               !listenerItem.Cancellation.IsCancellationRequested)
        {
            if (retVal == -1)
            {
                break;
            }

            PInvoke.TranslateMessage(msg);
            PInvoke.DispatchMessage(msg);
        }
    }

    private unsafe void RegisterUsbDeviceNotification(Guid interfaceGuid, HANDLE windowHandle)
    {
        ListenerItem listenerItem;
        lock (_sync)
        {
            listenerItem = _listeners.Single(i => i.InterfaceGuid == interfaceGuid && !i.IsStopping);
        }

        DEV_BROADCAST_DEVICEINTERFACE dbcc = new()
        {
            dbcc_size = (uint)Marshal.SizeOf(typeof(DEV_BROADCAST_DEVICEINTERFACE)),
            dbcc_devicetype = DEV_BROADCAST_HDR_DEVICE_TYPE.DBT_DEVTYP_DEVICEINTERFACE,
            dbcc_classguid = interfaceGuid
        };

        IntPtr notificationFilter = Marshal.AllocHGlobal(Marshal.SizeOf(dbcc));
        try
        {
            Marshal.StructureToPtr(dbcc, notificationFilter, true);

            HDEVNOTIFY notificationHandle = PInvoke.RegisterDeviceNotification(
                windowHandle,
                notificationFilter.ToPointer(),
                REGISTER_NOTIFICATION_FLAGS.DEVICE_NOTIFY_WINDOW_HANDLE
            );

            lock (_sync)
            {
                listenerItem.NotificationHandle = notificationHandle;
            }
        }
        finally
        {
            Marshal.FreeHGlobal(notificationFilter);
        }
    }

    #endregion

    #region Win32

    [StructLayout(LayoutKind.Sequential)]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Local")]
    private struct DEV_BROADCAST_HDR
    {
        public readonly uint dbch_size;
        public readonly DEV_BROADCAST_HDR_DEVICE_TYPE dbch_devicetype;
        public readonly uint dbch_reserved;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Local")]
    private struct DEV_BROADCAST_DEVICEINTERFACE
    {
        public uint dbcc_size;
        public DEV_BROADCAST_HDR_DEVICE_TYPE dbcc_devicetype;
        public readonly uint dbcc_reserved;
        public Guid dbcc_classguid;

        // To get value from lParam of WM_DEVICECHANGE, this length must be longer than 1.
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 255)]
        public readonly string dbcc_name;
    }

    #endregion
}
