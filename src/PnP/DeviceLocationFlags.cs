namespace Nefarius.Utilities.DeviceManagement.PnP;

/// <summary>
///     A variable of ULONG type that supplies one of the following flag values that apply if the caller supplies a device
///     instance identifier:
///     CM_LOCATE_DEVNODE_NORMAL
///     The function retrieves the device instance handle for the specified device only if the device is currently
///     configured in the device tree.
///     CM_LOCATE_DEVNODE_PHANTOM
///     The function retrieves a device instance handle for the specified device if the device is currently configured in
///     the device tree or the device is a nonpresent device that is not currently configured in the device tree.
///     CM_LOCATE_DEVNODE_CANCELREMOVE
///     The function retrieves a device instance handle for the specified device if the device is currently configured in
///     the device tree or in the process of being removed from the device tree. If the device is in the process of being
///     removed, the function cancels the removal of the device.
/// </summary>
public enum DeviceLocationFlags
{
    /// <summary>
    ///     The function retrieves the device instance handle for the specified device only if the device is currently
    ///     configured in the device tree.
    /// </summary>
    Normal,

    /// <summary>
    ///     The function retrieves a device instance handle for the specified device if the device is currently configured in
    ///     the device tree or the device is a nonpresent device that is not currently configured in the device tree.
    /// </summary>
    Phantom,

    /// <summary>
    ///     The function retrieves a device instance handle for the specified device if the device is currently configured in
    ///     the device tree or in the process of being removed from the device tree. If the device is in the process of being
    ///     removed, the function cancels the removal of the device.
    /// </summary>
    CancelRemove
}