using Nefarius.Utilities.DeviceManagement.Internal;

namespace Tests.Unit;

/// <summary>
///     In-memory <see cref="IDeviceManagementNative" /> for hermetic unit tests.
/// </summary>
internal sealed class FakeDeviceManagementNative : IDeviceManagementNative
{
    private readonly Dictionary<Guid, List<string>> _classDevices = new();
    private readonly Dictionary<string, string[]> _hardwareIds = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, string?> _parents = new(StringComparer.OrdinalIgnoreCase);

    public void AddClassDevice(Guid classGuid, string instanceId, string[]? hardwareIds = null,
        string? parentInstanceId = null)
    {
        if (!_classDevices.TryGetValue(classGuid, out List<string>? list))
        {
            list = new List<string>();
            _classDevices[classGuid] = list;
        }

        list.Add(instanceId.ToUpperInvariant());

        if (hardwareIds is not null)
        {
            _hardwareIds[instanceId] = hardwareIds;
        }

        if (parentInstanceId is not null || _parents.ContainsKey(instanceId))
        {
            _parents[instanceId] = parentInstanceId;
        }
    }

    public void SetParent(string instanceId, string? parentInstanceId)
    {
        _parents[instanceId] = parentInstanceId;
    }

    public void SetHardwareIds(string instanceId, string[] hardwareIds)
    {
        _hardwareIds[instanceId] = hardwareIds;
    }

    public IEnumerable<string> EnumerateDeviceInstanceIds(Guid classGuid, bool presentOnly)
    {
        return _classDevices.TryGetValue(classGuid, out List<string>? list)
            ? list.ToList()
            : Enumerable.Empty<string>();
    }

    public string[]? GetHardwareIds(string instanceId, bool presentOnly)
    {
        return _hardwareIds.TryGetValue(instanceId, out string[]? ids) ? ids : null;
    }

    public string? GetParentInstanceId(string instanceId)
    {
        return _parents.TryGetValue(instanceId, out string? parent) ? parent : null;
    }
}
