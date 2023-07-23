using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text;

using Windows.Win32.Devices.DeviceAndDriverInstallation;
using Windows.Win32.Devices.Properties;
using Windows.Win32.Foundation;

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
    internal static unsafe extern HDEVINFO SetupDiCreateDeviceInfoList(
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
        UInt32 memberIndex,
        ref SP_DEVINFO_DATA deviceInfoData
    );

    [DllImport(nameof(SetupApi), SetLastError = true)]
    internal static extern unsafe bool SetupDiEnumDeviceInfo(
        HDEVINFO deviceInfoSet,
        UInt32 memberIndex,
        SP_DEVINFO_DATA* deviceInfoData
    );

    [DllImport(nameof(SetupApi), SetLastError = true, CharSet = CharSet.Unicode)]
    internal static extern HDEVINFO SetupDiGetClassDevs(
        ref Guid classGuid,
        IntPtr enumerator,
        HWND hwndParent,
        UInt32 flags
    );

    [DllImport(nameof(SetupApi), SetLastError = true, CharSet = CharSet.Unicode)]
    internal static extern unsafe HDEVINFO SetupDiGetClassDevs(
        Guid* classGuid,
        string enumerator,
        HWND hwndParent,
        UInt32 flags
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
    public static unsafe extern bool SetupDiOpenDeviceInfo(
        [In] HDEVINFO deviceInfoSet,
        [In] [MarshalAs(UnmanagedType.LPWStr)] string deviceInstanceId,
        [In] [Optional] IntPtr parent,
        [In] UInt32 openFlags,
        [Out] [Optional] SP_DEVINFO_DATA* deviceInfoData
    );

    [DllImport(nameof(SetupApi), CharSet = CharSet.Unicode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static unsafe extern bool SetupDiSetSelectedDevice(
        [In] HDEVINFO deviceInfoSet,
        [In] SP_DEVINFO_DATA* deviceInfoData
    );

    [DllImport(nameof(SetupApi), CharSet = CharSet.Unicode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static unsafe extern bool SetupDiSetDeviceInstallParams(
        [In] HDEVINFO deviceInfoSet,
        [In] [Optional] SP_DEVINFO_DATA* deviceInfoData,
        [In] ref SP_DEVINSTALL_PARAMS deviceInstallParams
    );

    [DllImport(nameof(SetupApi), CharSet = CharSet.Unicode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static unsafe extern bool SetupDiBuildDriverInfoList(
        [In] HDEVINFO deviceInfoSet,
        [In] [Out] SP_DEVINFO_DATA* deviceInfoData,
        [In] SETUP_DI_BUILD_DRIVER_DRIVER_TYPE driverType
    );

    [DllImport(nameof(SetupApi), CharSet = CharSet.Unicode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static unsafe extern bool SetupDiEnumDriverInfo(
        [In] HDEVINFO deviceInfoSet,
        [In] [Optional] SP_DEVINFO_DATA* deviceInfoData,
        [In] SETUP_DI_BUILD_DRIVER_DRIVER_TYPE driverType,
        [In] UInt32 memberIndex,
        [In] [Out] ref SP_DRVINFO_DATA driverInfoData
    );

    [DllImport(nameof(SetupApi), CharSet = CharSet.Unicode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static unsafe extern bool SetupDiSetSelectedDriver(
        [In] HDEVINFO deviceInfoSet,
        [In] [Out] SP_DEVINFO_DATA* deviceInfoData,
        [In] [Out] ref SP_DRVINFO_DATA driverInfoData
    );

    [DllImport(nameof(SetupApi), CharSet = CharSet.Unicode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static unsafe extern bool SetupDiGetDeviceProperty(
        [In] HDEVINFO deviceInfoSet,
        [In] SP_DEVINFO_DATA* deviceInfoData,
        [In] DEVPROPKEY* propertyKey,
        [Out] out DEVPROPTYPE propertyType,
        [Out] [Optional] StringBuilder propertyBuffer,
        [In] UInt32 propertyBufferSize,
        [Out] [Optional] out UInt32 requiredSize,
        [In] UInt32 flags
    );

    #endregion

    #region Newdev

    [DllImport("newdev.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    internal static extern bool DiInstallDriver(
        [In] [Optional] HWND hwndParent,
        [In] string fullInfPath,
        [In] uint flags,
        [Out] [Optional] out bool needReboot);

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
#pragma warning disable CS8500
        [In] [Optional] SP_DRVINFO_DATA* driverInfoData,
#pragma warning restore CS8500
        [In] uint flags,
        [Out] out bool needReboot
    );

    #endregion

    #endregion
}