# DevicePropertyDevice

Namespace: Nefarius.Utilities.DeviceManagement.PnP

Common device property definitions.

```csharp
public abstract class DevicePropertyDevice : DevicePropertyKey, System.IEquatable`1[[Nefarius.Utilities.DeviceManagement.PnP.DevicePropertyKey, Nefarius.Utilities.DeviceManagement, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]]
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [DevicePropertyKey](./nefarius.utilities.devicemanagement.pnp.devicepropertykey.md) → [DevicePropertyDevice](./nefarius.utilities.devicemanagement.pnp.devicepropertydevice.md)<br>
Implements [IEquatable&lt;DevicePropertyKey&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.iequatable-1)

## Fields

### **DeviceDesc**

The Device Description.

```csharp
public static DevicePropertyKey DeviceDesc;
```

### **HardwareIds**

The list of hardware IDs.

```csharp
public static DevicePropertyKey HardwareIds;
```

### **CompatibleIds**

The list of compatible IDs.

```csharp
public static DevicePropertyKey CompatibleIds;
```

### **Service**

The service name.

```csharp
public static DevicePropertyKey Service;
```

### **Class**

The device class name.

```csharp
public static DevicePropertyKey Class;
```

### **ClassGuid**

The device class guid.

```csharp
public static DevicePropertyKey ClassGuid;
```

### **Driver**

The driver name.

```csharp
public static DevicePropertyKey Driver;
```

### **ConfigFlags**

Possible configuration flags.

```csharp
public static DevicePropertyKey ConfigFlags;
```

### **Manufacturer**

The manufacturer string.

```csharp
public static DevicePropertyKey Manufacturer;
```

### **FriendlyName**

The friendly display name.

```csharp
public static DevicePropertyKey FriendlyName;
```

### **LocationInfo**

The location information.

```csharp
public static DevicePropertyKey LocationInfo;
```

### **PDOName**

The Physical Device Object name.

```csharp
public static DevicePropertyKey PDOName;
```

### **Capabilities**

The device capabilities.

```csharp
public static DevicePropertyKey Capabilities;
```

### **UINumber**

The UI number.

```csharp
public static DevicePropertyKey UINumber;
```

### **UpperFilters**

The upper filters list.

```csharp
public static DevicePropertyKey UpperFilters;
```

### **LowerFilters**

The lower filters list.

```csharp
public static DevicePropertyKey LowerFilters;
```

### **BusTypeGuid**

The bus type GUILD.

```csharp
public static DevicePropertyKey BusTypeGuid;
```

### **LegacyBusType**

The legacy bus type.

```csharp
public static DevicePropertyKey LegacyBusType;
```

### **BusNumber**

The bus number.

```csharp
public static DevicePropertyKey BusNumber;
```

### **EnumeratorName**

The enumerator name.

```csharp
public static DevicePropertyKey EnumeratorName;
```

### **InstanceId**

The instance ID.

```csharp
public static DevicePropertyKey InstanceId;
```

### **Parent**

The parent instance ID.

```csharp
public static DevicePropertyKey Parent;
```

### **Children**

The list of child instances, if any.

```csharp
public static DevicePropertyKey Children;
```

### **Siblings**

The list of siblings, if any.

```csharp
public static DevicePropertyKey Siblings;
```

### **DriverDate**

The driver release date.

```csharp
public static DevicePropertyKey DriverDate;
```

### **DriverVersion**

The driver version.

```csharp
public static DevicePropertyKey DriverVersion;
```

### **DriverDesc**

The driver description.

```csharp
public static DevicePropertyKey DriverDesc;
```

### **DriverInfPath**

The driver INF path.

```csharp
public static DevicePropertyKey DriverInfPath;
```

### **DriverInfSection**

The driver INF section.

```csharp
public static DevicePropertyKey DriverInfSection;
```

### **MatchingDeviceId**

The matching device ID.

```csharp
public static DevicePropertyKey MatchingDeviceId;
```

### **DriverProvider**

The driver provider name.

```csharp
public static DevicePropertyKey DriverProvider;
```

### **Address**

The address (bus index, port number) the enumerator assigned to the device.

```csharp
public static DevicePropertyKey Address;
```

### **InstallDate**

The driver install date.

```csharp
public static DevicePropertyKey InstallDate;
```

### **FirstInstallDate**

The driver first install date.

```csharp
public static DevicePropertyKey FirstInstallDate;
```

### **LastArrivalDate**

The driver last arrival date.

```csharp
public static DevicePropertyKey LastArrivalDate;
```

### **LastRemovalDate**

The driver last removal date

```csharp
public static DevicePropertyKey LastRemovalDate;
```

## Properties

### **CategoryGuid**

The [Guid](https://docs.microsoft.com/en-us/dotnet/api/system.guid) for teh category this property belongs to.

```csharp
public Guid CategoryGuid { get; }
```

#### Property Value

[Guid](https://docs.microsoft.com/en-us/dotnet/api/system.guid)<br>

### **PropertyIdentifier**

The unique identifier withing the category group for this property.

```csharp
public uint PropertyIdentifier { get; }
```

#### Property Value

[UInt32](https://docs.microsoft.com/en-us/dotnet/api/system.uint32)<br>

### **PropertyType**

The managed type of the property (integer, string, array, ...).

```csharp
public Type PropertyType { get; }
```

#### Property Value

[Type](https://docs.microsoft.com/en-us/dotnet/api/system.type)<br>
