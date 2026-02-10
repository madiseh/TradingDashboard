# Azure Deployment Guide

## ✅ Prerequisites
- ✅ Azure CLI installed
- ✅ Active Azure Subscription
- ✅ .NET 9 SDK installed
- ✅ Git installed

---

## 🌐 Production URLs

- **API Production:** https://td-api-cot-ejenhygxhtewasb2.westeurope-01.azurewebsites.net
- **Blazor UI:** https://td-blazor-ui.azurewebsites.net

---

## Project Ports (Local Development)

### TradingDashboard.Presentation.Api.Cot (API)
- **HTTP:** http://localhost:5002
- **HTTPS:** https://localhost:7116
- **Profile:** `https` (default)

### TradingDashboard.Presentation.Blazor (UI)
- **HTTP:** http://localhost:5078
- **HTTPS:** https://localhost:7232
- **Profile:** `https` (default)

---

## Steps to Deploy API to Azure

### Prerequisites
- Azure CLI installed
- Active Azure Subscription
- .NET 9 SDK installed

### Step 1: Login to Azure

```powershell
az login
```

### Step 2: Create Resource Group

```powershell
az group create --name TradingDashboard-RG --location westeurope
```

### Step 3: Create App Service Plan (Free Tier)

```powershell
az appservice plan create `
  --name TradingDashboard-Plan `
  --resource-group TradingDashboard-RG `
  --sku F1 `
  --is-linux
```

### Step 4: Create Web App for API

```powershell
az webapp create `
  --name td-api-cot `
  --resource-group TradingDashboard-RG `
  --plan TradingDashboard-Plan `
  --runtime "DOTNET:9.0"
```

### Step 5: Publish API Project

```powershell
# Navigate to API folder
cd src\Presentation\TradingDashboard.Presentation.Api.Cot

# Publish the project
dotnet publish -c Release -o ./publish

# Create zip file
Compress-Archive -Path .\publish\* -DestinationPath .\app.zip -Force
```

**Note:** `publish/` and `app.zip` files are in `.gitignore` and won't be committed.

### Step 6: Deploy to Azure

```powershell
az webapp deploy `
  --resource-group TradingDashboard-RG `
  --name td-api-cot `
  --src-path .\app.zip `
  --type zip
```

### Step 7: Configure CORS (Optional)

```powershell
az webapp cors add `
  --resource-group TradingDashboard-RG `
  --name td-api-cot `
  --allowed-origins "*"
```

### Step 8: View Logs

```powershell
az webapp log tail `
  --resource-group TradingDashboard-RG `
  --name td-api-cot
```

### Step 9: Test API

```powershell
# Test Health Endpoint
curl https://td-api-cot-ejenhygxhtewasb2.westeurope-01.azurewebsites.net/health

# Or with PowerShell
Invoke-WebRequest -Uri https://td-api-cot-ejenhygxhtewasb2.westeurope-01.azurewebsites.net/health
```

---

## API Endpoints

After deployment, your API is available at:

**Base URL:** `https://td-api-cot-ejenhygxhtewasb2.westeurope-01.azurewebsites.net`

### Available Endpoints:

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/health` | Health check |
| `GET` | `/swagger` | Swagger UI (Development only) |
| `GET` | `/api/v1/common/market-types` | Get market types |
| `GET` | `/api/v1/common/markets?type={type}&q={search}` | Search markets |
| `GET` | `/api/v1/common/markets-directory` | Get complete markets directory |
| `GET` | `/api/v1/cot/instruments/{instrumentSlug}` | Get COT instrument details |

### Testing Endpoints:

```powershell
# Health Check
Invoke-WebRequest -Uri https://td-api-cot-ejenhygxhtewasb2.westeurope-01.azurewebsites.net/health

# Get Market Types
Invoke-WebRequest -Uri https://td-api-cot-ejenhygxhtewasb2.westeurope-01.azurewebsites.net/api/v1/common/market-types

