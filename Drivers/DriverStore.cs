/*

Copyright (c) 2017-2021, The LumiaWOA Authors

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

*/

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Nefarius.Utilities.DeviceManagement.Drivers
{
    [Flags]
    internal enum DriverStoreOpenFlag : uint
    {
        None = 0,
        Create = 1,
        Exclusive = 2
    }

    internal enum ProcessorArchitecture : ushort
    {
        PROCESSOR_ARCHITECTURE_INTEL = 0,
        PROCESSOR_ARCHITECTURE_MIPS = 1,
        PROCESSOR_ARCHITECTURE_ALPHA = 2,
        PROCESSOR_ARCHITECTURE_PPC = 3,
        PROCESSOR_ARCHITECTURE_SHX = 4,
        PROCESSOR_ARCHITECTURE_ARM = 5,
        PROCESSOR_ARCHITECTURE_IA64 = 6,
        PROCESSOR_ARCHITECTURE_ALPHA64 = 7,
        PROCESSOR_ARCHITECTURE_MSIL = 8,
        PROCESSOR_ARCHITECTURE_AMD64 = 9,
        PROCESSOR_ARCHITECTURE_IA32_ON_WIN64 = 10,
        PROCESSOR_ARCHITECTURE_NEUTRAL = 11,
        PROCESSOR_ARCHITECTURE_ARM64 = 12,
        PROCESSOR_ARCHITECTURE_ARM32_ON_WIN64 = 13,
        PROCESSOR_ARCHITECTURE_IA32_ON_ARM64 = 14,
    }

    [Flags]
    internal enum DriverStoreImportFlag : uint
    {
        None = 0,
        SkipTempCopy = 1,
        SkipExternalFileCheck = 2,
        NoRestorePoint = 4,
        NonInteractive = 8,
        Replace = 32,
        Hardlink = 64,
        PublishSameName = 256,
        Inbox = 512,
        F6 = 1024,
        BaseVersion = 2048,
        SystemDefaultLocale = 4096,
        SystemCritical = 8192
    }

    [Flags]
    internal enum DriverStoreOfflineAddDriverPackageFlags : uint
    {
        None = 0,
        SkipInstall = 1,
        Inbox = 2,
        F6 = 4,
        SkipExternalFilePresenceCheck = 8,
        NoTempCopy = 16,
        UseHardLinks = 32,
        InstallOnly = 64,
        ReplacePackage = 128,
        Force = 256,
        BaseVersion = 512
    }

    [Flags]
    internal enum DriverStoreConfigureFlags : uint
    {
        None = 0,
        Force = 1,
        ActiveOnly = 2,
        SourceConfigurations = 65536,
        SourceDeviceIds = 131072,
        TargetDeviceNodes = 1048576
    }

    [Flags]
    internal enum DriverStoreReflectCriticalFlag : uint
    {
        None = 0,
        Force = 1,
        Configurations = 2
    }

    [Flags]
    internal enum DriverStoreReflectFlag : uint
    {
        None = 0,
        FilesOnly = 1,
        ActiveDrivers = 2,
        ExternalOnly = 4,
        Configurations = 8
    }

    [Flags]
    internal enum DriverStorePublishFlag : uint
    {
        None = 0
    }

    [Flags]
    internal enum DriverStoreUnpublishFlag : uint
    {
        None = 0
    }

    internal enum DriverStoreObjectType : uint
    {
        DriverDatabase = 1
    }

    internal struct DevPropKey
    {
        public Guid fmtid;

        public uint pid;
    }

    internal enum DevPropType : uint
    {
        DevPropTypeUint32 = 7,
        DevPropTypeUint64 = 9,
        DevPropTypeString = 18
    }

    [Flags]
    internal enum DriverStoreSetObjectPropertyFlag : uint
    {
        None = 0
    }

    [Flags]
    internal enum DriverPackageEnumFilesFlag : uint
    {
        Copy = 1,
        Delete = 2,
        Rename = 4,
        Inf = 16,
        Catalog = 32,
        Binaries = 64,
        CopyInfs = 128,
        IncludeInfs = 256,
        External = 4096,
        UniqueSource = 8192,
        UniqueDestination = 16384
    }

    [Flags]
    internal enum DriverPackageOpenFlag : uint
    {
        VersionOnly = 1,
        FilesOnly = 2,
        DefaultLanguage = 4,
        LocalizableStrings = 8,
        TargetOSVersion = 16,
        StrictValidation = 32,
        ClassSchemaOnly = 64,
        LogTelemetry = 128,
        PrimaryOnly = 256
    }

    internal enum DriverPackageGetPropertyFlag : uint
    {
        None
    }

    [Flags]
    internal enum DriverStoreCopyFlag : uint
    {
        None = 0,
        External = 1,
        CopyInfs = 2,
        SkipExistingCopyInfs = 4,
        SystemDefaultLocale = 8,
        Hardlink = 16
    }

    public static class DriverStore
    {
        public static IEnumerable<string> DriverStoreEntries
        {
            get
            {
                List<string> existingDrivers = new();

                uint ntStatus = DriverStoreNative.DriverStoreOfflineEnumDriverPackage(
                    (
                        string DriverPackageInfPath,
                        IntPtr Ptr,
                        IntPtr Unknown
                    ) =>
                    {
                        DriverStoreNative.DriverStoreOfflineEnumDriverPackageInfo
                            DriverStoreOfflineEnumDriverPackageInfoW =
                                (DriverStoreNative.DriverStoreOfflineEnumDriverPackageInfo)Marshal.PtrToStructure(Ptr,
                                    typeof(DriverStoreNative.DriverStoreOfflineEnumDriverPackageInfo));
                        Console.Title =
                            $"Driver Updater - DriverStoreOfflineEnumDriverPackage - {DriverPackageInfPath}";
                        if (DriverStoreOfflineEnumDriverPackageInfoW.InboxInf == 0)
                        {
                            existingDrivers.Add(DriverPackageInfPath);
                        }

                        return 1;
                    }
                    , IntPtr.Zero, Environment.GetEnvironmentVariable("%WINDIR%"));

                return existingDrivers;
            }
        }
    }

    internal static class DriverStoreNative
    {
        [DllImport("drvstore.dll", CharSet = CharSet.Unicode, EntryPoint = "DriverStoreDeleteW", SetLastError = true)]
        internal static extern uint DriverStoreDelete(
            IntPtr driverStoreHandle,
            string DriverStoreFilename,
            uint Flags);

        [DllImport("drvstore.dll", CharSet = CharSet.Unicode, EntryPoint = "DriverStoreOfflineDeleteDriverPackageW", SetLastError = true)]
        internal static extern uint DriverStoreOfflineDeleteDriverPackage(
            string DriverPackageInfPath,
            uint Flags,
            IntPtr Reserved,
            string TargetSystemRoot,
            string TargetSystemDrive);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Size = 0x2B8, Pack = 0x4)]
        public struct DriverStoreOfflineEnumDriverPackageInfo
        {
            public int InboxInf;

            public ProcessorArchitecture ProcessorArchitecture;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 85)]
            public string LocaleName;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string PublishedInfName;
        };

        public delegate int CallbackRoutine(
            [MarshalAs(UnmanagedType.LPWStr)]
            string DriverPackageInfPath,
            IntPtr DriverStoreOfflineEnumDriverPackageInfo,
            IntPtr Unknown);

        [DllImport("drvstore.dll", EntryPoint = "DriverStoreOfflineEnumDriverPackageW", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern uint DriverStoreOfflineEnumDriverPackage(CallbackRoutine CallbackRoutine, IntPtr lParam, string TargetSystemRoot);

        [DllImport("drvstore.dll", EntryPoint = "DriverStoreUnreflectCriticalW", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern uint DriverStoreUnreflectCritical(
            IntPtr driverStoreHandle,
            string DriverStoreFileName,
            uint Flags,
            string FilterDeviceIds);

        [DllImport("drvstore.dll", CharSet = CharSet.Unicode, EntryPoint = "DriverStoreOpenW", SetLastError = true)]
        internal static extern IntPtr DriverStoreOpen(
            string targetSystemPath,
            string targetBootDrive,
            DriverStoreOpenFlag Flags,
            IntPtr transactionHandle);

        [DllImport("drvstore.dll", SetLastError = true)]
        internal static extern bool DriverStoreClose(IntPtr driverStoreHandle);

        [DllImport("drvstore.dll", CharSet = CharSet.Unicode, EntryPoint = "DriverStoreImportW", SetLastError = true)]
        internal static extern uint DriverStoreImport(
            IntPtr driverStoreHandle,
            string driverPackageFileName,
            ProcessorArchitecture ProcessorArchitecture,
            string localeName,
            DriverStoreImportFlag flags,
            StringBuilder driverStoreFileName,
            int driverStoreFileNameSize);

        [DllImport("drvstore.dll", CharSet = CharSet.Unicode, EntryPoint = "DriverStoreOfflineAddDriverPackageW", SetLastError = true)]
        internal static extern uint DriverStoreOfflineAddDriverPackage(
            string DriverPackageInfPath,
            DriverStoreOfflineAddDriverPackageFlags Flags,
            IntPtr Reserved,
            ProcessorArchitecture ProcessorArchitecture,
            string LocaleName,
            StringBuilder DestInfPath,
            ref int cchDestInfPath,
            string TargetSystemRoot,
            string TargetSystemDrive);

        [DllImport("drvstore.dll", CharSet = CharSet.Unicode, EntryPoint = "DriverStoreConfigureW", SetLastError = true)]
        internal static extern uint DriverStoreConfigure(
            IntPtr hDriverStore,
            string DriverStoreFilename,
            DriverStoreConfigureFlags Flags,
            string SourceFilter,
            string TargetFilter);

        [DllImport("drvstore.dll", CharSet = CharSet.Unicode, EntryPoint = "DriverStoreReflectCriticalW", SetLastError = true)]
        internal static extern uint DriverStoreReflectCritical(
            IntPtr driverStoreHandle,
            string driverStoreFileName,
            DriverStoreReflectCriticalFlag flag,
            string filterDeviceId);

        [DllImport("drvstore.dll", CharSet = CharSet.Unicode, EntryPoint = "DriverStoreReflectW", SetLastError = true)]
        internal static extern uint DriverStoreReflect(
            IntPtr driverStoreHandle,
            string driverStoreFileName,
            DriverStoreReflectFlag flag,
            string filterSectionNames);

        [DllImport("drvstore.dll", CharSet = CharSet.Unicode, EntryPoint = "DriverStorePublishW", SetLastError = true)]
        internal static extern uint DriverStorePublish(
            IntPtr driverStoreHandle,
            string driverStoreFileName,
            DriverStorePublishFlag flag,
            StringBuilder publishedFileName,
            int publishedFileNameSize,
            ref bool isPublishedFileNameChanged);

        [DllImport("drvstore.dll", CharSet = CharSet.Unicode, EntryPoint = "DriverStoreUnpublishW", SetLastError = true)]
        internal static extern uint DriverStoreUnpublish(
            IntPtr driverStoreHandle,
            string driverStoreFileName,
            DriverStoreUnpublishFlag flag,
            StringBuilder publishedFileName,
            int publishedFileNameSize,
            ref bool isPublishedFileNameChanged);

        [DllImport("drvstore.dll", CharSet = CharSet.Unicode, EntryPoint = "DriverStoreSetObjectPropertyW", SetLastError = true)]
        internal static extern bool DriverStoreSetObjectProperty(
            IntPtr driverStoreHandle,
            DriverStoreObjectType objectType,
            string objectName,
            ref DevPropKey propertyKey,
            DevPropType propertyType,
            ref uint propertyBuffer,
            int propertySize,
            DriverStoreSetObjectPropertyFlag flag);

        [DllImport("drvstore.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern bool DriverPackageEnumFilesW(
            IntPtr driverPackageHandle,
            IntPtr enumContext,
            DriverPackageEnumFilesFlag flags,
            EnumFilesDelegate callbackRoutine,
            IntPtr lParam);

        [DllImport("drvstore.dll", CharSet = CharSet.Unicode, EntryPoint = "DriverPackageOpenW", SetLastError = true)]
        internal static extern IntPtr DriverPackageOpen(
            string driverPackageFilename,
            ProcessorArchitecture processorArchitecture,
            string localeName,
            DriverPackageOpenFlag flags,
            IntPtr resolveContext);

        [DllImport("drvstore.dll", CharSet = CharSet.Unicode, EntryPoint = "DriverPackageGetVersionInfoW", SetLastError = true)]
        internal static extern bool DriverPackageGetVersionInfo(
            IntPtr driverPackageHandle,
            IntPtr pVersionInfo);

        [DllImport("drvstore.dll", CharSet = CharSet.Unicode, EntryPoint = "DriverPackageGetPropertyW", SetLastError = true)]
        internal static extern bool DriverPackageGetProperty(
            IntPtr driverPackageHandle,
            IntPtr enumContext,
            string sectionName,
            IntPtr propertyKey,
            IntPtr propertyType,
            IntPtr propertyBuffer,
            uint bufferSize,
            IntPtr propertySize,
            DriverPackageGetPropertyFlag flags);

        [DllImport("drvstore.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern void DriverPackageClose(IntPtr driverPackageHandle);

        [DllImport("drvstore.dll", CharSet = CharSet.Unicode, EntryPoint = "DriverStoreCopyW", SetLastError = true)]
        internal static extern uint DriverStoreCopy(
            IntPtr driverPackageHandle,
            string driverPackageFilename,
            ProcessorArchitecture processorArchitecture,
            IntPtr localeName,
            DriverStoreCopyFlag flags,
            string destinationPath);

        public delegate bool EnumFilesDelegate(IntPtr driverPackageHandle, IntPtr pDriverFile, IntPtr lParam);
    }
}