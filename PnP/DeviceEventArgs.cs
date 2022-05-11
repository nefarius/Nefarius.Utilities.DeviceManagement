using System;
using System.Collections.Generic;
using System.Text;

namespace Nefarius.Utilities.DeviceManagement.PnP
{
    public class DeviceEventArgs
    {
        public Guid InterfaceGuid { get; set; }
        public string SymLink { get; set; }
    }
}
