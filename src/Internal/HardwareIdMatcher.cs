using System;
using System.Collections.Generic;
using System.Linq;

namespace Nefarius.Utilities.DeviceManagement.Internal;

/// <summary>
///     Pure hardware-ID matching used by <see cref="PnP.Devcon.FindInDeviceClassByHardwareId(System.Guid,string,out System.Collections.Generic.IEnumerable{string},bool,bool)" />.
/// </summary>
internal static class HardwareIdMatcher
{
    /// <summary>
    ///     Returns true if <paramref name="hardwareIds" /> match <paramref name="needle" />.
    /// </summary>
    /// <param name="hardwareIds">Device hardware IDs.</param>
    /// <param name="needle">ID or substring to search for.</param>
    /// <param name="allowPartial">True to match substrings; false for exact (case-insensitive) match.</param>
    public static bool Matches(IEnumerable<string> hardwareIds, string needle, bool allowPartial)
    {
        if (hardwareIds is null)
        {
            return false;
        }

        if (string.IsNullOrEmpty(needle))
        {
            return false;
        }

        List<string> ids = hardwareIds.Select(id => id.ToUpperInvariant()).ToList();
        string normalizedNeedle = needle.ToUpperInvariant();

        if (allowPartial)
        {
            return ids.Any(id => id.Contains(normalizedNeedle));
        }

        return ids.Any(id => id.Equals(normalizedNeedle, StringComparison.OrdinalIgnoreCase));
    }
}
