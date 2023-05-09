# IDeviceNotificationListener

Namespace: Nefarius.Utilities.DeviceManagement.PnP

Utility class to listen for system-wide device arrivals and removals based on a provided device interface GUID.

```csharp
public interface IDeviceNotificationListener
```

**Remarks:**

Original source: https://gist.github.com/emoacht/73eff195317e387f4cda

## Methods

### **StartListen(Guid)**

Start listening for device arrivals/removals using the provided [Guid](https://docs.microsoft.com/en-us/dotnet/api/system.guid). Call this after you've
 subscribed to [IDeviceNotificationListener.DeviceArrived](./nefarius.utilities.devicemanagement.pnp.idevicenotificationlistener.md#devicearrived) and [IDeviceNotificationListener.DeviceRemoved](./nefarius.utilities.devicemanagement.pnp.idevicenotificationlistener.md#deviceremoved) events.

```csharp
void StartListen(Guid interfaceGuid)
```

#### Parameters

`interfaceGuid` [Guid](https://docs.microsoft.com/en-us/dotnet/api/system.guid)<br>
The device interface GUID to listen for.

### **StopListen(Nullable&lt;Guid&gt;)**

Stop listening. The events [IDeviceNotificationListener.DeviceArrived](./nefarius.utilities.devicemanagement.pnp.idevicenotificationlistener.md#devicearrived) and [IDeviceNotificationListener.DeviceRemoved](./nefarius.utilities.devicemanagement.pnp.idevicenotificationlistener.md#deviceremoved) will not get invoked
 anymore after this call. If no [Guid](https://docs.microsoft.com/en-us/dotnet/api/system.guid) is specified, all currently registered interfaces will get
 unsubscribed.

```csharp
void StopListen(Nullable<Guid> interfaceGuid)
```

#### Parameters

`interfaceGuid` [Nullable&lt;Guid&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.nullable-1)<br>

### **RegisterDeviceArrived(Action&lt;DeviceEventArgs&gt;, Nullable&lt;Guid&gt;)**

Subscribe a custom event handler to device arrival events.

```csharp
void RegisterDeviceArrived(Action<DeviceEventArgs> handler, Nullable<Guid> interfaceGuid)
```

#### Parameters

`handler` [Action&lt;DeviceEventArgs&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.action-1)<br>
The event handler to invoke.

`interfaceGuid` [Nullable&lt;Guid&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.nullable-1)<br>
The interface GUID to get notified for or null to get notified for all listening GUIDs.

### **UnregisterDeviceArrived(Action&lt;DeviceEventArgs&gt;)**

Unsubscribe a previously registered event handler.

```csharp
void UnregisterDeviceArrived(Action<DeviceEventArgs> handler)
```

#### Parameters

`handler` [Action&lt;DeviceEventArgs&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.action-1)<br>
The event handler to unsubscribe.

### **RegisterDeviceRemoved(Action&lt;DeviceEventArgs&gt;, Nullable&lt;Guid&gt;)**

Subscribe a custom event handler to device removal events.

```csharp
void RegisterDeviceRemoved(Action<DeviceEventArgs> handler, Nullable<Guid> interfaceGuid)
```

#### Parameters

`handler` [Action&lt;DeviceEventArgs&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.action-1)<br>
The event handler to invoke.

`interfaceGuid` [Nullable&lt;Guid&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.nullable-1)<br>
The interface GUID to get notified for or null to get notified for all listening GUIDs.

### **UnregisterDeviceRemoved(Action&lt;DeviceEventArgs&gt;)**

Unsubscribe a previously registered event handler.

```csharp
void UnregisterDeviceRemoved(Action<DeviceEventArgs> handler)
```

#### Parameters

`handler` [Action&lt;DeviceEventArgs&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.action-1)<br>
The event handler to unsubscribe.

## Events

### **DeviceArrived**

Gets invoked when a new device has arrived (plugged in).

```csharp
public abstract event Action<DeviceEventArgs> DeviceArrived;
```

### **DeviceRemoved**

Gets invoked when an existing device has been removed (unplugged).

```csharp
public abstract event Action<DeviceEventArgs> DeviceRemoved;
```
