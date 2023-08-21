#nullable enable
using System.Collections.Generic;
using System.Linq;

namespace Nefarius.Utilities.DeviceManagement.PnP;

public partial class PnPDevice
{
    /// <summary>
    ///     The parent of this <see cref="IPnPDevice" />, if any.
    /// </summary>
    public IPnPDevice? Parent
    {
        get
        {
            string? parentId = GetProperty<string>(DevicePropertyKey.Device_Parent);

            return string.IsNullOrEmpty(parentId) ? null : GetDeviceByInstanceId(parentId);
        }
    }

    /// <summary>
    ///     Siblings of this <see cref="IPnPDevice" /> sharing the same parent, if any.
    /// </summary>
    public IEnumerable<IPnPDevice>? Siblings
    {
        get
        {
            string[]? siblingsIds = GetProperty<string[]>(DevicePropertyKey.Device_Siblings);

            return siblingsIds?.Select(id => GetDeviceByInstanceId(id));
        }
    }

    /// <summary>
    ///     Children of this <see cref="IPnPDevice" />, if any.
    /// </summary>
    public IEnumerable<IPnPDevice>? Children
    {
        get
        {
            string[]? childrenIds = GetProperty<string[]>(DevicePropertyKey.Device_Children);

            return childrenIds?.Select(id => GetDeviceByInstanceId(id));
        }
    }
}