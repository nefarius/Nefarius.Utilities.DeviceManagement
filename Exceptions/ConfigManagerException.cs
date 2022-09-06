using System;
using Nefarius.Utilities.DeviceManagement.PnP;

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

        internal ConfigManagerException(string message, SetupApiWrapper.ConfigManagerResult result) : this(message)
        {
        }
    }
}