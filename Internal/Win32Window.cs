/*  WinUSBNet library
 *  (C) 2010 Thomas Bleeker (www.madwizard.org)
 *
 *  Licensed under the MIT license, see license.txt or:
 *  http://www.opensource.org/licenses/mit-license.php
 */

/* NOTE: Parts of the code in this file are based on the work of Jan Axelson
 * See http://www.lvr.com/winusb.htm for more information
 */

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace Nefarius.Utilities.DeviceManagement.Internal
{
    internal class Win32Window
    {
        //typedef LRESULT(CALLBACK* WNDPROC)(HWND, UINT, WPARAM, LPARAM);
        private DeviceManagement.WndProc delegWndProc = null; // = myWndProc;
        private Thread WndProcThread;

        internal IntPtr WinFromHwnd { get; private set; }
        //[DllImport("user32.dll")]
        //static extern bool TranslateMessage([In] ref MSG lpMsg);

        //[DllImport("user32.dll")]
        //static extern IntPtr DispatchMessage([In] ref MSG lpmsg);
        public Win32Window(DeviceManagement.WndProc wndProc)
        {
            delegWndProc = wndProc;
        }

        private bool CreateWindowOperationCompleteFlag = false;

        private void MessagePool()
        {
            DeviceManagement.WNDCLASS wind_class = new DeviceManagement.WNDCLASS
            {
                //wind_class.hbrBackground = (IntPtr)COLOR_BACKGROUND + 1; //Black background, +1 is necessary
                //wind_class.style = (int)(DeviceManagement.CS_HREDRAW | DeviceManagement.CS_VREDRAW | DeviceManagement.CS_DBLCLKS); //Doubleclicks are active
                //wind_class.cbSize = Marshal.SizeOf(typeof(DeviceManagement.WNDCLASS));
                cbClsExtra = 0,
                cbWndExtra = 0,
                hInstance = Process.GetCurrentProcess().Handle,
                hIcon = IntPtr.Zero,
                hCursor = DeviceManagement.LoadCursor(IntPtr.Zero, (int)DeviceManagement.IDC_CROSS), // Crosshair cursor;
                lpszMenuName = null,
                lpszClassName = "myClass",
                lpfnWndProc = Marshal.GetFunctionPointerForDelegate(delegWndProc)
            };
            //wind_class.hIconSm = IntPtr.Zero;
            ushort regResult = DeviceManagement.RegisterClass(ref wind_class);

            if (regResult == 0)
            {
                uint error = DeviceManagement.GetLastError();

                goto exi;
            }

            string wndClass = wind_class.lpszClassName;

            //The next line did NOT work with me! When searching the web, the reason seems to be unclear! 
            //It resulted in a zero hWnd, but GetLastError resulted in zero (i.e. no error) as well !!??)
            //IntPtr hWnd = CreateWindowEx(0, wind_class.lpszClassName, "MyWnd", WS_OVERLAPPEDWINDOW | WS_VISIBLE, 0, 0, 30, 40, IntPtr.Zero, IntPtr.Zero, wind_class.hInstance, IntPtr.Zero);

            //This version worked and resulted in a non-zero hWnd
            IntPtr hWnd = DeviceManagement.CreateWindowEx(0,
                "myClass",
                "Hello Win32",
                DeviceManagement.WS_OVERLAPPEDWINDOW,
                unchecked((int)0x80000000), unchecked((int)0x80000000), unchecked((int)0x80000000),
                unchecked((int)0x80000000),
                IntPtr.Zero,
                IntPtr.Zero,
                wind_class.hInstance,
                IntPtr.Zero);

            if (hWnd == ((IntPtr)0))
            {
                uint error = DeviceManagement.GetLastError();
                goto exi;
            }

            WinFromHwnd = hWnd;
            CreateWindowOperationCompleteFlag = true;
            //DeviceManagement.ShowWindow(hWnd, 1);
            //DeviceManagement.UpdateWindow(hWnd);
            DeviceManagement.MSG msg;
            while (true)
            {
                while (DeviceManagement.GetMessage(out msg, IntPtr.Zero, DeviceManagement.WM_DEVICECHANGE,
                           DeviceManagement.WM_DEVICECHANGE) != 0)
                {
                    DeviceManagement.TranslateMessage(out msg);
                    DeviceManagement.DispatchMessage(out msg);
                }
            }

            exi:
            CreateWindowOperationCompleteFlag = true;
        }

        internal bool Create()
        {
            if (WinFromHwnd != IntPtr.Zero && CreateWindowOperationCompleteFlag == false)
            {
                return false;
            }

            WndProcThread = new Thread(MessagePool)
            {
                IsBackground = true,
                Name = "Win32Window Message Pool Thread"
            };
            WndProcThread.Start();
            while (CreateWindowOperationCompleteFlag == false) ; //don't worry,so fast!
            return true;

            //The explicit message pump is not necessary, messages are obviously dispatched by the framework.
            //However, if the while loop is implemented, the functions are called... Windows mysteries...
            //MSG msg;
            //while (GetMessage(out msg, IntPtr.Zero, 0, 0) != 0)
            //{
            //    TranslateMessage(ref msg);
            //    DispatchMessage(ref msg);
            //}
        }

        private static IntPtr myWndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
        {
            switch (msg)
            {
                case DeviceManagement.WM_DESTROY:
                    DeviceManagement.DestroyWindow(hWnd);

                    //If you want to shutdown the application, call the next function instead of DestroyWindow
                    //PostQuitMessage(0);
                    break;

                default:
                    break;
            }

            return DeviceManagement.DefWindowProc(hWnd, msg, wParam, lParam);
        }
    }
}