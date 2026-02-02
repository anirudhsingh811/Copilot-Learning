# ?? Deploy Simple Web App to Azure App Service

This guide shows you how to deploy a simple ASP.NET Core Web API to Azure App Service.

## ?? **What You'll Learn (AZ-204 Topics)**

- ? Create Azure App Service resources
- ? Configure App Service Plans
- ? Deploy using Azure CLI
- ? Configure Application Settings
- ? Use Managed Identity
- ? Access Key Vault from App Service
- ? Monitor with health checks

---

## ?? **Step 1: Test Locally First**

```bash
# Navigate to the web app folder
cd "Azure- 204/SimpleWebApp"

# Restore packages
dotnet restore

# Run locally
dotnet run

# Test in browser
# Open: http://localhost:5000
# Open: http://localhost:5000/swagger
```

**Test these endpoints:**
- `GET /` - Welcome message
- `GET /health` - Health check
- `GET /api/info` - App information
- `GET /api/env` - Environment variables
- `GET /api/echo/hello` - Echo test

---

## ??? **Step 2: Create Azure Resources**

### **Option A: Quick Script (Recommended)**

```powershell
# Variables
$resourceGroup = "rg-az204-compute"
$location = "eastus"
$appServicePlan = "plan-az204-simple"
$webAppName = "webapp-az204-simple-$(Get-Random -Maximum 9999)"
$keyVaultName = "kv-az204-9199"

# Login and set subscription
az login
az account set --subscription "c01b8f09-10c7-4d70-b719-05b1b231f9d3"

# Create Resource Group (if not exists)
az group create --name $resourceGroup --location $location

# Create App Service Plan (Free tier for testing)
az appservice plan create `
  --name $appServicePlan `
  --resource-group $resourceGroup `
  --location $location `
  --sku F1 `
  --is-linux

# Create Web App
az webapp create `
  --name $webAppName `
  --resource-group $resourceGroup `
  --plan $appServicePlan `
  --runtime "DOTNET|9.0"

# Enable Managed Identity
az webapp identity assign `
  --name $webAppName `
  --resource-group $resourceGroup

# Get the Managed Identity Principal ID
$principalId = az webapp identity show `
  --name $webAppName `
  --resource-group $resourceGroup `
  --query principalId -o tsv

# Grant Key Vault access to the Managed Identity
$kvId = az keyvault show --name $keyVaultName --query id -o tsv
az role assignment create `
  --role "Key Vault Secrets User" `
  --assignee $principalId `
  --scope $kvId

# Configure App Settings
az webapp config appsettings set `
  --name $webAppName `
  --resource-group $resourceGroup `
  --settings KeyVault__Uri="https://$keyVaultName.vault.azure.net/"

Write-Host "`n? Azure resources created successfully!" -ForegroundColor Green
Write-Host "Web App Name: $webAppName" -ForegroundColor Cyan
Write-Host "URL: https://$webAppName.azurewebsites.net" -ForegroundColor Cyan
```

### **Option B: Step-by-Step (For Learning)**

```bash
# 1. Create App Service Plan
az appservice plan create \
  --name plan-az204-simple \
  --resource-group rg-az204-compute \
  --location eastus \
  --sku B1 \
  --is-linux

# 2. Create Web App
az webapp create \
  --name webapp-az204-simple-1234 \
  --resource-group rg-az204-compute \
  --plan plan-az204-simple \
  --runtime "DOTNET|9.0"

# 3. Enable Managed Identity
az webapp identity assign \
  --name webapp-az204-simple-1234 \
  --resource-group rg-az204-compute

# 4. Configure App Settings
az webapp config appsettings set \
  --name webapp-az204-simple-1234 \
  --resource-group rg-az204-compute \
  --settings \
    KeyVault__Uri="https://kv-az204-9199.vault.azure.net/" \
    ASPNETCORE_ENVIRONMENT="Production"
```

---

## ?? **Step 3: Deploy the App**

### **Option 1: Deploy with Azure CLI (Recommended)**

```powershell
# Navigate to the web app folder
cd "Azure- 204/SimpleWebApp"

# Publish the app
dotnet publish -c Release -o ./publish

