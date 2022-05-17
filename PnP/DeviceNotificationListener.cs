using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using PInvoke;
using Win32Exception = System.ComponentModel.Win32Exception;

namespace Nefarius.Utilities.DeviceManagement.PnP
{
    /// <summary>
    ///     Utility class to listen for system-wide device arrivals and removals based on a provided device interface GUID.
    /// </summary>
    /// <remarks>Original source: https://gist.github.com/emoacht/73eff195317e387f4cda</remarks>
    public class DeviceNotificationListener : IDeviceNotificationListener, IDisposable
    {
        private readonly CancellationTokenSource _cancellationTokenSource = new();

        /// <inheritdoc />
        public event Action<DeviceEventArgs> DeviceArrived;

        /// <inheritdoc />
        public event Action<DeviceEventArgs> DeviceRemoved;

        private readonly List<ListenerItem> _listeners = new();

        private readonly List<DeviceEventRegistration> _arrivedRegistrations = new();
        private readonly List<DeviceEventRegistration> _removedRegistrations = new();

        #region Registration

        /// <inheritdoc />
        public void RegisterDeviceArrived(Action<DeviceEventArgs> handler, Guid? interfaceGuid = null)
        {
            if (_arrivedRegistrations.All(i => i.Handler != handler))
            {
                _arrivedRegistrations.Add(new DeviceEventRegistration
                {
                    Handler = handler,
                    InterfaceGuid = interfaceGuid ?? Guid.Empty
                });
            }
        }

        /// <inheritdoc />
        public void UnregisterDeviceArrived(Action<DeviceEventArgs> handler)
        {
            foreach (var arrivedRegistration in _arrivedRegistrations.ToList())
            {
                if (arrivedRegistration.Handler == handler)
                {
                    _arrivedRegistrations.Remove(arrivedRegistration);
                }
            }
        }

        /// <inheritdoc />
        public void RegisterDeviceRemoved(Action<DeviceEventArgs> handler, Guid? interfaceGuid = null)
        {
            if (_removedRegistrations.All(i => i.Handler != handler))
            {
                _removedRegistrations.Add(new DeviceEventRegistration
                {
                    Handler = handler,
                    InterfaceGuid = interfaceGuid ?? Guid.Empty
                });
            }
        }

        /// <inheritdoc />
        public void UnregisterDeviceRemoved(Action<DeviceEventArgs> handler)
        {
            foreach (var removedRegistration in _removedRegistrations.ToList())
            {
                if (removedRegistration.Handler == handler)
                {
                    _removedRegistrations.Remove(removedRegistration);
                }
            }
        }

        #endregion

        #region Processing

        private IntPtr WndProc(Guid interfaceGuid, IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == (int)User32.WindowMessage.WM_DEVICECHANGE)
            {
                DEV_BROADCAST_HDR hdr;

                switch ((int)wParam)
                {
                    case DBT_DEVICEARRIVAL:
                        hdr = (DEV_BROADCAST_HDR)Marshal.PtrToStructure(lParam, typeof(DEV_BROADCAST_HDR));

                        if (hdr.dbch_devicetype == DBT_DEVTYP_DEVICEINTERFACE)
                        {
                            var deviceInterface =
                                (DEV_BROADCAST_DEVICEINTERFACE)Marshal.PtrToStructure(lParam,
                                    typeof(DEV_BROADCAST_DEVICEINTERFACE));
                            
                            var arrivedEvent = new DeviceEventArgs
                            {
                                InterfaceGuid = interfaceGuid,
                                SymLink = deviceInterface.dbcc_name
                            };
                            
                            FireDeviceArrived(arrivedEvent);
                        }

                        break;

                    case DBT_DEVICEREMOVECOMPLETE:
                        hdr = (DEV_BROADCAST_HDR)Marshal.PtrToStructure(lParam, typeof(DEV_BROADCAST_HDR));

                        if (hdr.dbch_devicetype == DBT_DEVTYP_DEVICEINTERFACE)
                        {
                            var deviceInterface =
                                (DEV_BROADCAST_DEVICEINTERFACE)Marshal.PtrToStructure(lParam,
                                    typeof(DEV_BROADCAST_DEVICEINTERFACE));
                            
                            var removedEvent = new DeviceEventArgs
                            {
                                InterfaceGuid = interfaceGuid,
                                SymLink = deviceInterface.dbcc_name
                            };

                            FireDeviceRemoved(removedEvent);
                        }

                        break;
                }
            }

