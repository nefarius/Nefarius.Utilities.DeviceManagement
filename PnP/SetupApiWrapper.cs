using System;
using System.Runtime.InteropServices;

namespace Nefarius.Utilities.DeviceManagement.PnP
{
    internal static class SetupApiWrapper
    {
        #region Constant and Structure Definitions

        [StructLayout(LayoutKind.Sequential)]
        internal struct SP_DEVINFO_DATA
        {
            internal int cbSize;
            internal readonly Guid ClassGuid;
            internal readonly uint DevInst;
            internal readonly IntPtr Reserved;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct SP_CLASSINSTALL_HEADER
        {
            internal int cbSize;
            internal int InstallFunction;
        }
        
        [StructLayout(LayoutKind.Sequential)]
        internal struct SP_REMOVEDEVICE_PARAMS
        {
            internal SP_CLASSINSTALL_HEADER ClassInstallHeader;
            internal int Scope;
            internal int HwProfile;
        }
        
        [StructLayout(LayoutKind.Sequential)]
        internal struct DevPropKey
        {
            public Guid fmtid;
            public uint pid;

            public DevPropKey(Guid fmtid, uint pid)
            {
                this.fmtid = fmtid;
                this.pid = pid;
            }

            public DevPropKey(uint a, ushort b, ushort c, byte d, byte e, byte f, byte g, byte h, byte i, byte j, byte k, uint pid)
            {
                this.fmtid = new Guid(a, b, c, d, e, f, g, h, i, j, k);
                this.pid = pid;
            }
        }

        internal enum CM_GET_DEVICE_INTERFACE_LIST_FLAG : uint
        {
            CM_GET_DEVICE_INTERFACE_LIST_ALL_DEVICES = 0x00000001,
            CM_GET_DEVICE_INTERFACE_LIST_PRESENT = 0x00000000
        }

        internal enum ConfigManagerResult : uint
        {
            Success = 0x00000000,
            Default = 0x00000001,
            OutOfMemory = 0x00000002,
            InvalidPointer = 0x00000003,
            InvalidFlag = 0x00000004,
            InvalidDevnode = 0x00000005,
            InvalidDevinst = InvalidDevnode,
            InvalidResDes = 0x00000006,
            InvalidLogConf = 0x00000007,
            InvalidArbitrator = 0x00000008,
            InvalidNodelist = 0x00000009,
            DevnodeHasReqs = 0x0000000A,
            DevinstHasReqs = DevnodeHasReqs,
            InvalidResourceid = 0x0000000B,
            NoSuchDevnode = 0x0000000D,
            NoSuchDevinst = NoSuchDevnode,
            NoMoreLogConf = 0x0000000E,
            NoMoreResDes = 0x0000000F,
            AlreadySuchDevnode = 0x00000010,
            AlreadySuchDevinst = AlreadySuchDevnode,
            InvalidRangeList = 0x00000011,
            InvalidRange = 0x00000012,
            Failure = 0x00000013,
            NoSuchLogicalDev = 0x00000014,
            CreateBlocked = 0x00000015,
            RemoveVetoed = 0x00000017,
            ApmVetoed = 0x00000018,
            InvalidLoadType = 0x00000019,
            BufferSmall = 0x0000001A,
            NoArbitrator = 0x0000001B,
            NoRegistryHandle = 0x0000001C,
            RegistryError = 0x0000001D,
            InvalidDeviceId = 0x0000001E,
            InvalidData = 0x0000001F,
            InvalidApi = 0x00000020,
            DevloaderNotReady = 0x00000021,
            NeedRestart = 0x00000022,
            NoMoreHwProfiles = 0x00000023,
            DeviceNotThere = 0x00000024,
            NoSuchValue = 0x00000025,
            WrongType = 0x00000026,
            InvalidPriority = 0x00000027,
            NotDisableable = 0x00000028,
            FreeResources = 0x00000029,
            QueryVetoed = 0x0000002A,
            CantShareIrq = 0x0000002B,
            NoDependent = 0x0000002C,
            SameResources = 0x0000002D,
            NoSuchRegistryKey = 0x0000002E,
            InvalidMachinename = 0x0000002F,   // NT ONLY
            RemoteCommFailure = 0x00000030,   // NT ONLY
            MachineUnavailable = 0x00000031,   // NT ONLY
            NoCmServices = 0x00000032,   // NT ONLY
            AccessDenied = 0x00000033,   // NT ONLY
            CallNotImplemented = 0x00000034,
            InvalidProperty = 0x00000035,
            DeviceInterfaceActive = 0x00000036,
            NoSuchDeviceInterface = 0x00000037,
            InvalidReferenceString = 0x00000038,
            InvalidConflictList = 0x00000039,
            InvalidIndex = 0x0000003A,
            InvalidStructureSize = 0x0000003B
        }

