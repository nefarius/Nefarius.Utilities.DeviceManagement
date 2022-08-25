# DeviceNotificationListener

Namespace: Nefarius.Utilities.DeviceManagement.PnP

Utility class to listen for system-wide device arrivals and removals based on a provided device interface GUID.

```csharp
public class DeviceNotificationListener : IDeviceNotificationListener, System.IDisposable
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [DeviceNotificationListener](./nefarius.utilities.devicemanagement.pnp.devicenotificationlistener.md)<br>
Implements [IDeviceNotificationListener](./nefarius.utilities.devicemanagement.pnp.idevicenotificationlistener.md), [IDisposable](https://docs.microsoft.com/en-us/dotnet/api/system.idisposable)

## Constructors

### **DeviceNotificationListener()**



```csharp
public DeviceNotificationListener()
```

## Methods

### **RegisterDeviceArrived(Action&lt;DeviceEventArgs&gt;, Nullable&lt;Guid&gt;)**

Subscribe a custom event handler to device arrival events.

```csharp
public void RegisterDeviceArrived(Action<DeviceEventArgs> handler, Nullable<Guid> interfaceGuid)
```

#### Parameters

`handler` [Action&lt;DeviceEventArgs&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.action-1)<br>
The event handler to invoke.

`interfaceGuid` [Nullable&lt;Guid&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.nullable-1)<br>
The interface GUID to get notified for or null to get notified for all listening GUIDs.

### **UnregisterDeviceArrived(Action&lt;DeviceEventArgs&gt;)**

Unsubscribe a previously registered event handler.

```csharp
public void UnregisterDeviceArrived(Action<DeviceEventArgs> handler)
```

#### Parameters

`handler` [Action&lt;DeviceEventArgs&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.action-1)<br>
The event handler to unsubscribe.

### **RegisterDeviceRemoved(Action&lt;DeviceEventArgs&gt;, Nullable&lt;Guid&gt;)**

Subscribe a custom event handler to device removal events.

```csharp
public void RegisterDeviceRemoved(Action<DeviceEventArgs> handler, Nullable<Guid> interfaceGuid)
```

#### Parameters

`handler` [Action&lt;DeviceEventArgs&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.action-1)<br>
The event handler to invoke.

`interfaceGuid` [Nullable&lt;Guid&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.nullable-1)<br>
The interface GUID to get notified for or null to get notified for all listening GUIDs.

### **UnregisterDeviceRemoved(Action&lt;DeviceEventArgs&gt;)**

Unsubscribe a previously registered event handler.

```csharp
public void UnregisterDeviceRemoved(Action<DeviceEventArgs> handler)
```

#### Parameters

`handler` [Action&lt;DeviceEventArgs&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.action-1)<br>
The event handler to unsubscribe.

### **StartListen(Guid)**

Start listening for device arrivals/removals using the provided [Guid](https://docs.microsoft.com/en-us/dotnet/api/system.guid). Call this after you've
 subscribed to Nefarius.Utilities.DeviceManagement.PnP.DeviceNotificationListener.DeviceArrived and Nefarius.Utilities.DeviceManagement.PnP.DeviceNotificationListener.DeviceRemoved events.

```csharp
public void StartListen(Guid interfaceGuid)
```

#### Parameters

`interfaceGuid` [Guid](https://docs.microsoft.com/en-us/dotnet/api/system.guid)<br>
The device interface GUID to listen for.

### **StopListen(Nullable&lt;Guid&gt;)**

Stop listening. The events Nefarius.Utilities.DeviceManagement.PnP.DeviceNotificationListener.DeviceArrived and Nefarius.Utilities.DeviceManagement.PnP.DeviceNotificationListener.DeviceRemoved will not get invoked
 anymore after this call. If no [Guid](https://docs.microsoft.com/en-us/dotnet/api/system.guid) is specified, all currently registered interfaces will get
 unsubscribed.

```csharp
public void StopListen(Nullable<Guid> interfaceGuid)
```

#### Parameters

`interfaceGuid` [Nullable&lt;Guid&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.nullable-1)<br>

### **Dispose()**



```csharp
public void Dispose()
```

### **Dispose(Boolean)**



```csharp
protected void Dispose(bool disposing)
```

#### Parameters

`disposing` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

## Events

### **DeviceArrived**

Gets invoked when a new device has arrived (plugged in).

```csharp
public event Action<DeviceEventArgs> DeviceArrived;
```

### **DeviceRemoved**

Gets invoked when an existing device has been removed (unplugged).

```csharp
public event Action<DeviceEventArgs> DeviceRemoved;
```