            return IntPtr.Zero;
        }

        private void FireDeviceArrived(DeviceEventArgs args)
        {
            DeviceArrived?.Invoke(args);

            foreach (var arrivedRegistration in _arrivedRegistrations)
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

            foreach (var removedRegistration in _removedRegistrations)
            {
                if (removedRegistration.InterfaceGuid == args.InterfaceGuid ||
                    removedRegistration.InterfaceGuid == Guid.Empty)
                {
                    removedRegistration.Handler(args);
                }
            }
        }

        #endregion

        #region Util

        private static string GenerateRandomString()
        {
            // Creating object of random class
            var rand = new Random();

            // Choosing the size of string
            // Using Next() string
            var stringlen = rand.Next(4, 10);
            int randValue;
            var sb = new StringBuilder();
            char letter;
            for (var i = 0; i < stringlen; i++)
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

        #region Start/End

        /// <inheritdoc />
        public void StartListen(Guid interfaceGuid)
        {
            if (_listeners.All(i => i.InterfaceGuid != interfaceGuid))
            {
                var listenerThread = new Thread(Start);
                var listenerItem = new ListenerItem
                {
                    InterfaceGuid = interfaceGuid,
                    Thread = listenerThread
                };
                _listeners.Add(listenerItem);
                listenerThread.Start(listenerItem);
            }
        }

        private unsafe void Start(object parameter)
        {
            var listenerItem = (ListenerItem)parameter;
            var className = GenerateRandomString(); // random string to avoid conflicts
            var wndClass = User32.WNDCLASSEX.Create();

            fixed (char* cln = className)
            {
                wndClass.lpszClassName = cln;
            }
            
            var wndProc = new User32.WndProc((wnd, msg, param, lParam) => WndProc2(listenerItem.InterfaceGuid, wnd, msg, param, lParam));
            
            wndClass.style = User32.ClassStyles.CS_HREDRAW | User32.ClassStyles.CS_VREDRAW;
            wndClass.lpfnWndProc = wndProc;
            wndClass.cbClsExtra = 0;
            wndClass.cbWndExtra = 0;
            wndClass.hInstance = GetModuleHandle(IntPtr.Zero);

            User32.RegisterClassEx(ref wndClass);

            var windowHandle = User32.CreateWindowEx(0, className, GenerateRandomString(), 0, 0, 0, 0, 0,
                new IntPtr(-3), IntPtr.Zero, wndClass.hInstance, IntPtr.Zero);
            listenerItem.WindowHandle = windowHandle;

            MessagePump();
        }

        /// <inheritdoc />
        public void StopListen(Guid? interfaceGuid = null)
        {
            _cancellationTokenSource.Cancel();

            foreach (var listenerItem in _listeners.ToList())
            {
                if (interfaceGuid == null || listenerItem.InterfaceGuid == interfaceGuid)
                {
                    UnregisterUsbDeviceNotification(listenerItem.NotificationHandle);
                    User32.PostMessage(listenerItem.WindowHandle, User32.WindowMessage.WM_QUIT, IntPtr.Zero,
                        IntPtr.Zero);
                    listenerItem.Thread.Join(TimeSpan.FromSeconds(3));
                    _listeners.Remove(listenerItem);
                }
            }
        }

        private unsafe IntPtr WndProc2(Guid interfaceGuid, IntPtr hwnd, User32.WindowMessage msg, void* wParam, void* lParam)
        {
            switch (msg)
            {
                case User32.WindowMessage.WM_CREATE:
                {
                    RegisterUsbDeviceNotification(interfaceGuid, hwnd);
                    break;
                }
                case User32.WindowMessage.WM_DEVICECHANGE:
                {
                    var handled = false;
                    return WndProc(interfaceGuid, hwnd, (int)msg, (IntPtr)wParam, (IntPtr)lParam, ref handled);
                }
            }

            return User32.DefWindowProc(hwnd, msg, (IntPtr)wParam, (IntPtr)lParam);
        }

        private void MessagePump()
        {
            var msg = Marshal.AllocHGlobal(Marshal.SizeOf<User32.MSG>());
            int retVal;
            while ((retVal = User32.GetMessage(msg, IntPtr.Zero, 0, 0)) != 0 &&
                   !_cancellationTokenSource.Token.IsCancellationRequested)
                if (retVal == -1)
                {
                    break;
                }
                else
                {
                    User32.TranslateMessage(msg);
                    User32.DispatchMessage(msg);
                }
        }

        private void RegisterUsbDeviceNotification(Guid interfaceGuid, IntPtr windowHandle)
        {
            var listenerItem = _listeners.Single(i => i.InterfaceGuid == interfaceGuid);

            var dbcc = new DEV_BROADCAST_DEVICEINTERFACE
            {
                dbcc_size = (uint)Marshal.SizeOf(typeof(DEV_BROADCAST_DEVICEINTERFACE)),
                dbcc_devicetype = DBT_DEVTYP_DEVICEINTERFACE,
                dbcc_classguid = interfaceGuid
            };

            var notificationFilter = Marshal.AllocHGlobal(Marshal.SizeOf(dbcc));
            Marshal.StructureToPtr(dbcc, notificationFilter, true);

            var notificationHandle =
                RegisterDeviceNotification(windowHandle, notificationFilter, DEVICE_NOTIFY_WINDOW_HANDLE);
            if (notificationHandle == IntPtr.Zero)
                throw new Win32Exception(Marshal.GetLastWin32Error(), "Failed to register device notifications.");

            listenerItem.NotificationHandle = notificationHandle;
        }

        private void UnregisterUsbDeviceNotification(IntPtr notificationHandle)
        {
            if (notificationHandle != IntPtr.Zero)
                UnregisterDeviceNotification(notificationHandle);
        }

        #endregion

        #region Win32

        [DllImport(nameof(Kernel32), SetLastError = true)]  
        public static extern IntPtr GetModuleHandle(IntPtr lpModuleName); 

        [DllImport(nameof(User32), SetLastError = true)]
        private static extern IntPtr RegisterDeviceNotification(
            IntPtr hRecipient,
            IntPtr NotificationFilter,
            uint Flags);

        [DllImport(nameof(User32), SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnregisterDeviceNotification(IntPtr Handle);

        private const uint DEVICE_NOTIFY_WINDOW_HANDLE = 0x00000000;

        private const int
            DBT_DEVICEARRIVAL =
                0x8000; // Device event when a device or piece of media has been inserted and becomes available

        private const int
            DBT_DEVICEREMOVECOMPLETE =
                0x8004; // Device event when a device or piece of media has been physically removed

        [StructLayout(LayoutKind.Sequential)]
        private struct DEV_BROADCAST_HDR
        {
            public readonly uint dbch_size;
            public readonly uint dbch_devicetype;
            public readonly uint dbch_reserved;
        }

        private const uint DBT_DEVTYP_DEVICEINTERFACE = 0x00000005;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct DEV_BROADCAST_DEVICEINTERFACE
        {
            public uint dbcc_size;
            public uint dbcc_devicetype;
            public readonly uint dbcc_reserved;
            public Guid dbcc_classguid;

            // To get value from lParam of WM_DEVICECHANGE, this length must be longer than 1.
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 255)]
            public readonly string dbcc_name;
        }

        #endregion

        private class ListenerItem
        {
            public Guid InterfaceGuid { get; set; }
            public Thread Thread { get; set; }
            public IntPtr WindowHandle { get; set; }
            public IntPtr NotificationHandle { get; set; }
        }

        private class DeviceEventRegistration
        {
            public Action<DeviceEventArgs> Handler { get; set; }
            public Guid InterfaceGuid { get; set; }
        }

        public void Dispose()
        {
            // Dispose of unmanaged resources.
            Dispose(true);
            // Suppress finalization.
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _arrivedRegistrations.Clear();
                _removedRegistrations.Clear();
            }
        }
    }
}