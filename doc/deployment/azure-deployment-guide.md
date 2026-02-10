# راهنمای Deploy در Azure

## پورت‌های پروژه‌ها (Local Development)

### TradingDashboard.Presentation.Api.Cot (API)
- **HTTP:** http://localhost:5002
- **HTTPS:** https://localhost:7116
- **Profile:** `https` (default)

### TradingDashboard.Presentation.Blazor (UI)
- **HTTP:** http://localhost:5078
- **HTTPS:** https://localhost:7232
- **Profile:** `https` (default)

---

## مراحل Deploy کردن API به Azure

### پیش‌نیازها
- Azure CLI نصب شده
- Azure Subscription فعال
- .NET 9 SDK نصب شده

### مرحله 1: لاگین به Azure

```powershell
az login
```

### مرحله 2: ایجاد Resource Group

```powershell
az group create --name TradingDashboard-RG --location westeurope
```

### مرحله 3: ایجاد App Service Plan (رایگان)

```powershell
az appservice plan create `
  --name TradingDashboard-Plan `
  --resource-group TradingDashboard-RG `
  --sku F1 `
  --is-linux
```

### مرحله 4: ایجاد Web App برای API

```powershell
az webapp create `
  --name td-api-cot `
  --resource-group TradingDashboard-RG `
  --plan TradingDashboard-Plan `
  --runtime "DOTNET:9.0"
```

### مرحله 5: Publish کردن پروژه API

```powershell
# رفتن به پوشه API
cd src\Presentation\TradingDashboard.Presentation.Api.Cot

# Publish کردن پروژه
dotnet publish -c Release -o ./publish

# Zip کردن فایل‌ها
Compress-Archive -Path .\publish\* -DestinationPath .\app.zip -Force
```

### مرحله 6: Deploy کردن به Azure

```powershell
az webapp deploy `
  --resource-group TradingDashboard-RG `
  --name td-api-cot `
  --src-path .\app.zip `
  --type zip
```

### مرحله 7: تنظیم CORS (اختیاری)

```powershell
az webapp cors add `
  --resource-group TradingDashboard-RG `
  --name td-api-cot `
  --allowed-origins "*"
```

### مرحله 8: مشاهده لاگ‌ها

```powershell
az webapp log tail `
  --resource-group TradingDashboard-RG `
  --name td-api-cot
```

### مرحله 9: تست API

```powershell
# تست Health Endpoint
curl https://td-api-cot.azurewebsites.net/health

# یا با PowerShell
Invoke-WebRequest -Uri https://td-api-cot.azurewebsites.net/health
```

---

## Endpoints API

بعد از deploy، API شما در این آدرس در دسترس است:

**Base URL:** `https://td-api-cot.azurewebsites.net`

### Available Endpoints:

- `GET /health` - Health check
- `GET /api/v1/common/market-types` - دریافت انواع بازار
- `GET /api/v1/common/markets?type={type}&q={search}` - جستجوی بازارها
- `GET /api/v1/common/markets-directory` - دایرکتوری کامل بازارها
- `GET /api/v1/cot/instruments/{instrumentSlug}` - جزئیات ابزار COT

---

## اجرای لوکال

### اجرای API

```powershell
# از ریشه پروژه
dotnet run --project src\Presentation\TradingDashboard.Presentation.Api.Cot\TradingDashboard.Presentation.Api.Cot.csproj
```

یا:

```powershell
cd src\Presentation\TradingDashboard.Presentation.Api.Cot
dotnet run
```

API در آدرس زیر در دسترس خواهد بود:
- HTTP: http://localhost:5002
- HTTPS: https://localhost:7116

### اجرای Blazor UI

```powershell
# از ریشه پروژه
dotnet run --project src\Presentation\TradingDashboard.Presentation.Blazor\TradingDashboard.Presentation.Blazor.csproj
```

UI در آدرس زیر در دسترس خواهد بود:
- HTTP: http://localhost:5078
- HTTPS: https://localhost:7232

---

## یک دستور Deploy سریع

برای deploy سریع همه مراحل را در یک اسکریپت قرار دهید:

```powershell
# deploy-api.ps1

# متغیرها
$ResourceGroup = "TradingDashboard-RG"
$AppName = "td-api-cot"
$Location = "westeurope"

# رفتن به پوشه API
Set-Location "src\Presentation\TradingDashboard.Presentation.Api.Cot"

# Publish
Write-Host "Publishing API..." -ForegroundColor Green
dotnet publish -c Release -o ./publish

# Zip
Write-Host "Creating zip file..." -ForegroundColor Green
Compress-Archive -Path .\publish\* -DestinationPath .\app.zip -Force

# Deploy
Write-Host "Deploying to Azure..." -ForegroundColor Green
az webapp deploy `
  --resource-group $ResourceGroup `
  --name $AppName `
  --src-path .\app.zip `
  --type zip

Write-Host "Deployment completed!" -ForegroundColor Green
Write-Host "API URL: https://$AppName.azurewebsites.net" -ForegroundColor Cyan

# پاک کردن فایل‌های موقت
Remove-Item .\app.zip
Remove-Item .\publish -Recurse -Force
```

استفاده:

```powershell
.\deploy-api.ps1
```

---

## عیب‌یابی

### خطای "No subscriptions found"
- مطمئن شوید Azure Subscription ساخته‌اید
- از Azure Portal به https://portal.azure.com بروید و subscription را فعال کنید

### خطای "Body was inferred"
- در endpoint‌های Minimal API، dependency injection باید با `[FromServices]` مشخص شود
- یا service را در constructor ثبت کنید

### API در Azure کار نمی‌کند
- لاگ‌ها را چک کنید: `az webapp log tail --name td-api-cot --resource-group TradingDashboard-RG`
- مطمئن شوید Runtime در Azure روی .NET 9 است

### پورت اشتباه در Local
- پورت‌ها در `launchSettings.json` تنظیم می‌شوند
- API: 5002 (HTTP), 7116 (HTTPS)
- Blazor: 5078 (HTTP), 7232 (HTTPS)

---

## به‌روزرسانی‌های بعدی

برای deploy مجدد بعد از تغییرات:

```powershell
cd src\Presentation\TradingDashboard.Presentation.Api.Cot
dotnet publish -c Release -o ./publish
Compress-Archive -Path .\publish\* -DestinationPath .\app.zip -Force
az webapp deploy --resource-group TradingDashboard-RG --name td-api-cot --src-path .\app.zip --type zip
```

---

## لینک‌های مفید

- **Azure Portal:** https://portal.azure.com
- **API Production URL:** https://td-api-cot.azurewebsites.net
- **Azure CLI Docs:** https://docs.microsoft.com/cli/azure/
- **ASP.NET Core Deployment:** https://docs.microsoft.com/aspnet/core/host-and-deploy/azure-apps/
