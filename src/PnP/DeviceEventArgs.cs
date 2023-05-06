using System;

namespace Nefarius.Utilities.DeviceManagement.PnP;

/// <summary>
///     Device change event arguments.
/// </summary>
public class DeviceEventArgs
{
    /// <summary>
    ///     The <see cref="Guid" /> of the device interface.
    /// </summary>
    public Guid InterfaceGuid { get; set; }

    /// <summary>
    ///     The symbolic link path.
    /// </summary>
    public string SymLink { get; set; }
}