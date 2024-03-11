# DeviceNotificationListener

Namespace: Nefarius.Utilities.DeviceManagement.PnP

Utility class to listen for system-wide device arrivals and removals based on a provided device interface GUID.

```csharp
public sealed class DeviceNotificationListener : IDeviceNotificationListener, System.IDisposable
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [DeviceNotificationListener](./nefarius.utilities.devicemanagement.pnp.devicenotificationlistener.md)<br>
Implements [IDeviceNotificationListener](./nefarius.utilities.devicemanagement.pnp.idevicenotificationlistener.md), [IDisposable](https://docs.microsoft.com/en-us/dotnet/api/system.idisposable)

**Remarks:**

Original source: https://gist.github.com/emoacht/73eff195317e387f4cda

## Constructors

### <a id="constructors-.ctor"/>**DeviceNotificationListener()**

```csharp
public DeviceNotificationListener()
```

## Methods

### <a id="methods-dispose"/>**Dispose()**

```csharp
public void Dispose()
```

### <a id="methods-registerdevicearrived"/>**RegisterDeviceArrived(Action&lt;DeviceEventArgs&gt;, Nullable&lt;Guid&gt;)**

Subscribe a custom event handler to device arrival events.

```csharp
public void RegisterDeviceArrived(Action<DeviceEventArgs> handler, Nullable<Guid> interfaceGuid)
```

#### Parameters

`handler` [Action&lt;DeviceEventArgs&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.action-1)<br>
The event handler to invoke.

`interfaceGuid` [Nullable&lt;Guid&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.nullable-1)<br>
The interface GUID to get notified for or null to get notified for all listening GUIDs.

### <a id="methods-registerdeviceremoved"/>**RegisterDeviceRemoved(Action&lt;DeviceEventArgs&gt;, Nullable&lt;Guid&gt;)**

Subscribe a custom event handler to device removal events.

```csharp
public void RegisterDeviceRemoved(Action<DeviceEventArgs> handler, Nullable<Guid> interfaceGuid)
```

#### Parameters

`handler` [Action&lt;DeviceEventArgs&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.action-1)<br>
The event handler to invoke.

`interfaceGuid` [Nullable&lt;Guid&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.nullable-1)<br>
The interface GUID to get notified for or null to get notified for all listening GUIDs.

### <a id="methods-startlisten"/>**StartListen(Guid)**

Start listening for device arrivals/removals using the provided [Guid](https://docs.microsoft.com/en-us/dotnet/api/system.guid). Call this after you've
 subscribed to [DeviceNotificationListener.DeviceArrived](./nefarius.utilities.devicemanagement.pnp.devicenotificationlistener.md#devicearrived) and [DeviceNotificationListener.DeviceRemoved](./nefarius.utilities.devicemanagement.pnp.devicenotificationlistener.md#deviceremoved) events.

```csharp
public void StartListen(Guid interfaceGuid)
```

#### Parameters

`interfaceGuid` [Guid](https://docs.microsoft.com/en-us/dotnet/api/system.guid)<br>
The device interface GUID to listen for.

### <a id="methods-stoplisten"/>**StopListen(Nullable&lt;Guid&gt;)**

Stop listening. The events [DeviceNotificationListener.DeviceArrived](./nefarius.utilities.devicemanagement.pnp.devicenotificationlistener.md#devicearrived) and [DeviceNotificationListener.DeviceRemoved](./nefarius.utilities.devicemanagement.pnp.devicenotificationlistener.md#deviceremoved) will not get invoked
 anymore after this call. If no [Guid](https://docs.microsoft.com/en-us/dotnet/api/system.guid) is specified, all currently registered interfaces will get
 unsubscribed.

```csharp
public void StopListen(Nullable<Guid> interfaceGuid)
```

#### Parameters

`interfaceGuid` [Nullable&lt;Guid&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.nullable-1)<br>

### <a id="methods-unregisterdevicearrived"/>**UnregisterDeviceArrived(Action&lt;DeviceEventArgs&gt;)**

Unsubscribe a previously registered event handler.

```csharp
public void UnregisterDeviceArrived(Action<DeviceEventArgs> handler)
```

#### Parameters

`handler` [Action&lt;DeviceEventArgs&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.action-1)<br>
The event handler to unsubscribe.

### <a id="methods-unregisterdeviceremoved"/>**UnregisterDeviceRemoved(Action&lt;DeviceEventArgs&gt;)**

Unsubscribe a previously registered event handler.

```csharp
public void UnregisterDeviceRemoved(Action<DeviceEventArgs> handler)
```

#### Parameters

`handler` [Action&lt;DeviceEventArgs&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.action-1)<br>
The event handler to unsubscribe.

## Events

### <a id="events-devicearrived"/>**DeviceArrived**

Gets invoked when a new device has arrived (plugged in).

```csharp
public event Action<DeviceEventArgs> DeviceArrived;
```

### <a id="events-deviceremoved"/>**DeviceRemoved**

Gets invoked when an existing device has been removed (unplugged).

```csharp
public event Action<DeviceEventArgs> DeviceRemoved;
```
