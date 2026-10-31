using System;
using System.Collections.Generic;

namespace Nefarius.Utilities.DeviceManagement.Drivers;

/// <summary>
///     A read-only description of a driver package staged in the Windows driver store.
/// </summary>
/// <param name="PublishedName">
///     The published INF name: <c>oemNN.inf</c> for third-party packages, the plain INF file name for inbox packages.
///     May be empty if the driver store reports none.
/// </param>
/// <param name="StoreInfPath">The absolute path of the staged INF inside the driver store file repository.</param>
/// <param name="OriginalInfName">
///     The original INF file name (e.g. <c>mydriver.inf</c>) derived from the package directory name, or
///     <c>null</c> if it could not be determined.
/// </param>
/// <param name="Provider">The <c>Provider</c> value of the INF <c>[Version]</c> section, if readable.</param>
/// <param name="ClassName">The device setup class name, if known.</param>
/// <param name="ClassGuid">The device setup class GUID, or <see cref="Guid.Empty" /> if unknown.</param>
/// <param name="DriverVersion">The driver version from the INF <c>DriverVer</c> directive, if parsable.</param>
/// <param name="DriverDate">The driver date from the INF <c>DriverVer</c> directive, if parsable.</param>
/// <param name="Architecture">
///     The target architecture of the package (e.g. <c>amd64</c>, <c>x86</c>, <c>arm64</c>, <c>neutral</c>).
/// </param>
/// <param name="IsInbox">True if the package ships with Windows (inbox), false for third-party packages.</param>
/// <param name="BoundDeviceInstanceIds">
///     Instance IDs of devices (including phantom ones) whose installed driver INF matches this package.
/// </param>
public sealed record DriverStorePackage(
    string PublishedName,
    string StoreInfPath,
    string? OriginalInfName,
    string? Provider,
    string? ClassName,
    Guid ClassGuid,
    Version? DriverVersion,
    DateTime? DriverDate,
    string Architecture,
    bool IsInbox,
    IReadOnlyList<string> BoundDeviceInstanceIds);