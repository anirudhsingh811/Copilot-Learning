# ?? Quick Start Guide

## ? **Step 1: Test Locally (5 minutes)**

```powershell
# Navigate to the folder
cd "C:\Users\INASING7\source\repos\Azure- 204\Azure- 204\SimpleWebApp"

# Run the app
dotnet run

# You should see:
# Now listening on: http://localhost:5000
# Now listening on: https://localhost:5001
```

**Open in your browser:**
- http://localhost:5000 - Home page
- http://localhost:5000/swagger - Interactive API documentation
- http://localhost:5000/health - Health check

**Test with curl:**
```powershell
curl http://localhost:5000/
curl http://localhost:5000/health
curl http://localhost:5000/api/info
```

---

## ?? **Step 2: Deploy to Azure (10 minutes)**

### **Automatic Deployment (Recommended)**

```powershell
# Make sure you're in the SimpleWebApp folder
cd "C:\Users\INASING7\source\repos\Azure- 204\Azure- 204\SimpleWebApp"

# Run the deployment script
.\deploy.ps1
```

This script will:
1. ? Login to Azure
2. ? Create App Service Plan (B1 tier)
3. ? Create Web App
4. ? Enable Managed Identity
5. ? Grant Key Vault access
6. ? Deploy your app
7. ? Open Swagger UI in browser

**?? Takes about 5-8 minutes**

---

### **Manual Deployment (Learning Path)**

If you want to do it step-by-step to learn:

```powershell
# 1. Login to Azure
az login
az account set --subscription "c01b8f09-10c7-4d70-b719-05b1b231f9d3"

# 2. Create App Service Plan
az appservice plan create `
  --name plan-az204-simple `
  --resource-group rg-az204-compute `
  --sku B1 `
  --is-linux

# 3. Create Web App (change the name to make it unique)
$webAppName = "webapp-az204-$([guid]::NewGuid().ToString().Substring(0,8))"
az webapp create `
  --name $webAppName `
  --resource-group rg-az204-compute `
  --plan plan-az204-simple `
  --runtime "DOTNET|9.0"

# 4. Enable Managed Identity
az webapp identity assign `
  --name $webAppName `
  --resource-group rg-az204-compute

# 5. Grant Key Vault access
$principalId = az webapp identity show `
  --name $webAppName `
  --resource-group rg-az204-compute `
  --query principalId -o tsv

az role assignment create `
  --role "Key Vault Secrets User" `
  --assignee $principalId `
  --scope "/subscriptions/c01b8f09-10c7-4d70-b719-05b1b231f9d3/resourceGroups/rg-az204-compute/providers/Microsoft.KeyVault/vaults/kv-az204-9199"

# 6. Configure App Settings
az webapp config appsettings set `
  --name $webAppName `
  --resource-group rg-az204-compute `
  --settings KeyVault__Uri="https://kv-az204-9199.vault.azure.net/"

