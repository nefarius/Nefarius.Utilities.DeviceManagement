# Win32Exception

Namespace: Nefarius.Utilities.DeviceManagement.Exceptions

A Win32 API has failed.

```csharp
public class Win32Exception : System.Exception, System.Runtime.Serialization.ISerializable
```

Inheritance [Object](https://learn.microsoft.com/dotnet/api/system.object) → [Exception](https://learn.microsoft.com/dotnet/api/system.exception) → [Win32Exception](./nefarius.utilities.devicemanagement.exceptions.win32exception.md)<br>
Implements [ISerializable](https://learn.microsoft.com/dotnet/api/system.runtime.serialization.iserializable)<br>
Attributes [NullableContextAttribute](./system.runtime.compilerservices.nullablecontextattribute.md), [NullableAttribute](./system.runtime.compilerservices.nullableattribute.md)

## Properties

### <a id="properties-data"/>**Data**

```csharp
public IDictionary Data { get; }
```

#### Property Value

[IDictionary](https://learn.microsoft.com/dotnet/api/system.collections.idictionary)<br>

### <a id="properties-errorcode"/>**ErrorCode**

The native Windows error code.

```csharp
public Nullable<Int32> ErrorCode { get; }
```

#### Property Value

[Nullable](https://learn.microsoft.com/dotnet/api/system.nullable-1)<[Int32](https://learn.microsoft.com/dotnet/api/system.int32)><br>

### <a id="properties-errormessage"/>**ErrorMessage**

The Win32 error message.

```csharp
public string ErrorMessage { get; }
```

#### Property Value

[String](https://learn.microsoft.com/dotnet/api/system.string)<br>

### <a id="properties-helplink"/>**HelpLink**

```csharp
public string HelpLink { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/dotnet/api/system.string)<br>

### <a id="properties-hresult"/>**HResult**

```csharp
public int HResult { get; set; }
```

#### Property Value

[Int32](https://learn.microsoft.com/dotnet/api/system.int32)<br>

### <a id="properties-innerexception"/>**InnerException**

```csharp
public Exception InnerException { get; }
```

#### Property Value

[Exception](https://learn.microsoft.com/dotnet/api/system.exception)<br>

### <a id="properties-message"/>**Message**

```csharp
public string Message { get; }
```

#### Property Value

[String](https://learn.microsoft.com/dotnet/api/system.string)<br>

### <a id="properties-source"/>**Source**

```csharp
public string Source { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/dotnet/api/system.string)<br>

### <a id="properties-stacktrace"/>**StackTrace**

```csharp
public string StackTrace { get; }
```

#### Property Value

[String](https://learn.microsoft.com/dotnet/api/system.string)<br>

### <a id="properties-targetsite"/>**TargetSite**

```csharp
public MethodBase TargetSite { get; }
```

#### Property Value

[MethodBase](https://learn.microsoft.com/dotnet/api/system.reflection.methodbase)<br>

## Methods

### <a id="methods-getmessagefor"/>**GetMessageFor(Nullable&lt;Int32&gt;)**

Translates a Win32 error code to the user-readable message.

```csharp
public static string GetMessageFor(Nullable<Int32> errorCode)
```

#### Parameters

`errorCode` [Nullable](https://learn.microsoft.com/dotnet/api/system.nullable-1)<[Int32](https://learn.microsoft.com/dotnet/api/system.int32)><br>
The Win32 error code. Gets fetched from [Marshal.GetLastWin32Error()](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.marshal.getlastwin32error) if null.

#### Returns

The message, if any, or null.
