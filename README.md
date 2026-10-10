# Window placement

All executable projects target .NET 10 for Windows. `SharedUtilties` remains a `netstandard2.0` library.

## Windows service installation

Publish the service, then run the following commands from an elevated terminal. Replace the example path with the absolute path to the published executable.

```powershell
sc.exe create ZackDisplayService2 binPath= "C:\Apps\WindowDisplayService\WindowDisplayService.exe" start= auto depend= TermService DisplayName= "Zack Display Service"
sc.exe start ZackDisplayService2
```

To remove it:

```powershell
sc.exe stop ZackDisplayService2
sc.exe delete ZackDisplayService2
```

Running `WindowDisplayService.exe` directly keeps the previous interactive console behavior for local diagnostics.
