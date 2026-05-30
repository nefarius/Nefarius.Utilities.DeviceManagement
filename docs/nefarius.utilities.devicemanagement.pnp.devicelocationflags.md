# DeviceLocationFlags

Namespace: Nefarius.Utilities.DeviceManagement.PnP

A variable of ULONG type that supplies one of the following flag values that apply if the caller supplies a device
 instance identifier:
 CM_LOCATE_DEVNODE_NORMAL
 The function retrieves the device instance handle for the specified device only if the device is currently
 configured in the device tree.
 CM_LOCATE_DEVNODE_PHANTOM
 The function retrieves a device instance handle for the specified device if the device is currently configured in
 the device tree or the device is a nonpresent device that is not currently configured in the device tree.
 CM_LOCATE_DEVNODE_CANCELREMOVE
 The function retrieves a device instance handle for the specified device if the device is currently configured in
 the device tree or in the process of being removed from the device tree. If the device is in the process of being
 removed, the function cancels the removal of the device.

```csharp
public enum DeviceLocationFlags
```

Inheritance [Object](https://learn.microsoft.com/dotnet/api/system.object) → [ValueType](https://learn.microsoft.com/dotnet/api/system.valuetype) → [Enum](https://learn.microsoft.com/dotnet/api/system.enum) → [DeviceLocationFlags](./nefarius.utilities.devicemanagement.pnp.devicelocationflags.md)<br>
Implements [IComparable](https://learn.microsoft.com/dotnet/api/system.icomparable), [ISpanFormattable](https://learn.microsoft.com/dotnet/api/system.ispanformattable), [IFormattable](https://learn.microsoft.com/dotnet/api/system.iformattable), [IConvertible](https://learn.microsoft.com/dotnet/api/system.iconvertible)

## Fields

| Name | Value | Description |
| --- | --: | --- |
| Normal | 0 | The function retrieves the device instance handle for the specified device only if the device is currently configured in the device tree. |
| Phantom | 1 | The function retrieves a device instance handle for the specified device if the device is currently configured in the device tree or the device is a nonpresent device that is not currently configured in the device tree. |
| CancelRemove | 2 | The function retrieves a device instance handle for the specified device if the device is currently configured in the device tree or in the process of being removed from the device tree. If the device is in the process of being removed, the function cancels the removal of the device. |