# 7. Publish and deploy
dotnet publish -c Release -o ./publish
Compress-Archive -Path ./publish/* -DestinationPath ./app.zip -Force
az webapp deployment source config-zip `
  --name $webAppName `
  --resource-group rg-az204-compute `
  --src ./app.zip

# 8. Open in browser
Write-Host "Your app URL: https://$webAppName.azurewebsites.net" -ForegroundColor Green
Start-Process "https://$webAppName.azurewebsites.net/swagger"

# Cleanup
Remove-Item ./publish -Recurse -Force
Remove-Item ./app.zip -Force
```

---

## ?? **Step 3: Test Your Deployed App**

After deployment, your web app name will be shown. Replace `<your-webapp-name>` below:

```powershell
# Set your web app name
$webAppName = "webapp-az204-simple-1234"  # Replace with actual name

# Test endpoints
curl "https://$webAppName.azurewebsites.net/"
curl "https://$webAppName.azurewebsites.net/health"
curl "https://$webAppName.azurewebsites.net/api/info"
curl "https://$webAppName.azurewebsites.net/api/env"
curl "https://$webAppName.azurewebsites.net/api/secrets"

# Open Swagger UI
Start-Process "https://$webAppName.azurewebsites.net/swagger"
```

---

## ?? **Step 4: View Logs**

```powershell
# Stream logs in real-time
az webapp log tail `
  --name $webAppName `
  --resource-group rg-az204-compute

# Press Ctrl+C to stop
```

---

## ?? **Step 5: Practice AZ-204 Scenarios**

### **A) Deployment Slots (Zero-Downtime Deployment)**

```powershell
# Create staging slot
az webapp deployment slot create `
  --name $webAppName `
  --resource-group rg-az204-compute `
  --slot staging

# Deploy to staging (change some code first)
az webapp deployment source config-zip `
  --name $webAppName `
  --resource-group rg-az204-compute `
  --slot staging `
  --src ./app.zip

# Test staging: https://<your-webapp>-staging.azurewebsites.net

# Swap to production
az webapp deployment slot swap `
  --name $webAppName `
  --resource-group rg-az204-compute `
  --slot staging
```

### **B) Auto-Scaling**

```powershell
# View current plan
az appservice plan show `
  --name plan-az204-simple `
  --resource-group rg-az204-compute

# Update to Standard tier (required for auto-scaling)
az appservice plan update `
  --name plan-az204-simple `
  --resource-group rg-az204-compute `
  --sku S1

# Create autoscale setting
az monitor autoscale create `
  --resource-group rg-az204-compute `
  --resource $webAppName `
  --resource-type Microsoft.Web/sites `
  --name autoscale-az204 `
  --min-count 1 `
  --max-count 3 `
  --count 1

# Add scale-out rule (CPU > 70%)
az monitor autoscale rule create `
  --resource-group rg-az204-compute `
  --autoscale-name autoscale-az204 `
  --condition "Percentage CPU > 70 avg 5m" `
  --scale out 1
```

### **C) Application Insights**

```powershell
# Create Application Insights
az monitor app-insights component create `
  --app appinsights-az204 `
  --location eastus `
  --resource-group rg-az204-compute

# Get connection string
$connectionString = az monitor app-insights component show `
  --app appinsights-az204 `
  --resource-group rg-az204-compute `
  --query connectionString -o tsv

# Add to Web App
az webapp config appsettings set `
  --name $webAppName `
  --resource-group rg-az204-compute `
  --settings APPLICATIONINSIGHTS_CONNECTION_STRING="$connectionString"
```

---

## ?? **Step 6: Cleanup**

When you're done practicing:

```powershell
# Delete the web app
az webapp delete `
  --name $webAppName `
  --resource-group rg-az204-compute

# Delete the App Service Plan
az appservice plan delete `
  --name plan-az204-simple `
  --resource-group rg-az204-compute `
  --yes

# Or delete everything (CAUTION!)
# az group delete --name rg-az204-compute --yes
```

---

## ? **Troubleshooting**

### **App not responding?**
```powershell
# Check if app is running
az webapp show --name $webAppName --resource-group rg-az204-compute --query state

# Restart the app
az webapp restart --name $webAppName --resource-group rg-az204-compute

# Check logs
az webapp log tail --name $webAppName --resource-group rg-az204-compute
```

### **Can't access Key Vault?**
```powershell
# Verify Managed Identity is enabled
az webapp identity show --name $webAppName --resource-group rg-az204-compute

# Check role assignment
az role assignment list --assignee <principal-id>
```

### **Deployment failed?**
```powershell
# Check deployment logs
az webapp log deployment show --name $webAppName --resource-group rg-az204-compute
```

---

## ?? **Success Checklist**

- [ ] App runs locally on http://localhost:5000
- [ ] Swagger UI opens at http://localhost:5000/swagger
- [ ] Deployed to Azure successfully
- [ ] Can access https://<your-app>.azurewebsites.net
- [ ] Health check returns "Healthy"
- [ ] Key Vault integration works (reads ApiKey secret)
- [ ] Logs are streaming

---

## ?? **What You Learned (AZ-204)**

? **Create and configure App Service**
? **Deploy .NET apps to Azure**
? **Use Managed Identity** (no credentials in code!)
? **Access Key Vault** from App Service
? **Monitor with health checks**
? **Stream application logs**
? **Configure app settings**

---

## ?? **Next Steps**

1. **Practice deployment slots** - Create staging, swap to production
2. **Set up auto-scaling** - Handle traffic spikes
3. **Add Application Insights** - Monitor performance
4. **Try custom domains** - Add your own domain
5. **Configure CI/CD** - GitHub Actions or Azure DevOps

---

**Good luck with your AZ-204 exam!** ??
