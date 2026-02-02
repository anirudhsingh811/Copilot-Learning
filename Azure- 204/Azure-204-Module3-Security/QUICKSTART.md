# ?? Quick Start Guide - AZ-204 Security Module

This guide will get you up and running in 10 minutes!

## ? Prerequisites Checklist

- [ ] Azure subscription (free tier works!)
- [ ] Azure CLI installed
- [ ] .NET 9 SDK installed
- [ ] Git (optional, for cloning)

## ?? Step-by-Step Setup

### Step 1: Authenticate with Azure (2 minutes)

```powershell
# Login to Azure
az login

# Verify your subscription
az account show

# Set default subscription (if you have multiple)
az account set --subscription "Your Subscription Name"
```

### Step 2: Create Azure Resources (5 minutes)

Run this script to create all required resources:

```powershell
# Set variables (change <unique> to something unique like your initials + numbers)
$uniqueId = "<unique>"  # Example: "jd123"
$resourceGroup = "rg-az204-security"
$location = "eastus"
$keyVaultName = "kv-az204-demo-$uniqueId"
$storageAccountName = "staz204demo$uniqueId"

# Create resource group
Write-Host "Creating resource group..." -ForegroundColor Cyan
az group create --name $resourceGroup --location $location

# Create Key Vault
Write-Host "Creating Key Vault..." -ForegroundColor Cyan
az keyvault create `
  --name $keyVaultName `
  --resource-group $resourceGroup `
  --location $location `
  --enable-soft-delete true

# Get your user principal name and grant permissions
Write-Host "Granting Key Vault permissions..." -ForegroundColor Cyan
$userPrincipalName = az ad signed-in-user show --query userPrincipalName -o tsv
az keyvault set-policy `
  --name $keyVaultName `
  --upn $userPrincipalName `
  --secret-permissions get list set delete recover `
  --key-permissions get list create delete recover `
  --certificate-permissions get list create delete recover

# Create Storage Account
Write-Host "Creating Storage Account..." -ForegroundColor Cyan
az storage account create `
  --name $storageAccountName `
  --resource-group $resourceGroup `
  --location $location `
  --sku Standard_LRS `
  --kind StorageV2

# Get storage account key
$storageKey = az storage account keys list `
  --account-name $storageAccountName `
  --resource-group $resourceGroup `
  --query "[0].value" -o tsv

# Display configuration values
Write-Host "`n? Resources created successfully!" -ForegroundColor Green
Write-Host "`n?? Save these values:" -ForegroundColor Yellow
Write-Host "Key Vault URI: https://$keyVaultName.vault.azure.net/" -ForegroundColor White
Write-Host "Storage Account Name: $storageAccountName" -ForegroundColor White
Write-Host "Storage Account Key: $storageKey" -ForegroundColor White
```

### Step 3: Create App Registration (3 minutes)

**Option A: Using Azure Portal** (Recommended for first time)

1. Go to: https://portal.azure.com
2. Navigate to: **Azure Active Directory** > **App registrations** > **New registration**
3. Fill in:
   - **Name:** `AZ204-Security-Demo`
   - **Account type:** "Single tenant"
   - **Redirect URI:** Select "Public client/native" and enter `http://localhost`
4. Click **Register**
5. Copy the **Application (client) ID** and **Directory (tenant) ID**
6. Go to **API permissions** > **Add a permission**
7. Select **Microsoft Graph** > **Delegated permissions**
8. Add: `User.Read`, `Mail.Read`, `Calendars.Read`
9. Click **Grant admin consent**

**Option B: Using Azure CLI**

```powershell
Write-Host "Creating App Registration..." -ForegroundColor Cyan

# Create app registration
$appId = az ad app create `
  --display-name "AZ204-Security-Demo" `
  --sign-in-audience AzureADMyOrg `
  --public-client-redirect-uris "http://localhost" `
  --query appId -o tsv

# Get tenant ID
$tenantId = az account show --query tenantId -o tsv

# Add Microsoft Graph permissions
az ad app permission add --id $appId --api 00000003-0000-0000-c000-000000000000 --api-permissions e1fe6dd8-ba31-4d61-89e7-88639da4683d=Scope
az ad app permission add --id $appId --api 00000003-0000-0000-c000-000000000000 --api-permissions 570282fd-fa5c-430d-a7fd-fc8dc98a9dca=Scope
az ad app permission add --id $appId --api 00000003-0000-0000-c000-000000000000 --api-permissions 465a38f9-76ea-45b9-9f34-9e8b0d4b0b42=Scope

# Grant admin consent
az ad app permission admin-consent --id $appId

Write-Host "`n? App Registration created!" -ForegroundColor Green
Write-Host "`n?? Application Details:" -ForegroundColor Yellow
Write-Host "Tenant ID: $tenantId" -ForegroundColor White
Write-Host "Client ID (Application ID): $appId" -ForegroundColor White
```

### Step 4: Configure the Application (1 minute)

**Option A: Using User Secrets** (Recommended - keeps secrets secure)

```powershell
# Navigate to project directory
cd Azure-204-Module3-Security

