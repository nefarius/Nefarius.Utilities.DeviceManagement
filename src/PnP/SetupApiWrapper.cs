using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

using Windows.Win32.Devices.DeviceAndDriverInstallation;
using Windows.Win32.Foundation;

namespace Nefarius.Utilities.DeviceManagement.PnP;

/// <summary>
///     SetupAPI imports.
/// </summary>
/// <example>https://learn.microsoft.com/en-us/windows-hardware/drivers/install/setupapi</example>
/// <remarks>
///     TODO: migrate over to CsWin32
/// </remarks>
internal static class SetupApiWrapper
{
    private const int LineLen = 256;

    #region Constant and Structure Definitions

    [StructLayout(LayoutKind.Sequential)]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    internal struct SP_DEVINFO_DATA
    {
#pragma warning disable IDE1006
        internal int cbSize;
#pragma warning restore IDE1006
        internal readonly Guid ClassGuid;
        internal readonly uint DevInst;
        internal readonly IntPtr Reserved;
    }

    [StructLayout(LayoutKind.Sequential)]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    internal struct SP_DRVINFO_DATA
    {
#pragma warning disable IDE1006
        internal readonly int cbSize;
#pragma warning restore IDE1006
        internal readonly UInt32 DriverType;
        internal IntPtr Reserved;

        [MarshalAs(UnmanagedType.LPWStr, SizeConst = LineLen)]
        internal readonly string Description;

        [MarshalAs(UnmanagedType.LPWStr, SizeConst = LineLen)]
        internal readonly string MfgName;

        [MarshalAs(UnmanagedType.LPWStr, SizeConst = LineLen)]
        internal readonly string ProviderName;

        internal readonly FILETIME DriverDate;
        internal readonly UInt64 DriverVersion;
    }

    [StructLayout(LayoutKind.Sequential)]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    internal struct SP_CLASSINSTALL_HEADER
    {
#pragma warning disable IDE1006
        internal int cbSize;
#pragma warning restore IDE1006
        internal int InstallFunction;
    }

    [StructLayout(LayoutKind.Sequential)]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    internal struct SP_REMOVEDEVICE_PARAMS
    {
        internal SP_CLASSINSTALL_HEADER ClassInstallHeader;
        internal int Scope;
        internal int HwProfile;
    }

    #endregion

    #region Interop Definitions

    #region SetupAPI

    [DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
    internal static extern IntPtr SetupDiCreateDeviceInfoList(ref Guid classGuid, IntPtr hwndParent);

    [DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
    internal static extern bool SetupDiDestroyDeviceInfoList(IntPtr deviceInfoSet);

    [DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
    internal static extern bool SetupDiCreateDeviceInfo(IntPtr deviceInfoSet, string deviceName, ref Guid classGuid,
        string deviceDescription, IntPtr hwndParent, int creationFlags, ref SP_DEVINFO_DATA deviceInfoData);

    [DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
    internal static extern bool SetupDiSetDeviceRegistryProperty(IntPtr deviceInfoSet,
        ref SP_DEVINFO_DATA deviceInfoData, int property, [MarshalAs(UnmanagedType.LPWStr)] string propertyBuffer,
        int propertyBufferSize);

    [DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
    internal static extern bool SetupDiCallClassInstaller(int installFunction, IntPtr deviceInfoSet,
        ref SP_DEVINFO_DATA deviceInfoData);

    [DllImport("setupapi.dll", SetLastError = true)]
    internal static extern bool SetupDiEnumDeviceInfo(IntPtr deviceInfoSet,
        UInt32 memberIndex,
        ref SP_DEVINFO_DATA deviceInfoData);

    [DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
    internal static extern IntPtr SetupDiGetClassDevs(ref Guid classGuid, IntPtr enumerator, IntPtr hwndParent,
        int flags);

    [DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
    internal static extern bool SetupDiEnumDeviceInterfaces(IntPtr deviceInfoSet, IntPtr deviceInfoData,
        ref Guid interfaceClassGuid, int memberIndex, ref SP_DEVINFO_DATA deviceInterfaceData);

    [DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
    internal static extern bool SetupDiGetDeviceInterfaceDetail(IntPtr deviceInfoSet,
        ref SP_DEVINFO_DATA deviceInterfaceData, IntPtr deviceInterfaceDetailData,
        int deviceInterfaceDetailDataSize,
        ref int requiredSize, ref SP_DEVINFO_DATA deviceInfoData);

    [DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
    internal static extern bool SetupDiOpenDeviceInfo(IntPtr deviceInfoSet, string deviceInstanceId,
        IntPtr hwndParent, int flags, ref SP_DEVINFO_DATA deviceInfoData);

    [DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
    internal static extern bool SetupDiSetClassInstallParams(IntPtr deviceInfoSet,
        ref SP_DEVINFO_DATA deviceInterfaceData, ref SP_REMOVEDEVICE_PARAMS classInstallParams,
        int classInstallParamsSize);

    [DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    internal static extern bool SetupDiGetDeviceInstallParams(
        IntPtr hDevInfo,
        ref SP_DEVINFO_DATA deviceInfoData,
        IntPtr deviceInstallParams
    );

    [DllImport("setupapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool SetupUninstallOEMInf(
        [MarshalAs(UnmanagedType.LPWStr)] string infName,
        uint flags,
        IntPtr reserved);

    [DllImport("setupapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static unsafe extern bool SetupDiOpenDeviceInfo(
        [In] IntPtr deviceInfoSet,
        [In] [MarshalAs(UnmanagedType.LPWStr)] string deviceInstanceId,
        [In] [Optional] IntPtr parent,
        [In] UInt32 openFlags,
        [Out] [Optional] SP_DEVINFO_DATA* deviceInfoData
    );

    #endregion

    #region Newdev

    [DllImport("newdev.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    internal static extern bool DiInstallDriver(
        IntPtr hwndParent,
        string fullInfPath,
        uint flags,
        out bool needReboot);

    [DllImport("newdev.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    internal static extern bool DiUninstallDriver(
        [In] IntPtr hwndParent,
        [In] string infPath,
        [In] uint flags,
        [Out] out bool needReboot
    );

    [DllImport("newdev.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    internal static extern unsafe bool DiInstallDevice(
        [In] [Optional] IntPtr hwndParent,
        [In] HDEVINFO deviceInfoSet,
        [In] SP_DEVINFO_DATA* deviceInfoData,
#pragma warning disable CS8500
        [In] [Optional] SP_DRVINFO_DATA* driverInfoData,
#pragma warning restore CS8500
        [In] uint flags,
        [Out] out bool needReboot
    );

    #endregion

    #endregion
}