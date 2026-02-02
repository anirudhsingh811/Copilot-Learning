# ?? Simple Web App for AZ-204 Study

A minimal ASP.NET Core Web API to practice **Azure App Service** deployment for AZ-204 certification.

## ?? Features

- ? RESTful API with Swagger UI
- ? Health checks endpoint
- ? Azure Key Vault integration with Managed Identity
- ? Environment information endpoint
- ? Ready for App Service deployment

## ?? Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/` | Welcome message with server info |
| GET | `/health` | Health check |
| GET | `/api/info` | Application information |
| GET | `/api/env` | Environment variables (shows App Service info) |
| GET | `/api/echo/{message}` | Echo test |
| GET | `/api/secrets` | Test Key Vault access |
| GET | `/swagger` | Swagger UI |

## ?? Quick Start - Local Testing

```powershell
# Navigate to the folder
cd "Azure- 204/SimpleWebApp"

# Restore packages
dotnet restore

# Run locally
dotnet run

# Open in browser
start http://localhost:5000
start http://localhost:5000/swagger
```

## ?? Deploy to Azure

### **Option 1: One-Click Deploy**

```powershell
cd "Azure- 204/SimpleWebApp"
.\deploy.ps1
```

This will:
- ? Create App Service Plan
- ? Create Web App
- ? Enable Managed Identity
- ? Grant Key Vault access
- ? Deploy the app
- ? Open Swagger UI in browser

### **Option 2: Manual Deploy**

Follow the detailed guide in **[DEPLOYMENT.md](DEPLOYMENT.md)**

## ?? Test Your Deployed App

```powershell
# Replace with your web app name
$webAppName = "webapp-az204-simple-1234"

# Test endpoints
curl "https://$webAppName.azurewebsites.net/"
curl "https://$webAppName.azurewebsites.net/health"
curl "https://$webAppName.azurewebsites.net/api/info"
curl "https://$webAppName.azurewebsites.net/api/env"
curl "https://$webAppName.azurewebsites.net/api/secrets"
```

## ?? Monitor Your App

```powershell
# Stream logs
az webapp log tail --name $webAppName --resource-group rg-az204-compute

# Download logs
az webapp log download --name $webAppName --resource-group rg-az204-compute
```

## ?? AZ-204 Learning Points

This app demonstrates:

1. **App Service Deployment**
   - Creating App Service resources
   - Publishing .NET apps
   - Configuration management

2. **Managed Identity**
   - System-assigned identity
   - Key Vault access without credentials
   - Azure RBAC integration

3. **Configuration**
   - App Settings
   - Environment variables
   - Key Vault references

4. **Health Checks**
   - Built-in health endpoint
   - App Service health monitoring

5. **Monitoring**
   - Application logs
   - Diagnostic logs
   - Environment information

## ?? Cleanup

```powershell
# Delete the web app
az webapp delete --name $webAppName --resource-group rg-az204-compute

# Delete the App Service Plan
az appservice plan delete --name plan-az204-simple --resource-group rg-az204-compute --yes
```

## ?? Next Steps

- Create a **deployment slot** for staging
- Configure **auto-scaling**
- Add **Application Insights**
- Set up **continuous deployment** from GitHub
- Practice **slot swapping**

## ?? Troubleshooting

**App not responding?**
```powershell
# Check logs
az webapp log tail --name $webAppName --resource-group rg-az204-compute
```

**Can't access Key Vault?**
```powershell
# Check Managed Identity is enabled
az webapp identity show --name $webAppName --resource-group rg-az204-compute

# Check role assignment
az role assignment list --assignee <principal-id> --scope <key-vault-id>
```

**Deployment failed?**
```powershell
# Check deployment logs
az webapp log deployment show --name $webAppName --resource-group rg-az204-compute
```

---

Good luck with your AZ-204 certification! ??
