using System;

namespace Nefarius.Utilities.DeviceManagement.Internal;

/// <summary>
///     Pure evaluation of whether a device ancestry indicates a root-enumerated virtual device.
/// </summary>
internal static class VirtualDeviceEvaluator
{
    internal const string TreeRootParentId = @"HTREE\ROOT\0";

    /// <summary>
    ///     Returns true if the top-most device below the PnP tree root looks root-enumerated virtual.
    /// </summary>
    public static bool IsRootEnumeratedVirtual(string topMostInstanceIdBelowRoot)
    {
        if (string.IsNullOrEmpty(topMostInstanceIdBelowRoot))
        {
            return false;
        }

        return topMostInstanceIdBelowRoot.StartsWith(@"ROOT\SYSTEM", StringComparison.OrdinalIgnoreCase)
               || topMostInstanceIdBelowRoot.StartsWith(@"ROOT\USB", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    ///     Walks from <paramref name="startInstanceId" /> toward the tree root using <paramref name="getParentInstanceId" />.
    /// </summary>
    /// <param name="startInstanceId">Leaf device instance ID.</param>
    /// <param name="getParentInstanceId">Resolves a device's parent instance ID.</param>
    /// <param name="excludeIfMatches">If true for any visited instance, the result is false (not virtual).</param>
    /// <returns>True if the ancestry indicates a virtual/root-enumerated device.</returns>
    public static bool WalkAndEvaluate(
        string startInstanceId,
        Func<string, string?> getParentInstanceId,
        Func<string, bool>? excludeIfMatches = null)
    {
        if (string.IsNullOrEmpty(startInstanceId))
        {
            return false;
        }

        string current = startInstanceId;

        while (true)
        {
            if (excludeIfMatches != null && excludeIfMatches(current))
            {
                return false;
            }

            string? parentId = getParentInstanceId(current);

            if (string.IsNullOrEmpty(parentId))
            {
                // Preserve historical behavior: retry getParent until a value appears.
                continue;
            }

            if (parentId!.Equals(TreeRootParentId, StringComparison.OrdinalIgnoreCase))
            {
                break;
            }

            current = parentId;
        }

        return IsRootEnumeratedVirtual(current);
    }
}
