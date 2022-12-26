using System;

using Nefarius.Utilities.DeviceManagement.PnP;

namespace Nefarius.Utilities.DeviceManagement.Exceptions;

/// <summary>
///     A <see cref="UsbPnPDevice" /> related exception.
/// </summary>
public class UsbPnPDeviceException : Exception
{
    /// <summary>
    ///     A <see cref="UsbPnPDevice" /> operation has failed.
    /// </summary>
    /// <param name="message">The error message.</param>
    internal UsbPnPDeviceException(string message) : base(message)
    {
    }
}

/// <summary>
///     Thrown if a conversion of a <see cref="PnPDevice" /> to a <see cref="UsbPnPDevice" /> wasn't possible.
/// </summary>
public class UsbPnPDeviceConversionException : UsbPnPDeviceException
{
    internal UsbPnPDeviceConversionException(string message) : base(message)
    {
    }
}
