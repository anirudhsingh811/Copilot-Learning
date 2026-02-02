# ?? Quick Start - Azure App Service Manager

## ? Ready to Create Real Azure Resources!

The `AppServiceManager` is **FULLY ENABLED** and ready to use!

## ?? Pre-Flight Checklist

Before running, ensure you have:

- [ ] Azure subscription (get free: https://azure.microsoft.com/free/)
- [ ] Azure CLI installed: `az --version`
- [ ] Logged in to Azure: `az login`
- [ ] Resource group created or existing
- [ ] Updated subscription ID in code (line 47 of `8-WebApp-Program.cs`)

## ?? Quick Setup

### 1. Get Your Subscription ID
```bash
az account show --query id --output tsv
```

### 2. Create a Resource Group (if needed)
```bash
az group create --name MyRGname --location eastus
```

### 3. Update the Code
Open `8-WebApp-Program.cs` and update line 47:
```csharp
string subscriptionId = "paste-your-subscription-id-here";
```

### 4. Run the Application
```bash
dotnet run
```

Choose option **1** from the menu.

## ?? What Will Be Created

| Resource | Type | SKU/Tier | Estimated Cost* |
|----------|------|----------|-----------------|
| App Service Plan | Linux | B1 (Basic) | ~$13/month |
| Web App | .NET 8.0 | - | Included in plan |
| Deployment Slot | Staging | - | Included |
| Managed Identity | System-assigned | - | Free |

*Costs are approximate. Check Azure pricing for your region.

## ?? Execution Flow

1. **Creates App Service Plan** (Basic B1, Linux)
2. **Creates Web App** (.NET Core 8.0)
3. **Configures App Settings** (shows CLI commands)
4. **Enables Managed Identity** (system-assigned)
5. **Shows Scaling Options** (displays CLI commands)
6. **Creates Deployment Slot** (staging)
7. **Gets Web App Metrics** (displays current state)
8. **Shows Slot Swap** (displays CLI commands)

## ?? Debugging Tips

### Set Breakpoints Here:
- `CreateWebAppAsync` - Watch the web app being created
- `EnableManagedIdentityAsync` - See identity assignment
- `CreateDeploymentSlotAsync` - Observe slot creation

### Common Issues:

**Error: Subscription not found**
- Run: `az account list --output table`
- Set correct subscription: `az account set --subscription "your-id"`

**Error: Resource group not found**
- Create it: `az group create --name MyRGname --location eastus`

**Error: Name already exists**
- App names must be globally unique
- The code uses GUID suffix to help avoid this

**Error: Authorization failed**
- Check your role: `az role assignment list --assignee $(az account show --query user.name -o tsv)`
- You need at least "Contributor" role on the subscription/resource group

## ?? Cleanup Resources

After testing, delete resources to avoid charges:

```bash
# Delete the entire resource group (?? deletes everything in it!)
az group delete --name MyRGname --yes --no-wait

# Or delete individual resources
az webapp delete --name <your-webapp-name> --resource-group MyRGname
az appservice plan delete --name myAppServicePlan --resource-group MyRGname
```

## ?? Pro Tips

1. **Use a separate resource group** for testing - easy to clean up!
2. **Monitor costs** in Azure Portal > Cost Management
3. **Free tier** is available for App Service Plans (F1) but has limitations
4. **Auto-shutdown** isn't available for App Service, so remember to delete when done!

## ?? Next Steps

Once you've successfully created resources:

1. **Deploy an app**: Use `az webapp deployment source config-zip`
2. **Add custom domain**: `az webapp config hostname add`
3. **Setup SSL**: `az webapp config ssl bind`
4. **Configure monitoring**: Enable Application Insights
5. **Setup CI/CD**: Connect to GitHub Actions or Azure DevOps

## ?? Need Help?

- Azure docs: https://docs.microsoft.com/azure/app-service/
- Azure CLI reference: https://docs.microsoft.com/cli/azure/webapp
- Pricing calculator: https://azure.microsoft.com/pricing/calculator/

---

**Ready?** Make sure you've updated your subscription ID, then run `dotnet run` and choose option 1! ??
