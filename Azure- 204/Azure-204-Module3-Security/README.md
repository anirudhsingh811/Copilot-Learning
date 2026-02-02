# AZ-204 Module 3: Implement Azure Security

## ?? Overview

This interactive learning module provides **real, runnable examples** covering all Azure Security topics required for the AZ-204 certification exam. Each demo is fully functional and demonstrates real-world Azure security patterns.

**Exam Weight:** 20-25% (Highest Priority!)

## ?? Topics Covered

### 1. Azure Key Vault
- ? Secrets Management (Create, Read, Update, Delete, List versions)
- ? Cryptographic Keys (RSA, EC keys)
- ? Certificates (Self-signed, managed certificates)
- ? Soft delete and recovery
- ? Access using DefaultAzureCredential

### 2. MSAL Authentication
- ? Public Client Application (Interactive browser authentication)
- ? Confidential Client Application (Client credentials flow)
- ? Device Code Flow (For browserless devices)
- ? Token caching and silent acquisition
- ? Multiple authentication flows

### 3. Microsoft Graph API
- ? User Profile retrieval
- ? Reading emails
- ? Calendar events
- ? Organization details
- ? Proper permission scopes

### 4. Managed Identities
- ? System-Assigned Identity detection
- ? User-Assigned Identity usage
- ? DefaultAzureCredential pattern (recommended)
- ? Azure Resource Manager integration
- ? Local development vs production authentication

### 5. Shared Access Signatures (SAS)
- ? Blob Service SAS tokens
- ? Account SAS tokens
- ? Stored Access Policies (revocable SAS)
- ? Permission control and expiration
- ? Real storage operations

## ?? Quick Start

### Prerequisites

1. **Azure Subscription**
   - Free tier is sufficient: https://azure.microsoft.com/free/

2. **Azure CLI**
   ```powershell
   # Install Azure CLI
   winget install Microsoft.AzureCLI
   
   # Or via Chocolatey
   choco install azure-cli
   
   # Authenticate
   az login
   ```

3. **.NET 9 SDK**
   - Already installed (project targets .NET 9)

### Initial Setup

1. **Clone and Navigate**
   ```powershell
   cd Azure-204-Module3-Security
   ```

2. **Install Dependencies**
   ```powershell
   dotnet restore
   ```

3. **Run the Application**
   ```powershell
   dotnet run
   ```

The application will run with an interactive menu. Some demos require Azure resources (see setup sections below).

## ?? Azure Resources Setup

### 1. Key Vault Setup

```powershell
# Create resource group
az group create --name rg-az204-security --location eastus

# Create Key Vault (replace <unique> with your unique identifier)
az keyvault create `
  --name kv-az204-demo-<unique> `
  --resource-group rg-az204-security `
  --location eastus `
  --enable-soft-delete true `
  --enable-purge-protection false

# Grant yourself permissions
$userPrincipalName = az ad signed-in-user show --query userPrincipalName -o tsv

az keyvault set-policy `
  --name kv-az204-demo-<unique> `
  --upn $userPrincipalName `
  --secret-permissions get list set delete recover `
  --key-permissions get list create delete recover `
  --certificate-permissions get list create delete recover
```

### 2. App Registration Setup (for MSAL & Graph)

**Via Azure Portal:**

1. Navigate to: **Azure Portal** > **Azure Active Directory** > **App Registrations**
2. Click **"New registration"**
3. Configure:
   - **Name:** `AZ204-Security-Demo`
   - **Supported account types:** "Accounts in this organizational directory only"
   - **Redirect URI:** 
     - Platform: "Public client/native (mobile & desktop)"
     - URI: `http://localhost`
4. Click **"Register"**
5. Note down:
   - **Application (client) ID**
   - **Directory (tenant) ID**

6. Configure **API Permissions:**
   - Click **"API permissions"** > **"Add a permission"**
   - Select **"Microsoft Graph"** > **"Delegated permissions"**
   - Add:
     - `User.Read`
     - `Mail.Read`
     - `Calendars.Read`
   - Click **"Grant admin consent for [Your Organization]"**

