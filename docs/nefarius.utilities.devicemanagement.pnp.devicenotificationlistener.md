# DeviceNotificationListener

Namespace: Nefarius.Utilities.DeviceManagement.PnP

Utility class to listen for system-wide device arrivals and removals based on a provided device interface GUID.

```csharp
public class DeviceNotificationListener
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [DeviceNotificationListener](./nefarius.utilities.devicemanagement.pnp.devicenotificationlistener.md)

## Constructors

### **DeviceNotificationListener()**



```csharp
public DeviceNotificationListener()
```

## Methods

### **StartListen(Guid)**

Start listening for device arrivals/removals using the provided [Guid](https://docs.microsoft.com/en-us/dotnet/api/system.guid). Call this after you've
 subscribed to Nefarius.Utilities.DeviceManagement.PnP.DeviceNotificationListener.DeviceArrived and Nefarius.Utilities.DeviceManagement.PnP.DeviceNotificationListener.DeviceRemoved events.

```csharp
public void StartListen(Guid interfaceGuid)
```

#### Parameters

`interfaceGuid` [Guid](https://docs.microsoft.com/en-us/dotnet/api/system.guid)<br>
The device interface GUID to listen for.

### **StopListen()**

Stop listening. The events Nefarius.Utilities.DeviceManagement.PnP.DeviceNotificationListener.DeviceArrived and Nefarius.Utilities.DeviceManagement.PnP.DeviceNotificationListener.DeviceRemoved will not get invoked
 anymore after this call.

```csharp
public void StopListen()
```

## Events

### **DeviceArrived**



```csharp
public event Action<string> DeviceArrived;
```

### **DeviceRemoved**



```csharp
public event Action<string> DeviceRemoved;
```
