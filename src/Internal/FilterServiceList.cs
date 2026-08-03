using System;
using System.Collections.Generic;
using System.Linq;

namespace Nefarius.Utilities.DeviceManagement.Internal;

/// <summary>
///     Pure MultiSz filter-service list mutation used by <see cref="PnP.DeviceClassFilters" />.
/// </summary>
internal static class FilterServiceList
{
    /// <summary>
    ///     Adds a service to the filter list without introducing empty entries or duplicates
    ///     (same <see cref="Enumerable.Distinct{TSource}(IEnumerable{TSource})" /> semantics as production).
    /// </summary>
    public static IReadOnlyList<string> Add(IEnumerable<string>? existing, string service)
    {
        IEnumerable<string> source = existing ?? Enumerable.Empty<string>();

        return source
            .Concat(new[] { service })
            .Where(e => !string.IsNullOrWhiteSpace(e))
            .Distinct()
            .ToList();
    }

    /// <summary>
    ///     Removes a service (case-insensitive) and normalizes the remaining list.
    /// </summary>
    public static IReadOnlyList<string> Remove(IEnumerable<string>? existing, string service)
    {
        List<string> elements = (existing ?? Enumerable.Empty<string>()).ToList();
        elements.RemoveAll(e => e.Equals(service, StringComparison.OrdinalIgnoreCase));

        return elements
            .Where(e => !string.IsNullOrWhiteSpace(e))
            .Distinct()
            .ToList();
    }
}
