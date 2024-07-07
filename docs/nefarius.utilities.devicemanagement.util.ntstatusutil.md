# NtStatusUtil

Namespace: Nefarius.Utilities.DeviceManagement.Util

Utility methods for handling NTSTATUS values.

```csharp
public static class NtStatusUtil
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [NtStatusUtil](./nefarius.utilities.devicemanagement.util.ntstatusutil.md)

## Methods

### <a id="methods-convertntstatustowin32error"/>**ConvertNtStatusToWin32Error(UInt32)**

Converts an NTSTATUS value to a [WIN32_ERROR](./windows.win32.foundation.win32_error.md).

```csharp
public static int ConvertNtStatusToWin32Error(uint ntStatus)
```

#### Parameters

`ntStatus` [UInt32](https://docs.microsoft.com/en-us/dotnet/api/system.uint32)<br>
The NTSTATUS value to convert.

#### Returns

The converted Win32 error code.

**Remarks:**

Source: https://stackoverflow.com/a/32205631