# Initialize user secrets
dotnet user-secrets init

# Set your configuration (replace with your values)
dotnet user-secrets set "Azure:TenantId" "your-tenant-id-here"
dotnet user-secrets set "Azure:ClientId" "your-client-id-here"
dotnet user-secrets set "Azure:SubscriptionId" "your-subscription-id-here"
dotnet user-secrets set "KeyVault:VaultUri" "https://kv-az204-demo-<unique>.vault.azure.net/"

# Set storage account credentials as environment variables
$env:AZURE_STORAGE_ACCOUNT_NAME = "staz204demo<unique>"
$env:AZURE_STORAGE_ACCOUNT_KEY = "your-storage-key-here"

Write-Host "`n? Configuration complete!" -ForegroundColor Green
```

**Option B: Using appsettings.json** (Quick but less secure)

Edit `appsettings.json`:

```json
{
  "Azure": {
    "TenantId": "your-tenant-id",
    "ClientId": "your-client-id",
    "SubscriptionId": "your-subscription-id"
  },
  "KeyVault": {
    "VaultUri": "https://kv-az204-demo-<unique>.vault.azure.net/"
  }
}
```

And set environment variables:
```powershell
$env:AZURE_STORAGE_ACCOUNT_NAME = "staz204demo<unique>"
$env:AZURE_STORAGE_ACCOUNT_KEY = "your-storage-key"
```

### Step 5: Run the Application! ??

```powershell
# Restore packages
dotnet restore

# Build
dotnet build

# Run
dotnet run
```

You should see the interactive menu! Try starting with:
- **[4] Managed Identity** - Works without additional setup
- **[1] Key Vault** - If you completed the Azure setup
- **[5] SAS** - If you set storage account credentials

## ?? What to Try First

### Beginner Path
1. **Managed Identity Demo** - Understand authentication concepts
2. **Key Vault Demo** - See secrets management in action
3. **SAS Demo** - Learn about secure storage access

### Advanced Path
1. **MSAL Authentication** - Interactive authentication flows
2. **Microsoft Graph** - Access Office 365 data
3. **Run All Demos** - See everything in action

## ?? Common Issues

### Issue: "Key Vault access denied"

**Solution:**
```powershell
# Re-grant permissions
$userPrincipalName = az ad signed-in-user show --query userPrincipalName -o tsv
az keyvault set-policy `
  --name kv-az204-demo-<unique> `
  --upn $userPrincipalName `
  --secret-permissions all `
  --key-permissions all `
  --certificate-permissions all
```

### Issue: "MSAL authentication fails"

**Solution:**
- Verify Tenant ID and Client ID are correct
- Check redirect URI is set to `http://localhost`
- Ensure API permissions are granted

### Issue: "Storage SAS fails"

**Solution:**
- Verify environment variables are set correctly:
  ```powershell
  echo $env:AZURE_STORAGE_ACCOUNT_NAME
  echo $env:AZURE_STORAGE_ACCOUNT_KEY
  ```
- Check storage account key is correct

### Issue: "CredentialUnavailableException"

**Solution:**
```powershell
# Make sure you're logged in to Azure CLI
az login
az account show
```

## ?? Cost Estimate

Running these demos for learning purposes:
- **Key Vault:** ~$0.03/10,000 operations
- **Storage Account:** ~$0.02/GB (we use minimal storage)
- **App Registration:** Free
- **Managed Identity:** Free

**Total estimated cost for 1 week of learning: < $1 USD**

## ?? Cleanup (When Done)

Delete everything to avoid charges:

```powershell
# Delete all resources in one command
az group delete --name rg-az204-security --yes --no-wait

# Delete app registration
az ad app delete --id <your-client-id>

# Purge soft-deleted Key Vault (optional, if you want to reuse the name)
az keyvault purge --name kv-az204-demo-<unique>
```

## ?? Next Steps

1. ? Run each demo individually
2. ? Check menu option **[7] Setup Guide** for detailed Azure Portal instructions
3. ? Review **[8] AZ-204 Exam Tips** for certification preparation
4. ? Read the full **README.md** for in-depth explanations
5. ? Explore the code in the `Services/` folder

## ?? Learning Resources

- **Official Microsoft Learn:** https://learn.microsoft.com/training/paths/az-204-implement-authentication-authorization/
- **Azure CLI Reference:** https://learn.microsoft.com/cli/azure/
- **Microsoft Graph Explorer:** https://developer.microsoft.com/graph/graph-explorer
- **MSAL Documentation:** https://learn.microsoft.com/azure/active-directory/develop/msal-overview

## ? Need Help?

1. Check the error message in the console - it usually provides hints
2. Review the **Setup Guide** in the application menu [7]
3. Check **README.md** for troubleshooting section
4. Verify all configuration values are correct

---

**You're all set! Time to learn Azure Security! ??**

Run `dotnet run` and start exploring!