7. For **Confidential Client** (optional):
   - Go to **"Certificates & secrets"**
   - Click **"New client secret"**
   - Description: "Demo Secret"
   - Expires: 6 months
   - Click **"Add"**
   - **Copy the secret value immediately** (it won't be shown again)

**Via Azure CLI:**

```powershell
# Create App Registration
$appId = az ad app create `
  --display-name "AZ204-Security-Demo" `
  --sign-in-audience AzureADMyOrg `
  --public-client-redirect-uris "http://localhost" `
  --query appId -o tsv

echo "Application (Client) ID: $appId"

# Get Tenant ID
$tenantId = az account show --query tenantId -o tsv
echo "Tenant ID: $tenantId"

# Add Microsoft Graph API permissions
az ad app permission add `
  --id $appId `
  --api 00000003-0000-0000-c000-000000000000 `
  --api-permissions `
    e1fe6dd8-ba31-4d61-89e7-88639da4683d=Scope ` # User.Read
    570282fd-fa5c-430d-a7fd-fc8dc98a9dca=Scope ` # Mail.Read
    465a38f9-76ea-45b9-9f34-9e8b0d4b0b42=Scope   # Calendars.Read

# Grant admin consent
az ad app permission admin-consent --id $appId

# Create client secret (for confidential client)
$secret = az ad app credential reset --id $appId --query password -o tsv
echo "Client Secret: $secret (SAVE THIS NOW!)"
```

### 3. Storage Account Setup (for SAS demos)

```powershell
# Create storage account (replace <unique> with your unique identifier)
az storage account create `
  --name staz204demo<unique> `
  --resource-group rg-az204-security `
  --location eastus `
  --sku Standard_LRS `
  --kind StorageV2 `
  --allow-blob-public-access false

# Get account key
$storageKey = az storage account keys list `
  --account-name staz204demo<unique> `
  --resource-group rg-az204-security `
  --query "[0].value" -o tsv

echo "Storage Account Name: staz204demo<unique>"
echo "Storage Account Key: $storageKey"
```

## ?? Configuration

### Option 1: appsettings.json (Not Recommended for Secrets)

Update `appsettings.json`:

```json
{
  "Azure": {
    "TenantId": "your-tenant-id-here",
    "ClientId": "your-client-id-here",
    "ClientSecret": "your-client-secret-here",
    "SubscriptionId": "your-subscription-id-here",
    "ResourceGroup": "rg-az204-security"
  },
  "KeyVault": {
    "VaultUri": "https://kv-az204-demo-<unique>.vault.azure.net/",
    "VaultName": "kv-az204-demo-<unique>"
  }
}
```

### Option 2: User Secrets (Recommended for Local Development)

```powershell
# Initialize user secrets
dotnet user-secrets init

# Set secrets
dotnet user-secrets set "Azure:TenantId" "your-tenant-id"
dotnet user-secrets set "Azure:ClientId" "your-client-id"
dotnet user-secrets set "Azure:ClientSecret" "your-client-secret"
dotnet user-secrets set "Azure:SubscriptionId" "your-subscription-id"
dotnet user-secrets set "KeyVault:VaultUri" "https://kv-az204-demo-<unique>.vault.azure.net/"

# List all secrets
dotnet user-secrets list
```

### Option 3: Environment Variables (Recommended for Production)

```powershell
# Set environment variables
$env:Azure__TenantId = "your-tenant-id"
$env:Azure__ClientId = "your-client-id"
$env:Azure__ClientSecret = "your-client-secret"
$env:Azure__SubscriptionId = "your-subscription-id"
$env:KeyVault__VaultUri = "https://kv-az204-demo-<unique>.vault.azure.net/"
$env:AZURE_STORAGE_ACCOUNT_NAME = "staz204demo<unique>"
$env:AZURE_STORAGE_ACCOUNT_KEY = "your-storage-key"
```

## ?? Usage

Run the application:

```powershell
dotnet run
```

You'll see an interactive menu:

```
?????????????????????????????????????????????????????????????????????
?         AZ-204: Implement Azure Security                         ?
?         Interactive Learning & Hands-On Demos                    ?
?????????????????????????????????????????????????????????????????????

??  Exam Weight: 20-25% (HIGHEST PRIORITY!)

?? Choose a demo to run:

  [1] ?? Azure Key Vault
  [2] ?? MSAL Authentication
  [3] ?? Microsoft Graph API
  [4] ?? Managed Identity
  [5] ?? Shared Access Signatures (SAS)
  [6] ?? Run All Demos
  [7] ?? Setup Guide
  [8] ?? AZ-204 Exam Tips
  [Q] ? Exit
```

### Demo Descriptions

**[1] Azure Key Vault** - Demonstrates:
- Creating and retrieving secrets
- Managing cryptographic keys
- Working with certificates
- Listing versions and properties
- Soft delete operations

**[2] MSAL Authentication** - Demonstrates:
- Interactive browser authentication (Public Client)
- Service-to-service authentication (Confidential Client)
- Device code flow for browserless devices
- Token caching and silent acquisition

**[3] Microsoft Graph API** - Demonstrates:
- Retrieving user profile information
- Reading emails from mailbox
- Accessing calendar events
- Getting organization details
- Proper permission scopes

**[4] Managed Identity** - Demonstrates:
- System-assigned identity detection
- User-assigned identity usage
- DefaultAzureCredential authentication chain
- Best practices for development and production

**[5] Shared Access Signatures** - Demonstrates:
- Creating blob-level SAS tokens
- Account-level SAS tokens
- Stored access policies (revocable SAS)
- Permission control and testing

## ?? AZ-204 Exam Tips

### High-Priority Topics

1. **Key Vault**
   - Secrets vs Keys vs Certificates
   - Soft delete and purge protection
   - Access policies vs RBAC
   - Managed Identity access to Key Vault

2. **Managed Identities**
   - System-assigned vs User-assigned
   - When to use each type
   - DefaultAzureCredential

3. **MSAL/OAuth**
   - Authentication flows (Auth Code, Client Credentials, Device Code)
   - Delegated vs Application permissions
   - Token lifetime and refresh

4. **SAS Tokens**
   - Service SAS vs Account SAS vs User Delegation SAS
   - Stored access policies for revocation
   - Best security practices

5. **Microsoft Graph**
   - Common endpoints and operations
   - Permission types and consent
   - Batch requests and delta queries

### Common Exam Scenarios

- **Scenario:** An app needs to access Key Vault without storing credentials
  - **Answer:** Use Managed Identity

- **Scenario:** Need to grant temporary access to a specific blob
  - **Answer:** Generate a Service SAS token with appropriate permissions and expiration

- **Scenario:** Background service needs to read emails from all users
  - **Answer:** Use Confidential Client with Application permissions (Mail.Read)

- **Scenario:** Need to revoke SAS tokens without changing storage keys
  - **Answer:** Use Stored Access Policies

## ?? Troubleshooting

### "Access denied" errors with Key Vault

```powershell
# Verify your user has correct permissions
az keyvault show --name kv-az204-demo-<unique> --query properties.accessPolicies

# Add permissions if missing
$userObjectId = az ad signed-in-user show --query id -o tsv
az keyvault set-policy --name kv-az204-demo-<unique> --object-id $userObjectId --secret-permissions get list set delete
```

### MSAL authentication fails

- Ensure redirect URI is configured correctly: `http://localhost`
- Check API permissions are granted and admin consent is given
- Verify TenantId and ClientId are correct

### Storage SAS operations fail

- Verify storage account key is correct
- Check firewall rules on storage account
- Ensure correct storage account name in environment variables

### "CredentialUnavailableException"

- Run `az login` to authenticate Azure CLI
- Alternatively, configure environment variables or managed identity

## ?? Cleanup

To avoid charges, delete resources when done:

```powershell
# Delete the entire resource group (removes all resources)
az group delete --name rg-az204-security --yes --no-wait

# Or delete individual resources
az keyvault delete --name kv-az204-demo-<unique>
az storage account delete --name staz204demo<unique> --resource-group rg-az204-security --yes

# Delete App Registration
az ad app delete --id <your-app-id>

# Purge soft-deleted Key Vault (if needed)
az keyvault purge --name kv-az204-demo-<unique>
```

## ?? Additional Resources

- **Microsoft Learn:** https://learn.microsoft.com/training/paths/az-204-implement-authentication-authorization/
- **Azure SDK Documentation:** https://docs.microsoft.com/azure/developer/
- **Microsoft Graph Explorer:** https://developer.microsoft.com/graph/graph-explorer
- **MSAL Documentation:** https://docs.microsoft.com/azure/active-directory/develop/msal-overview
- **Key Vault Documentation:** https://docs.microsoft.com/azure/key-vault/

## ?? Code Structure

```
Azure-204-Module3-Security/
??? Program.cs                              # Main entry point with interactive menu
??? Services/
?   ??? KeyVaultService.cs                 # Azure Key Vault operations
?   ??? MsalAuthenticationService.cs       # MSAL authentication flows
?   ??? GraphApiService.cs                 # Microsoft Graph API calls
?   ??? ManagedIdentityService.cs          # Managed Identity demonstrations
?   ??? SharedAccessSignatureService.cs    # SAS token generation and usage
??? appsettings.json                        # Configuration (non-sensitive)
??? README.md                               # This file
```

## ? Features

- ? Fully working, runnable examples
- ? Comprehensive error handling and helpful messages
- ? Interactive menu-driven interface
- ? Detailed logging and output
- ? Best practices demonstrated
- ? Exam-focused scenarios
- ? Complete setup instructions
- ? Works with both local development and Azure environments

## ?? Contributing

This is a learning module for AZ-204 certification. Feel free to enhance it with additional scenarios or improvements!

## ?? License

This project is for educational purposes as part of AZ-204 certification preparation.

---

**Good luck with your AZ-204 exam! ??**
