﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text;

using Windows.Win32.Devices.DeviceAndDriverInstallation;
using Windows.Win32.Devices.Properties;
using Windows.Win32.Foundation;

using FILETIME = Windows.Win32.Foundation.FILETIME;

namespace Nefarius.Utilities.DeviceManagement.PnP;

/// <summary>
///     SetupAPI imports.
/// </summary>
/// <example>https://learn.microsoft.com/en-us/windows-hardware/drivers/install/setupapi</example>
/// <remarks>
///     TODO: migrate over to CsWin32
/// </remarks>
internal static class SetupApi
{
    private const int LineLen = 256;
    private const int MaxPath = 260;

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

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    internal struct SP_DRVINFO_DATA
    {
#pragma warning disable IDE1006
        internal int cbSize;
#pragma warning restore IDE1006
        internal readonly UInt32 DriverType;
        internal IntPtr Reserved;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = LineLen)]
        internal readonly string Description;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = LineLen)]
        internal readonly string MfgName;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = LineLen)]
        internal readonly string ProviderName;

        internal readonly FILETIME DriverDate;
        internal readonly UInt64 DriverVersion;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    internal struct SP_DRVINFO_DETAIL_DATA
    {
#pragma warning disable IDE1006
        internal int cbSize;
#pragma warning restore IDE1006

        internal readonly FILETIME InfDate;
        internal readonly UInt32 CompatIDsOffset;
        internal readonly UInt32 CompatIDsLength;
        internal IntPtr Reserved;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = LineLen)]
        internal readonly string SectionName;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MaxPath)]
        internal readonly string InfFileName;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = LineLen)]
        internal readonly string DrvDescription;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1)]
        internal readonly string HardwareID;
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

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    [SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Global")]
    internal struct SP_DEVINSTALL_PARAMS
    {
        internal Int32 cbSize;
        internal Int32 Flags;
        internal Int32 FlagsEx;
        internal HWND hwndParent;
        internal IntPtr InstallMsgHandler;
        internal IntPtr InstallMsgHandlerContext;
        internal IntPtr FileQueue;
        internal IntPtr ClassInstallReserved;
        internal UInt32 Reserved;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        internal string DriverPath;
    }

    #endregion

    #region Interop Definitions

    #region SetupAPI

    [DllImport(nameof(SetupApi), SetLastError = true, CharSet = CharSet.Unicode)]
    internal static extern HDEVINFO SetupDiCreateDeviceInfoList(
        ref Guid classGuid,
        HWND hwndParent
    );

    [DllImport(nameof(SetupApi), SetLastError = true, CharSet = CharSet.Unicode)]
    internal static extern unsafe HDEVINFO SetupDiCreateDeviceInfoList(
        Guid* classGuid,
        HWND hwndParent
    );

    [DllImport(nameof(SetupApi), SetLastError = true, CharSet = CharSet.Unicode)]
    internal static extern bool SetupDiDestroyDeviceInfoList(
        HDEVINFO deviceInfoSet
    );

    [DllImport(nameof(SetupApi), SetLastError = true, CharSet = CharSet.Unicode)]
    internal static extern bool SetupDiCreateDeviceInfo(
        HDEVINFO deviceInfoSet,
        string deviceName,
        ref Guid classGuid,
        string deviceDescription,
        HWND hwndParent,
        int creationFlags,
        ref SP_DEVINFO_DATA deviceInfoData
    );

    [DllImport(nameof(SetupApi), SetLastError = true, CharSet = CharSet.Unicode)]
    internal static extern bool SetupDiSetDeviceRegistryProperty(
        HDEVINFO deviceInfoSet,
        ref SP_DEVINFO_DATA deviceInfoData,
        int property,
        [MarshalAs(UnmanagedType.LPWStr)] string propertyBuffer,
        int propertyBufferSize
    );

    [DllImport(nameof(SetupApi), SetLastError = true, CharSet = CharSet.Unicode)]
    internal static extern bool SetupDiCallClassInstaller(
        int installFunction,
        IntPtr deviceInfoSet,
        ref SP_DEVINFO_DATA deviceInfoData
    );

    [DllImport(nameof(SetupApi), SetLastError = true)]
    internal static extern bool SetupDiEnumDeviceInfo(
        HDEVINFO deviceInfoSet,
        uint memberIndex,
        ref SP_DEVINFO_DATA deviceInfoData
    );

    [DllImport(nameof(SetupApi), SetLastError = true)]
    internal static extern unsafe bool SetupDiEnumDeviceInfo(
        HDEVINFO deviceInfoSet,
        uint memberIndex,
        SP_DEVINFO_DATA* deviceInfoData
    );

    [DllImport(nameof(SetupApi), SetLastError = true, CharSet = CharSet.Unicode)]
    internal static extern HDEVINFO SetupDiGetClassDevs(
        ref Guid classGuid,
        IntPtr enumerator,
        HWND hwndParent,
        uint flags
    );

    [DllImport(nameof(SetupApi), SetLastError = true, CharSet = CharSet.Unicode)]
    internal static extern unsafe HDEVINFO SetupDiGetClassDevs(
        Guid* classGuid,
        string enumerator,
        HWND hwndParent,
        uint flags
    );

    [DllImport(nameof(SetupApi), SetLastError = true, CharSet = CharSet.Unicode)]
    internal static extern bool SetupDiEnumDeviceInterfaces(
        HDEVINFO deviceInfoSet,
        IntPtr deviceInfoData,
        ref Guid interfaceClassGuid,
        int memberIndex,
        ref SP_DEVINFO_DATA deviceInterfaceData
    );

    [DllImport(nameof(SetupApi), SetLastError = true, CharSet = CharSet.Unicode)]
    internal static extern bool SetupDiGetDeviceInterfaceDetail(
        HDEVINFO deviceInfoSet,
        ref SP_DEVINFO_DATA deviceInterfaceData,
        IntPtr deviceInterfaceDetailData,
        int deviceInterfaceDetailDataSize,
        ref int requiredSize,
        ref SP_DEVINFO_DATA deviceInfoData
    );

    [DllImport(nameof(SetupApi), SetLastError = true, CharSet = CharSet.Unicode)]
    internal static extern bool SetupDiOpenDeviceInfo(
        HDEVINFO deviceInfoSet,
        string deviceInstanceId,
        IntPtr hwndParent,
        int flags,
        ref SP_DEVINFO_DATA deviceInfoData
    );

    [DllImport(nameof(SetupApi), SetLastError = true, CharSet = CharSet.Unicode)]
    internal static extern unsafe bool SetupDiSetClassInstallParams(
        HDEVINFO deviceInfoSet,
        SP_DEVINFO_DATA* deviceInterfaceData,
        SP_REMOVEDEVICE_PARAMS* classInstallParams,
        int classInstallParamsSize
    );

    [DllImport(nameof(SetupApi), SetLastError = true, CharSet = CharSet.Unicode)]
    internal static extern unsafe bool SetupDiGetDeviceInstallParams(
        [In] HDEVINFO hDevInfo,
        [In] [Optional] SP_DEVINFO_DATA* deviceInfoData,
        [In] [Out] ref SP_DEVINSTALL_PARAMS deviceInstallParams
    );

    [DllImport(nameof(SetupApi), SetLastError = true, CharSet = CharSet.Unicode)]
    internal static extern unsafe bool SetupDiGetDeviceInstallParams(
        [In] HDEVINFO hDevInfo,
        [In] [Optional] SP_DEVINFO_DATA* deviceInfoData,
        [Out] IntPtr deviceInstallParams
    );

    [DllImport(nameof(SetupApi), CharSet = CharSet.Unicode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool SetupUninstallOEMInf(
        [MarshalAs(UnmanagedType.LPWStr)] string infName,
        uint flags,
        IntPtr reserved
    );

    [DllImport(nameof(SetupApi), CharSet = CharSet.Unicode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern unsafe bool SetupDiOpenDeviceInfo(
        [In] HDEVINFO deviceInfoSet,
        [In] [MarshalAs(UnmanagedType.LPWStr)] string deviceInstanceId,
        [In] [Optional] IntPtr parent,
        [In] uint openFlags,
        [Out] [Optional] SP_DEVINFO_DATA* deviceInfoData
    );

    [DllImport(nameof(SetupApi), CharSet = CharSet.Unicode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern unsafe bool SetupDiSetSelectedDevice(
        [In] HDEVINFO deviceInfoSet,
        [In] SP_DEVINFO_DATA* deviceInfoData
    );

    [DllImport(nameof(SetupApi), CharSet = CharSet.Unicode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern unsafe bool SetupDiSetDeviceInstallParams(
        [In] HDEVINFO deviceInfoSet,
        [In] [Optional] SP_DEVINFO_DATA* deviceInfoData,
        [In] ref SP_DEVINSTALL_PARAMS deviceInstallParams
    );

    [DllImport(nameof(SetupApi), CharSet = CharSet.Unicode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern unsafe bool SetupDiBuildDriverInfoList(
        [In] HDEVINFO deviceInfoSet,
        [In] [Out] SP_DEVINFO_DATA* deviceInfoData,
        [In] SETUP_DI_DRIVER_TYPE driverType
    );

    [DllImport(nameof(SetupApi), CharSet = CharSet.Unicode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern unsafe bool SetupDiEnumDriverInfo(
        [In] HDEVINFO deviceInfoSet,
        [In] [Optional] SP_DEVINFO_DATA* deviceInfoData,
        [In] SETUP_DI_DRIVER_TYPE driverType,
        [In] uint memberIndex,
        [In] [Out] ref SP_DRVINFO_DATA driverInfoData
    );

    [DllImport(nameof(SetupApi), CharSet = CharSet.Unicode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern unsafe bool SetupDiSetSelectedDriver(
        [In] HDEVINFO deviceInfoSet,
        [In] [Out] SP_DEVINFO_DATA* deviceInfoData,
        [In] [Out] ref SP_DRVINFO_DATA driverInfoData
    );

    [DllImport(nameof(SetupApi), CharSet = CharSet.Unicode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern unsafe bool SetupDiGetDeviceProperty(
        [In] HDEVINFO deviceInfoSet,
        [In] SP_DEVINFO_DATA* deviceInfoData,
        [In] DEVPROPKEY* propertyKey,
        [Out] out DEVPROPTYPE propertyType,
        [Out] [Optional] StringBuilder propertyBuffer,
        [In] uint propertyBufferSize,
        [Out] [Optional] [SuppressMessage("ReSharper", "OptionalParameterRefOut")]
        out uint requiredSize,
        [In] uint flags
    );

    [DllImport(nameof(SetupApi), CharSet = CharSet.Unicode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern unsafe bool SetupDiGetDriverInfoDetail(
        [In] HDEVINFO deviceInfoSet,
        [In] [Optional] SP_DEVINFO_DATA* deviceInfoData,
        [In] ref SP_DRVINFO_DATA driverInfoData,
        [In] [Out] ref SP_DRVINFO_DETAIL_DATA driverInfoDetailData,
        [In] uint driverInfoDetailDataSize,
        [Out] [Optional] [SuppressMessage("ReSharper", "OptionalParameterRefOut")]
        out uint requiredSize
    );

    [DllImport(nameof(SetupApi), CharSet = CharSet.Unicode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern unsafe bool SetupDiGetDriverInfoDetail(
        [In] HDEVINFO deviceInfoSet,
        [In] [Optional] SP_DEVINFO_DATA* deviceInfoData,
        [In] ref SP_DRVINFO_DATA driverInfoData,
        [In] [Out] IntPtr driverInfoDetailData,
        [In] uint driverInfoDetailDataSize,
        [Out] [Optional] [SuppressMessage("ReSharper", "OptionalParameterRefOut")]
        out uint requiredSize
    );

    [DllImport(nameof(SetupApi), CharSet = CharSet.Unicode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern unsafe bool SetupDiDestroyDriverInfoList(
        [In] HDEVINFO deviceInfoSet,
        [In] [Optional] SP_DEVINFO_DATA* deviceInfoData,
        [In] SETUP_DI_DRIVER_TYPE driverType
    );

    [DllImport(nameof(SetupApi), CharSet = CharSet.Unicode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool SetupDiDestroyDriverInfoList(
        [In] HDEVINFO deviceInfoSet,
        [In] ref SP_DEVINFO_DATA deviceInfoData,
        [In] SETUP_DI_DRIVER_TYPE driverType
    );

    #endregion

    #region Newdev

    [DllImport("newdev.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    internal static extern bool DiInstallDriver(
        [In] [Optional] HWND hwndParent,
        [In] string fullInfPath,
        [In] uint flags,
        [Out] [Optional] [SuppressMessage("ReSharper", "OptionalParameterRefOut")]
        out bool needReboot);

    [DllImport("newdev.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    internal static extern bool DiUninstallDriver(
        [In] HWND hwndParent,
        [In] string infPath,
        [In] uint flags,
        [Out] out bool needReboot
    );

    [DllImport("newdev.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    internal static extern unsafe bool DiInstallDevice(
        [In] [Optional] HWND hwndParent,
        [In] HDEVINFO deviceInfoSet,
        [In] SP_DEVINFO_DATA* deviceInfoData,
        [In] ref SP_DRVINFO_DATA driverInfoData,
        [In] uint flags,
        [Out] out bool needReboot
    );

    [DllImport("newdev.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    internal static extern unsafe bool DiInstallDevice(
        [In] [Optional] HWND hwndParent,
        [In] HDEVINFO deviceInfoSet,
        [In] SP_DEVINFO_DATA* deviceInfoData,
        [In] [Optional] IntPtr driverInfoData,
        [In] uint flags,
        [Out] out bool needReboot
    );

    [DllImport("newdev.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    internal static extern unsafe bool DiUninstallDevice(
        [In] [Optional] HWND hwndParent,
        [In] HDEVINFO deviceInfoSet,
        [In] SP_DEVINFO_DATA* deviceInfoData,
        [In] uint flags,
        [Out] out bool needReboot
    );

    [DllImport("newdev.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    internal static extern bool DiUninstallDevice(
        [In] [Optional] HWND hwndParent,
        [In] HDEVINFO deviceInfoSet,
        [In] ref SP_DEVINFO_DATA deviceInfoData,
        [In] uint flags,
        [Out] out bool needReboot
    );

    #endregion

    #endregion
}