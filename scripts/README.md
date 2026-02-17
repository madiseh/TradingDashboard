# Scripts

برای kill کردن process ها می‌تونی از دستور زیر استفاده کنی:

```powershell
@("TradingDashboard.Presentation.Api.Cot","TradingDashboard.Presentation.Api.Login","TradingDashboard.Presentation.Blazor") | ForEach-Object { Get-Process -Name $_ -ErrorAction SilentlyContinue | Stop-Process -Force }
```

یا فایل زیر را اجرا کن:

```powershell
.\scripts\kill-processes.ps1
```
