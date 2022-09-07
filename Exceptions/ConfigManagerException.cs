using System;
using Windows.Win32.Devices.DeviceAndDriverInstallation;

namespace Nefarius.Utilities.DeviceManagement.Exceptions
{
    /// <summary>
    ///     A Configuration Manager API has failed.
    /// </summary>
    public class ConfigManagerException : Exception
    {
        internal ConfigManagerException(string message) : base(message)
        {
        }

        internal ConfigManagerException(string message, CONFIGRET result) : this(message)
        {
        }
    }
}