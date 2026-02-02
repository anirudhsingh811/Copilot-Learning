# Azure App Service - Complete Guide with Examples

This repository contains comprehensive examples and code for working with Azure App Service.

## ?? Contents

### 1. **Creating and Managing App Service** (`1-CreateAppService.azcli`)
- Create Resource Groups
- Create App Service Plans
- Create Web Apps
- Update configurations

### 2. **App Settings Configuration** (`2-AppSettings.azcli`)
- Set application settings
- Configure connection strings
- Manage custom domains
- Enable HTTPS

### 3. **Authentication & Authorization** (`3-Authentication.azcli`)
- Azure AD authentication
- Microsoft Identity Platform
- Token store configuration
- External redirect URLs
- Google authentication

### 4. **Scaling** (`4-Scaling.azcli`)
- Manual scaling (scale up/down)
- Scale out (add instances)
- Auto-scaling based on metrics:
  - CPU percentage
  - Memory percentage
  - HTTP queue length

### 5. **Deployment Slots** (`5-DeploymentSlots.azcli`)
- Create deployment slots
- Slot-specific settings
- Swap slots (staging ? production)
- Multi-phase swap with preview
- Auto-swap configuration

### 6. **Deployment Methods** (`6-DeploymentMethods.azcli`)
- ZIP deployment
- Local Git
- GitHub Actions
- Azure DevOps / Azure Pipelines
- FTP deployment
- Container deployment
- Cloud storage (OneDrive/Dropbox)

### 7. **Azure SDK for .NET** (`7-WebApp.cs`, `8-WebApp-Program.cs`)
- Programmatic App Service management
- Create and configure web apps
- Manage app settings
- Scale resources
- Deployment slot operations
- Enable Managed Identity

### 8. **ASP.NET Core Web App** (`9-AspNetCore-Startup.cs`, `10-appsettings.json`)
- Azure AD authentication
- Session management
- Health checks
- Application Insights
- Response compression
- Best practices for production

### 9. **Infrastructure as Code** (`11-Bicep-AppService.bicep`)
- Bicep template for App Service
- App Service Plan
- Application Insights integration
- Deployment slots
- Auto-scaling rules
- Managed Identity

## ?? Getting Started

### Prerequisites

1. **Azure CLI** - Install from [here](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli)
2. **Azure Subscription** - [Create free account](https://azure.microsoft.com/free/)
3. **.NET SDK 8.0+** - For C# examples
4. **Visual Studio / VS Code** - For development

### Azure CLI Login

```powershell
az login
az account set --subscription "your-subscription-id"
```

### Running CLI Examples

```powershell
# Execute PowerShell scripts
.\1-CreateAppService.azcli
```

### Running .NET SDK Examples

```powershell
# Install required packages
dotnet add package Azure.ResourceManager.AppService
dotnet add package Azure.Identity

# Update subscription ID in Program.cs
# Run the application
dotnet run
```

### Deploying with Bicep

```powershell
# Deploy the Bicep template
az deployment group create \
  --resource-group myResourceGroup \
  --template-file 11-Bicep-AppService.bicep \
  --parameters webAppName=myuniquewebapp sku=B1
```

## ?? Key Concepts

### **App Service Plans**
- Defines compute resources for your apps
- SKU options: Free (F1), Basic (B1-B3), Standard (S1-S3), Premium (P1V2-P3V3)
- Can host multiple apps in the same plan

### **Deployment Slots**
- Separate environments (staging, production, etc.)
- Swap slots with zero downtime
- Available in Standard tier and above
- Test in staging before production

### **Scaling Options**
- **Scale Up**: Increase compute resources (CPU, memory)
- **Scale Out**: Add more instances
- **Auto-scaling**: Automatically adjust based on metrics

### **Authentication**
- Built-in authentication (Easy Auth)
- Support for Azure AD, Google, Facebook, Twitter
- Token store for authenticated users

### **Best Practices**
- ? Always use HTTPS
- ? Enable Always On for production apps
- ? Use deployment slots for safe deployments
- ? Enable Application Insights for monitoring
- ? Use Managed Identity for secure access
- ? Configure auto-scaling for variable loads
- ? Implement health checks
- ? Use TLS 1.2 or higher

## ?? Additional Resources

- [Azure App Service Documentation](https://docs.microsoft.com/azure/app-service/)
- [Azure CLI Reference](https://docs.microsoft.com/cli/azure/webapp)
- [Azure SDK for .NET](https://docs.microsoft.com/dotnet/azure/)
- [Bicep Documentation](https://docs.microsoft.com/azure/azure-resource-manager/bicep/)

## ?? Tips

1. **Cost Optimization**: Use Basic or Free tier for development/testing
2. **Performance**: Enable HTTP/2 and response compression
3. **Security**: Use Managed Identity instead of connection strings
4. **Monitoring**: Set up Application Insights from the start
5. **CI/CD**: Use GitHub Actions or Azure DevOps for automated deployments

## ?? Troubleshooting

### Common Issues

1. **Deployment fails**: Check App Service logs in Azure Portal
2. **Slow performance**: Enable Always On and consider scaling up
3. **Authentication issues**: Verify Azure AD configuration and redirect URLs
4. **Slot swap fails**: Check slot-sticky settings

### View Logs

```powershell
# Stream logs
az webapp log tail --name mywebapp --resource-group myResourceGroup

# Download logs
az webapp log download --name mywebapp --resource-group myResourceGroup
```

## ?? License

This is sample code for educational purposes.

---

**Happy coding with Azure App Service! ??**
