﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;

namespace Nefarius.Utilities.DeviceManagement.PnP;

/// <summary>
///     Utility class to listen for system-wide device arrivals and removals based on a provided device interface GUID.
/// </summary>
/// <remarks>Original source: https://gist.github.com/emoacht/73eff195317e387f4cda</remarks>
public sealed class DeviceNotificationListener : IDeviceNotificationListener, IDisposable
{
    private readonly List<DeviceEventRegistration> _arrivedRegistrations = new();
    private readonly CancellationTokenSource _cancellationTokenSource = new();

    private readonly List<ListenerItem> _listeners = new();
    private readonly List<DeviceEventRegistration> _removedRegistrations = new();

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
        if (disposing)
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
        if (_arrivedRegistrations.All(i => i.Handler != handler))
        {
            _arrivedRegistrations.Add(new DeviceEventRegistration
            {
                Handler = handler, InterfaceGuid = interfaceGuid ?? Guid.Empty
            });
        }
    }

    /// <summary>
    ///     Unsubscribe a previously registered event handler.
    /// </summary>
    /// <param name="handler">The event handler to unsubscribe.</param>
    public void UnregisterDeviceArrived(Action<DeviceEventArgs> handler)
    {
        foreach (DeviceEventRegistration arrivedRegistration in _arrivedRegistrations.ToList())
        {
            if (arrivedRegistration.Handler == handler)
            {
                _arrivedRegistrations.Remove(arrivedRegistration);
            }
        }
    }

    /// <summary>
    ///     Subscribe a custom event handler to device removal events.
    /// </summary>
    /// <param name="handler">The event handler to invoke.</param>
    /// <param name="interfaceGuid">The interface GUID to get notified for or null to get notified for all listening GUIDs.</param>
    public void RegisterDeviceRemoved(Action<DeviceEventArgs> handler, Guid? interfaceGuid = null)
    {
        if (_removedRegistrations.All(i => i.Handler != handler))
        {
            _removedRegistrations.Add(new DeviceEventRegistration
            {
                Handler = handler, InterfaceGuid = interfaceGuid ?? Guid.Empty
            });
        }
    }

    /// <summary>
    ///     Unsubscribe a previously registered event handler.
    /// </summary>
    /// <param name="handler">The event handler to unsubscribe.</param>
    public void UnregisterDeviceRemoved(Action<DeviceEventArgs> handler)
    {
        foreach (DeviceEventRegistration removedRegistration in _removedRegistrations.ToList())
        {
            if (removedRegistration.Handler == handler)
            {
                _removedRegistrations.Remove(removedRegistration);
            }
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

        foreach (DeviceEventRegistration arrivedRegistration in _arrivedRegistrations)
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

        foreach (DeviceEventRegistration removedRegistration in _removedRegistrations)
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
        if (_listeners.All(i => i.InterfaceGuid != interfaceGuid))
        {
            Thread listenerThread = new(Start);
            ListenerItem listenerItem = new() { InterfaceGuid = interfaceGuid, Thread = listenerThread };
            _listeners.Add(listenerItem);
            listenerThread.Start(listenerItem);
        }
    }

    private unsafe void Start(object parameter)
    {
        ListenerItem listenerItem = (ListenerItem)parameter;
        string className = GenerateRandomString(); // random string to avoid conflicts
        string windowName = GenerateRandomString();
        using FreeLibrarySafeHandle hInst = PInvoke.GetModuleHandle((string)null);

        WNDCLASSEXW windowClass = new()
        {
            cbSize = (uint)Marshal.SizeOf<WNDCLASSEXW>(),
            style = WNDCLASS_STYLES.CS_HREDRAW | WNDCLASS_STYLES.CS_VREDRAW,
            cbClsExtra = 0,
            cbWndExtra = 0,
            hInstance = (HMODULE)hInst.DangerousGetHandle(),
            lpfnWndProc = (wnd, msg, wParam, lParam) =>
                WndProc2(listenerItem.InterfaceGuid, wnd, msg, wParam, lParam)
        };

        fixed (char* pClassName = className)
        fixed (char* pWindowName = windowName)
        {
            windowClass.lpszClassName = pClassName;

            PInvoke.RegisterClassEx(windowClass);

            listenerItem.WindowHandle = PInvoke.CreateWindowEx(
                0,
                pClassName,
                pWindowName,
                0,
                0, 0, 0, 0,
                HWND.Null,
                HMENU.Null,
                new HMODULE(hInst.DangerousGetHandle())
            );
        }

        MessagePump();
    }

    /// <summary>
    ///     Stop listening. The events <see cref="DeviceArrived" /> and <see cref="DeviceRemoved" /> will not get invoked
    ///     anymore after this call. If no <see cref="Guid" /> is specified, all currently registered interfaces will get
    ///     unsubscribed.
    /// </summary>
    public void StopListen(Guid? interfaceGuid = null)
    {
        _cancellationTokenSource.Cancel();

        foreach (ListenerItem listenerItem in _listeners.ToList())
        {
            if (interfaceGuid == null || listenerItem.InterfaceGuid == interfaceGuid)
            {
                UnregisterUsbDeviceNotification(listenerItem.NotificationHandle);
                PInvoke.PostMessage(listenerItem.WindowHandle, PInvoke.WM_QUIT, new WPARAM(0), new LPARAM(0));
                listenerItem.Thread.Join(TimeSpan.FromSeconds(3));
                _listeners.Remove(listenerItem);
            }
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

    private void MessagePump()
    {
        int retVal;
        while ((retVal = PInvoke.GetMessage(out MSG msg, HWND.Null, 0, 0)) != 0 &&
               !_cancellationTokenSource.Token.IsCancellationRequested)
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
        ListenerItem listenerItem = _listeners.Single(i => i.InterfaceGuid == interfaceGuid);

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

            listenerItem.NotificationHandle = notificationHandle;
        }
        finally
        {
            Marshal.FreeHGlobal(notificationFilter);
        }
    }

    private void UnregisterUsbDeviceNotification(HDEVNOTIFY notificationHandle)
    {
        PInvoke.UnregisterDeviceNotification(notificationHandle);
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