        internal enum DevPropType : uint
        {
            TYPEMOD_ARRAY = 0x00001000,  // array of fixed-sized data elements
            TYPEMOD_LIST = 0x00002000,  // list of variable-sized data elements

            //
            // Property data types.
            //
            Empty = 0x00000000, // nothing, no property data
            Null = 0x00000001, // null property data
            Sbyte = 0x00000002, // 8-bit signed int (sbyte)
            Byte = 0x00000003, // 8-bit unsigned int (byte)
            Int16 = 0x00000004, // 16-bit signed int (short)
            Uint16 = 0x00000005, // 16-bit unsigned int (ushort)
            Int32 = 0x00000006, // 32-bit signed int (long)
            Uint32 = 0x00000007, // 32-bit unsigned int (ulong)
            Int64 = 0x00000008, // 64-bit signed int (long64)
            Uint64 = 0x00000009, // 64-bit unsigned int (ulong64)
            Float = 0x0000000a, // 32-bit floating-point (float)
            Double = 0x0000000b, // 64-bit floating-point (double)
            Decimal = 0x0000000c, // 128-bit data (decimal)
            Guid = 0x0000000d, // 128-bit unique identifier (guid)
            Currency = 0x0000000e, // 64 bit signed int currency value (currency)
            Date = 0x0000000f, // date (date)
            FileTime = 0x00000010, // file time (filetime)
            Boolean = 0x00000011, // 8-bit boolean (devprop_boolean)
            String = 0x00000012, // null-terminated string
            StringList = (String | TYPEMOD_LIST), // multi-sz string list
            SecurityDescriptor = 0x00000013, // self-relative binary security_descriptor
            SecurityDescriptorString = 0x00000014, // security descriptor string (sddl format)
            Devpropkey = 0x00000015, // device property key (devpropkey)
            Devproptype = 0x00000016, // device property type (devproptype)
            Binary = (Byte | TYPEMOD_ARRAY), // custom binary data
            Error = 0x00000017, // 32-bit win32 system error code
            Ntstatus = 0x00000018, // 32-bit ntstatus code
            StringIndirect = 0x00000019, // string resource (@[path\]<dllname>,-<strid>)
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct SP_DRVINFO_DATA
        {
            internal readonly uint cbSize;
            internal readonly uint DriverType;
            internal readonly IntPtr Reserved;
            internal readonly string Description;
            internal readonly string MfgName;
            internal readonly string ProviderName;
            internal readonly DateTime DriverDate;
            internal readonly ulong DriverVersion;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct SP_DEVICE_INTERFACE_DATA
        {
            internal Int32    cbSize;
            internal Guid     interfaceClassGuid;
            internal Int32    flags;
            internal UIntPtr  reserved;
        }
        
        /// <summary>
        ///     Flags for DiUninstallDriver
        /// </summary>
        [Flags]
        internal enum DIURFLAG
        {
            NO_REMOVE_INF = 0x00000001, // Do not remove inf from the system
            UNCONFIGURE_INF = 0x00000002 // Unconfigure inf, if possible
        }

        [Flags]
        internal enum SetupUOInfFlags : uint
        {
            NONE = 0x0000,
            SUOI_FORCEDELETE = 0x0001
        }

        #endregion

        #region Interop Definitions

        #region SetupAPI

        [DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern IntPtr SetupDiCreateDeviceInfoList(ref Guid ClassGuid, IntPtr hwndParent);

        [DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern bool SetupDiDestroyDeviceInfoList(IntPtr DeviceInfoSet);

        [DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern bool SetupDiCreateDeviceInfo(IntPtr DeviceInfoSet, string DeviceName, ref Guid ClassGuid,
            string DeviceDescription, IntPtr hwndParent, int CreationFlags, ref SP_DEVINFO_DATA DeviceInfoData);

        [DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern bool SetupDiSetDeviceRegistryProperty(IntPtr DeviceInfoSet,
            ref SP_DEVINFO_DATA DeviceInfoData, int Property, [MarshalAs(UnmanagedType.LPWStr)] string PropertyBuffer,
            int PropertyBufferSize);

        [DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern bool SetupDiCallClassInstaller(int InstallFunction, IntPtr DeviceInfoSet,
            ref SP_DEVINFO_DATA DeviceInfoData);
        
        [DllImport("setupapi.dll", SetLastError = true)]
        internal static extern bool SetupDiEnumDeviceInfo(IntPtr deviceInfoSet,
            UInt32 memberIndex,
            ref SP_DEVINFO_DATA deviceInfoData);

        [DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern IntPtr SetupDiGetClassDevs(ref Guid ClassGuid, IntPtr Enumerator, IntPtr hwndParent,
            int Flags);

        [DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern bool SetupDiEnumDeviceInterfaces(IntPtr DeviceInfoSet, IntPtr DeviceInfoData,
            ref Guid InterfaceClassGuid, int MemberIndex, ref SP_DEVINFO_DATA DeviceInterfaceData);

        [DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern bool SetupDiGetDeviceInterfaceDetail(IntPtr DeviceInfoSet,
            ref SP_DEVINFO_DATA DeviceInterfaceData, IntPtr DeviceInterfaceDetailData,
            int DeviceInterfaceDetailDataSize,
            ref int RequiredSize, ref SP_DEVINFO_DATA DeviceInfoData);

        [DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern bool SetupDiOpenDeviceInfo(IntPtr DeviceInfoSet, string DeviceInstanceId,
            IntPtr hwndParent, int Flags, ref SP_DEVINFO_DATA DeviceInfoData);
        
        [DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern bool SetupDiSetClassInstallParams(IntPtr DeviceInfoSet,
            ref SP_DEVINFO_DATA DeviceInterfaceData, ref SP_REMOVEDEVICE_PARAMS ClassInstallParams,
            int ClassInstallParamsSize);
        
        [DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern bool SetupDiGetDeviceInstallParams(
            IntPtr hDevInfo,
            ref SP_DEVINFO_DATA DeviceInfoData,
            IntPtr DeviceInstallParams
        );

        [DllImport("setupapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool SetupUninstallOEMInf(
            [MarshalAs(UnmanagedType.LPWStr)] string infName,
            SetupUOInfFlags flags,
            IntPtr reserved);

        #endregion

        #region Cfgmgr32

        [DllImport("CfgMgr32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern ConfigManagerResult CM_Get_DevNode_Property(
            uint devInst,
            ref DevPropKey propertyKey,
            out DevPropType propertyType,
            IntPtr buffer,
            ref uint bufferSize,
            uint flags
            );

        [DllImport("Cfgmgr32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern ConfigManagerResult CM_Set_DevNode_Property(
            uint devInst,
            ref DevPropKey PropertyKey,
            DevPropType PropertyType,
            IntPtr PropertyBuffer,
            uint PropertyBufferSize,
            uint ulFlags // reserved
        );

        #endregion

        #region Newdev

        [DllImport("newdev.dll", SetLastError = true)]
        internal static extern bool DiInstallDriver(
            IntPtr hwndParent,
            string FullInfPath,
            uint Flags,
            out bool NeedReboot);

        [DllImport("newdev.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern bool DiUninstallDriver(
            [In] IntPtr hwndParent,
            [In] string infPath,
            [In] DIURFLAG flags,
            [Out] out bool needReboot
        );

        #endregion

        #endregion
    }
}
