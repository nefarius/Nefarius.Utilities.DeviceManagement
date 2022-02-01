using System;
using System.Runtime.InteropServices;

namespace Nefarius.Utilities.DeviceManagement.PnP
{
    internal static class Kernel32
    {
        public static bool MethodExists(string libraryName, string methodName)
        {
            var libraryPtr = LoadLibrary(libraryName);

            if (libraryPtr == UIntPtr.Zero) return false;

            var procPtr = GetProcAddress(libraryPtr, methodName);

            return procPtr != UIntPtr.Zero;
        }

        [DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern UIntPtr LoadLibrary(string lpFileName);

        [DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true)]
        internal static extern UIntPtr GetProcAddress(UIntPtr hModule, string lpProcName);
    }
}