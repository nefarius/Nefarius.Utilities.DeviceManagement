using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

using Windows.Win32;

namespace Nefarius.Utilities.DeviceManagement.PnP;

/// <summary>
///     Provides common device interface <see cref="Guid" />s.
/// </summary>
[SuppressMessage("ReSharper", "UnusedMember.Global")]
public static class DeviceInterfaceIds
{
    /// <summary>
    ///     An interface exposed on USB host controllers.
    /// </summary>
    public static Guid UsbHostController => Guid.Parse("{3abf6f2d-71c4-462a-8a92-1e6861e6af27}");

    /// <summary>
    ///     An interface exposed on USB hubs.
    /// </summary>
    public static Guid UsbHub => Guid.Parse("{f18a0e88-c30c-11d0-8815-00a0c906bed8}");

    /// <summary>
    ///     An interface exposed on USB devices.
    /// </summary>
    public static Guid UsbDevice => Guid.Parse("{a5dcbf10-6530-11d2-901f-00c04fb951ed}");

    /// <summary>
    ///     An interface exposed on XUSB (Xbox 360) or XGIP (Xbox One) compatible (XInput) devices.
    /// </summary>
    public static Guid XUsbDevice => Guid.Parse("{EC87F1E3-C13B-4100-B5F7-8B84D54260CB}");

    /// <summary>
    ///     An interface exposed on HID devices.
    /// </summary>
    public static Guid HidDevice
    {
        get
        {
            // GUID_DEVINTERFACE_HID exists but this is considered best practice
            PInvoke.HidD_GetHidGuid(out Guid guid);

            return guid;
        }
    }
}