# Create a zip file
Compress-Archive -Path ./publish/* -DestinationPath ./app.zip -Force

# Deploy to Azure
az webapp deployment source config-zip `
  --name $webAppName `
  --resource-group $resourceGroup `
  --src ./app.zip

Write-Host "`n?? Deployment complete!" -ForegroundColor Green
Write-Host "Visit: https://$webAppName.azurewebsites.net" -ForegroundColor Cyan
Write-Host "Swagger: https://$webAppName.azurewebsites.net/swagger" -ForegroundColor Cyan
```

### **Option 2: Deploy with Visual Studio**

1. **Right-click** on `SimpleWebApp` project
2. **Click "Publish"**
3. **Choose "Azure"** ? Next
4. **Choose "Azure App Service (Linux)"** ? Next
5. **Select your subscription** and resource group
6. **Select or create** App Service
7. **Click "Finish"** ? **Click "Publish"**

### **Option 3: Deploy with VS Code**

1. Install **Azure App Service extension**
2. **Right-click** on `SimpleWebApp` folder
3. **Select "Deploy to Web App..."**
4. **Follow the prompts**

---

## ?? **Step 4: Test Your Deployed App**

```powershell
# Get your web app URL
$webAppUrl = az webapp show `
  --name $webAppName `
  --resource-group $resourceGroup `
  --query defaultHostName -o tsv

# Test endpoints
Write-Host "`nTesting deployed app..." -ForegroundColor Yellow

# Test root
curl "https://$webAppUrl/"

# Test health
curl "https://$webAppUrl/health"

# Test info
curl "https://$webAppUrl/api/info"

# Test environment variables (shows App Service info)
curl "https://$webAppUrl/api/env"

# Test Key Vault integration
curl "https://$webAppUrl/api/secrets"

# Open Swagger UI in browser
Start-Process "https://$webAppUrl/swagger"
```

---

## ??? **Step 5: Configure App Service Features**

### **Enable Application Insights**

```bash
# Create Application Insights
az monitor app-insights component create \
  --app appinsights-az204-simple \
  --location eastus \
  --resource-group rg-az204-compute

# Get instrumentation key
$instrumentationKey = az monitor app-insights component show \
  --app appinsights-az204-simple \
  --resource-group rg-az204-compute \
  --query instrumentationKey -o tsv

# Configure Web App to use Application Insights
az webapp config appsettings set \
  --name $webAppName \
  --resource-group $resourceGroup \
  --settings APPINSIGHTS_INSTRUMENTATIONKEY=$instrumentationKey
```

### **Configure Deployment Slots**

```bash
# Create staging slot
az webapp deployment slot create \
  --name $webAppName \
  --resource-group $resourceGroup \
  --slot staging

# Deploy to staging
az webapp deployment source config-zip \
  --name $webAppName \
  --resource-group $resourceGroup \
  --slot staging \
  --src ./app.zip

# Test staging
# https://webapp-az204-simple-1234-staging.azurewebsites.net

# Swap staging to production
az webapp deployment slot swap \
  --name $webAppName \
  --resource-group $resourceGroup \
  --slot staging \
  --target-slot production
```

### **Configure Auto-Scaling**

```bash
# Create autoscale rule
az monitor autoscale create \
  --name autoscale-az204 \
  --resource-group rg-az204-compute \
  --resource $webAppName \
  --resource-type Microsoft.Web/sites \
  --min-count 1 \
  --max-count 3 \
  --count 1

# Add scale-out rule (CPU > 70%)
az monitor autoscale rule create \
  --resource-group rg-az204-compute \
  --autoscale-name autoscale-az204 \
  --condition "Percentage CPU > 70 avg 5m" \
  --scale out 1

# Add scale-in rule (CPU < 30%)
az monitor autoscale rule create \
  --resource-group rg-az204-compute \
  --autoscale-name autoscale-az204 \
  --condition "Percentage CPU < 30 avg 5m" \
  --scale in 1
```

---

## ?? **Step 6: Monitor Your App**

### **View Logs**

```bash
# Stream logs in real-time
az webapp log tail \
  --name $webAppName \
  --resource-group $resourceGroup

# Download logs
az webapp log download \
  --name $webAppName \
  --resource-group $resourceGroup \
  --log-file logs.zip
```

### **Check Metrics**

```bash
# View CPU usage
az monitor metrics list \
  --resource /subscriptions/{subscription-id}/resourceGroups/rg-az204-compute/providers/Microsoft.Web/sites/$webAppName \
  --metric "CpuPercentage" \
  --start-time 2024-01-01T00:00:00Z \
  --end-time 2024-01-31T23:59:59Z
```

---

## ?? **Step 7: Security Best Practices**

```bash
# Enable HTTPS only
az webapp update \
  --name $webAppName \
  --resource-group $resourceGroup \
  --https-only true

# Set minimum TLS version
az webapp config set \
  --name $webAppName \
  --resource-group $resourceGroup \
  --min-tls-version 1.2

# Configure IP restrictions (optional)
az webapp config access-restriction add \
  --name $webAppName \
  --resource-group $resourceGroup \
  --rule-name "Allow-Office" \
  --action Allow \
  --ip-address 203.0.113.0/24 \
  --priority 100
```

---

## ?? **Step 8: Cleanup (When Done)**

```bash
# Delete the web app
az webapp delete \
  --name $webAppName \
  --resource-group $resourceGroup

# Delete the App Service Plan
az appservice plan delete \
  --name plan-az204-simple \
  --resource-group $resourceGroup \
  --yes

# Or delete entire resource group (CAUTION!)
# az group delete --name rg-az204-compute --yes
```

---

## ?? **AZ-204 Exam Tips**

### **App Service Plan Tiers**

| Tier | Use Case | Key Features |
|------|----------|--------------|
| **F1 (Free)** | Dev/Test | 1 GB RAM, 60 min/day, No SLA |
| **D1 (Shared)** | Dev/Test | 1 GB RAM, Shared compute |
| **B1 (Basic)** | Low traffic | Dedicated, Manual scale |
| **S1 (Standard)** | Production | Auto-scale, 5 slots, Custom domains |
| **P1V2 (Premium)** | High performance | Better performance, 20 slots |
| **I1 (Isolated)** | Max security | Private VNet, Dedicated ASE |

### **Common Exam Scenarios**

**Q: How to deploy with zero downtime?**
- **A:** Use deployment slots ? deploy to staging ? swap to production

**Q: App needs to read from Key Vault?**
- **A:** Enable Managed Identity ? Grant Key Vault access ? Use DefaultAzureCredential

**Q: Handle traffic spikes automatically?**
- **A:** Configure auto-scaling rules based on CPU/Memory/HTTP queue

**Q: App runs slowly after idle?**
- **A:** Enable "Always On" (requires Basic tier or higher)

---

## ?? **Complete Deployment Script**

Save this as `deploy.ps1`:

```powershell
# Complete deployment script for AZ-204 Simple Web App

param(
    [string]$ResourceGroup = "rg-az204-compute",
    [string]$Location = "eastus",
    [string]$AppServicePlan = "plan-az204-simple",
    [string]$WebAppName = "webapp-az204-simple-$(Get-Random -Maximum 9999)",
    [string]$KeyVaultName = "kv-az204-9199"
)

Write-Host "?? Starting deployment..." -ForegroundColor Cyan

# Login
az login
az account set --subscription "c01b8f09-10c7-4d70-b719-05b1b231f9d3"

# Create App Service Plan
Write-Host "`n?? Creating App Service Plan..." -ForegroundColor Yellow
az appservice plan create `
  --name $AppServicePlan `
  --resource-group $ResourceGroup `
  --location $Location `
  --sku B1 `
  --is-linux

# Create Web App
Write-Host "`n?? Creating Web App..." -ForegroundColor Yellow
az webapp create `
  --name $WebAppName `
  --resource-group $ResourceGroup `
  --plan $AppServicePlan `
  --runtime "DOTNET|9.0"

# Enable Managed Identity
Write-Host "`n?? Enabling Managed Identity..." -ForegroundColor Yellow
az webapp identity assign `
  --name $WebAppName `
  --resource-group $ResourceGroup

# Grant Key Vault access
Write-Host "`n?? Granting Key Vault access..." -ForegroundColor Yellow
$principalId = az webapp identity show `
  --name $WebAppName `
  --resource-group $ResourceGroup `
  --query principalId -o tsv

$kvId = az keyvault show --name $KeyVaultName --query id -o tsv
az role assignment create `
  --role "Key Vault Secrets User" `
  --assignee $principalId `
  --scope $kvId

# Configure settings
Write-Host "`n??  Configuring App Settings..." -ForegroundColor Yellow
az webapp config appsettings set `
  --name $WebAppName `
  --resource-group $ResourceGroup `
  --settings `
    KeyVault__Uri="https://$KeyVaultName.vault.azure.net/" `
    ASPNETCORE_ENVIRONMENT="Production"

# Publish app
Write-Host "`n?? Publishing application..." -ForegroundColor Yellow
cd "Azure- 204/SimpleWebApp"
dotnet publish -c Release -o ./publish
Compress-Archive -Path ./publish/* -DestinationPath ./app.zip -Force

# Deploy
Write-Host "`n?? Deploying to Azure..." -ForegroundColor Yellow
az webapp deployment source config-zip `
  --name $WebAppName `
  --resource-group $ResourceGroup `
  --src ./app.zip

# Cleanup
Remove-Item ./publish -Recurse -Force
Remove-Item ./app.zip -Force

# Done
Write-Host "`n? Deployment complete!" -ForegroundColor Green
Write-Host "`nWeb App URL: https://$WebAppName.azurewebsites.net" -ForegroundColor Cyan
Write-Host "Swagger UI: https://$WebAppName.azurewebsites.net/swagger" -ForegroundColor Cyan
Write-Host "`nTest endpoints:" -ForegroundColor Yellow
Write-Host "  curl https://$WebAppName.azurewebsites.net/" -ForegroundColor Gray
Write-Host "  curl https://$WebAppName.azurewebsites.net/health" -ForegroundColor Gray
Write-Host "  curl https://$WebAppName.azurewebsites.net/api/info" -ForegroundColor Gray

# Open in browser
Start-Process "https://$WebAppName.azurewebsites.net/swagger"
```

Run it:
```powershell
cd "Azure- 204"
.\deploy.ps1
```

---

## ?? **You're Done!**

Your web app is now running in Azure App Service! 

**Next Steps:**
- Practice creating deployment slots
- Try auto-scaling configuration
- Set up continuous deployment from GitHub
- Monitor with Application Insights

Good luck with your AZ-204 exam! ??