# Get Markets
Invoke-WebRequest -Uri "https://td-api-cot-ejenhygxhtewasb2.westeurope-01.azurewebsites.net/api/v1/common/markets?type=futures"

# Get COT Instrument
Invoke-WebRequest -Uri https://td-api-cot-ejenhygxhtewasb2.westeurope-01.azurewebsites.net/api/v1/cot/instruments/gold
```

---

## Running Locally

### Run API

```powershell
# From project root
dotnet run --project src\Presentation\TradingDashboard.Presentation.Api.Cot\TradingDashboard.Presentation.Api.Cot.csproj
```

Or:

```powershell
cd src\Presentation\TradingDashboard.Presentation.Api.Cot
dotnet run
```

API will be available at:
- HTTP: http://localhost:5002
- HTTPS: https://localhost:7116

### Run Blazor UI

```powershell
# From project root
dotnet run --project src\Presentation\TradingDashboard.Presentation.Blazor\TradingDashboard.Presentation.Blazor.csproj
```

UI will be available at:
- HTTP: http://localhost:5078
- HTTPS: https://localhost:7232

---

## Quick Deploy Script

For quick deployment, use this script:

```powershell
# deploy-api.ps1

# Variables
$ResourceGroup = "TradingDashboard-RG"
$AppName = "td-api-cot"
$Location = "westeurope"

# Navigate to API folder
Set-Location "src\Presentation\TradingDashboard.Presentation.Api.Cot"

# Publish
Write-Host "Publishing API..." -ForegroundColor Green
dotnet publish -c Release -o ./publish

# Create zip
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
Write-Host "API URL: https://$AppName-ejenhygxhtewasb2.westeurope-01.azurewebsites.net" -ForegroundColor Cyan

# Clean up temporary files
Remove-Item .\app.zip
Remove-Item .\publish -Recurse -Force
```

Usage:

```powershell
.\deploy-api.ps1
```

---

## Troubleshooting

### Error: "No subscriptions found"
- Make sure you have created an Azure Subscription
- Go to Azure Portal at https://portal.azure.com and activate the subscription

### Error: "TypeLoadException: Could not load type 'Microsoft.OpenApi.Models.OpenApiOperation'"
- This error occurs due to conflict between `Microsoft.AspNetCore.OpenApi` and `Swashbuckle`
- Solution: Don't use `.WithOpenApi()`, only use Swagger

### Error: "Body was inferred"
- In Minimal API endpoints, dependency injection must be specified with `[FromServices]`
- Example: `([FromServices] ICotInstrumentRepository repository)`

### API not working in Azure
- Check logs: `az webapp log tail --name td-api-cot --resource-group TradingDashboard-RG`
- Make sure Runtime in Azure is set to .NET 9
- Test the `/health` endpoint

### Wrong port in Local
- Ports are configured in `launchSettings.json`
- API: 5002 (HTTP), 7116 (HTTPS)
- Blazor: 5078 (HTTP), 7232 (HTTPS)

### Publish files visible in Git
- `publish/` and `*.zip` are added to `.gitignore`
- These files won't be committed

---

## Redeployment After Changes

To redeploy after making changes:

```powershell
cd src\Presentation\TradingDashboard.Presentation.Api.Cot
dotnet publish -c Release -o ./publish
Compress-Archive -Path .\publish\* -DestinationPath .\app.zip -Force
az webapp deploy --resource-group TradingDashboard-RG --name td-api-cot --src-path .\app.zip --type zip
```

---

## Useful Links

- **Azure Portal:** https://portal.azure.com
- **API Production URL:** https://td-api-cot-ejenhygxhtewasb2.westeurope-01.azurewebsites.net
- **Azure CLI Docs:** https://docs.microsoft.com/cli/azure/
- **ASP.NET Core Deployment:** https://docs.microsoft.com/aspnet/core/host-and-deploy/azure-apps/
