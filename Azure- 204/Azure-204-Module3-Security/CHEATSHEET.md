# ?? AZ-204 Security - Quick Reference Cheat Sheet

## ?? Quick Commands

### Run Application
```powershell
cd Azure-204-Module3-Security
dotnet run
```

### Azure Login
```powershell
az login
az account show
```

### User Secrets
```powershell
dotnet user-secrets init
dotnet user-secrets set "Azure:TenantId" "value"
dotnet user-secrets list
dotnet user-secrets clear
```

### Environment Variables
```powershell
# Set
$env:AZURE_STORAGE_ACCOUNT_NAME = "value"
$env:AZURE_STORAGE_ACCOUNT_KEY = "value"

# View
echo $env:AZURE_STORAGE_ACCOUNT_NAME
Get-ChildItem Env: | Where-Object {$_.Name -like "*AZURE*"}
```

---

## ?? Azure Resource Creation

### Resource Group
```powershell
az group create --name rg-az204-security --location eastus
```

### Key Vault
```powershell
# Create
az keyvault create \
  --name kv-az204-demo-<unique> \
  --resource-group rg-az204-security \
  --location eastus

# Grant permissions
az keyvault set-policy \
  --name kv-az204-demo-<unique> \
  --upn <your-email> \
  --secret-permissions all \
  --key-permissions all \
  --certificate-permissions all

# List Key Vaults
az keyvault list --resource-group rg-az204-security
```

### Storage Account
```powershell
# Create
az storage account create \
  --name staz204demo<unique> \
  --resource-group rg-az204-security \
  --location eastus \
  --sku Standard_LRS

# Get keys
az storage account keys list \
  --account-name staz204demo<unique> \
  --resource-group rg-az204-security
```

### App Registration (CLI)
```powershell
# Create
$appId = az ad app create \
  --display-name "AZ204-Security-Demo" \
  --sign-in-audience AzureADMyOrg \
  --public-client-redirect-uris "http://localhost" \
  --query appId -o tsv

# Add Graph permissions
az ad app permission add --id $appId \
  --api 00000003-0000-0000-c000-000000000000 \
  --api-permissions \
    e1fe6dd8-ba31-4d61-89e7-88639da4683d=Scope

# Grant consent
az ad app permission admin-consent --id $appId

# Delete
az ad app delete --id $appId
```

---

## ?? Configuration Values Needed

| Setting | Example | Where to Get |
|---------|---------|--------------|
| Tenant ID | `72f988bf-...` | Azure AD > Properties |
| Client ID | `a1b2c3d4-...` | App Registration > Overview |
| Client Secret | `xxx~yyy...` | App Registration > Certificates & secrets |
| Subscription ID | `12345678-...` | Subscriptions blade |
| Key Vault URI | `https://kv-name.vault.azure.net/` | Key Vault > Overview |
| Storage Name | `staz204demo123` | Storage Account name |
| Storage Key | `abc123...` | Storage > Access keys |

---

## ?? Demo Quick Access

| # | Demo | Setup Required | Time |
|---|------|----------------|------|
| 1 | Key Vault | Key Vault + Permissions | 5 min |
| 2 | MSAL Auth | App Registration | 5 min |
| 3 | Graph API | App Registration + Permissions | 5 min |
| 4 | Managed Identity | None (Educational) | 2 min |
| 5 | SAS Tokens | Storage Account | 5 min |
| 6 | All Demos | All above | 20 min |
| 7 | Setup Guide | None | Read |
| 8 | Exam Tips | None | Read |

---

## ?? Key Vault Operations (Code)

```csharp
// Initialize
var credential = new DefaultAzureCredential();
var secretClient = new SecretClient(new Uri(vaultUri), credential);

// Secrets
await secretClient.SetSecretAsync("secret-name", "value");
KeyVaultSecret secret = await secretClient.GetSecretAsync("secret-name");
await secretClient.DeleteSecretAsync("secret-name");

// Keys
var keyClient = new KeyClient(new Uri(vaultUri), credential);
KeyVaultKey key = await keyClient.CreateRsaKeyAsync("key-name");

// Certificates
var certClient = new CertificateClient(new Uri(vaultUri), credential);
await certClient.StartCreateCertificateAsync("cert-name", policy);
```

---

## ?? MSAL Authentication (Code)

```csharp
// Public Client (Interactive)
var app = PublicClientApplicationBuilder
    .Create(clientId)
    .WithAuthority($"https://login.microsoftonline.com/{tenantId}")
    .WithDefaultRedirectUri()
    .Build();

var result = await app
    .AcquireTokenInteractive(scopes)
    .ExecuteAsync();

// Confidential Client (Service)
var app = ConfidentialClientApplicationBuilder
    .Create(clientId)
    .WithClientSecret(clientSecret)
    .WithAuthority($"https://login.microsoftonline.com/{tenantId}")
    .Build();

var result = await app
    .AcquireTokenForClient(scopes)
    .ExecuteAsync();
```

---

## ?? Microsoft Graph (Code)

```csharp
// Initialize
var credential = new InteractiveBrowserCredential();
var graphClient = new GraphServiceClient(credential, scopes);

// Get user
var user = await graphClient.Me.GetAsync();

// Get emails
var messages = await graphClient.Me.Messages
    .GetAsync(config => {
        config.QueryParameters.Top = 10;
        config.QueryParameters.Select = new[] { "subject", "from" };
    });

// Get calendar
var events = await graphClient.Me.Calendar.Events.GetAsync();
```

---

## ?? Managed Identity (Code)

```csharp
// System-Assigned
var credential = new ManagedIdentityCredential();

// User-Assigned
var credential = new ManagedIdentityCredential(clientId);

// DefaultAzureCredential (Best Practice)
var credential = new DefaultAzureCredential();

// Use with any Azure SDK
var secretClient = new SecretClient(vaultUri, credential);
var blobClient = new BlobServiceClient(storageUri, credential);
```

---

## ?? SAS Tokens (Code)

```csharp
// Blob SAS
var sasBuilder = new BlobSasBuilder
{
    BlobContainerName = "container",
    BlobName = "blob.txt",
    Resource = "b",
    StartsOn = DateTimeOffset.UtcNow,
    ExpiresOn = DateTimeOffset.UtcNow.AddHours(1)
};
sasBuilder.SetPermissions(BlobSasPermissions.Read);

Uri sasUri = blobClient.GenerateSasUri(sasBuilder);

// Account SAS
var sasBuilder = new AccountSasBuilder
{
    Services = AccountSasServices.Blobs,
    ResourceTypes = AccountSasResourceTypes.Object,
    StartsOn = DateTimeOffset.UtcNow,
    ExpiresOn = DateTimeOffset.UtcNow.AddHours(2),
    Protocol = SasProtocol.Https
};
sasBuilder.SetPermissions(AccountSasPermissions.Read);

string sasToken = sasBuilder.ToSasQueryParameters(credential).ToString();
```

---

## ?? Troubleshooting

### Key Vault Issues
```powershell
# Check permissions
az keyvault show --name kv-name --query properties.accessPolicies

# Re-grant permissions
az keyvault set-policy --name kv-name --upn <email> --secret-permissions all
```

### MSAL Issues
- Verify Tenant ID and Client ID
- Check redirect URI: exactly `http://localhost`
- Ensure permissions granted + admin consent
- Clear token cache: Delete `%LOCALAPPDATA%\.IdentityService`

### Storage Issues
```powershell
# Verify environment variables
echo $env:AZURE_STORAGE_ACCOUNT_NAME
echo $env:AZURE_STORAGE_ACCOUNT_KEY

# Test connection
az storage container list --account-name <name> --account-key <key>
```

### Azure CLI Issues
```powershell
# Re-login
az logout
az login

# Check subscription
az account show
az account set --subscription "name"

# Update CLI
az upgrade
```

---

## ?? Exam Key Concepts

### Key Vault
- **Secrets** - Store connection strings, passwords
- **Keys** - Cryptographic keys for encryption
- **Certificates** - SSL/TLS certificates
- **Soft Delete** - 90-day recovery period
- **Access Policies** - Identity-based permissions
- **RBAC** - Role-based access control (alternative)

### Managed Identity
- **System-Assigned** - Tied to resource lifecycle
- **User-Assigned** - Standalone, reusable
- **Use Cases** - VM, App Service, Functions, Containers
- **No Credentials** - Azure manages automatically

### MSAL Flows
- **Authorization Code** - Web apps with backend
- **Client Credentials** - Service-to-service, daemons
- **Device Code** - CLI tools, IoT devices
- **Implicit** - Legacy, not recommended

### SAS Types
- **Service SAS** - Single resource (blob, file, queue, table)
- **Account SAS** - Multiple services, broader access
- **User Delegation** - Azure AD-based (most secure)
- **Stored Policy** - Revocable without key regeneration

### Graph API
- **Delegated** - User context, requires user consent
- **Application** - App context, admin consent only
- **Scopes** - Granular permissions (User.Read, Mail.Read)
- **Endpoints** - /me, /users, /groups, /mail, /calendar

---

## ?? Cost Monitoring

```powershell
# Set up cost alert
az consumption budget create \
  --amount 10 \
  --budget-name "AZ204-Security-Budget" \
  --category Cost \
  --time-grain Monthly \
  --time-period-start <start-date> \
  --time-period-end <end-date>

# View costs
az consumption usage list --start-date <date> --end-date <date>
```

---

## ?? Cleanup

```powershell
# Delete everything
az group delete --name rg-az204-security --yes --no-wait

# Delete app registration
az ad app delete --id <client-id>

# Purge Key Vault (to reuse name)
az keyvault purge --name kv-az204-demo-<unique>

# List soft-deleted Key Vaults
az keyvault list-deleted
```

---

## ?? Quick Links

- **Azure Portal:** https://portal.azure.com
- **Graph Explorer:** https://developer.microsoft.com/graph/graph-explorer
- **Microsoft Learn:** https://learn.microsoft.com/training/paths/az-204-implement-authentication-authorization/
- **Azure CLI Docs:** https://learn.microsoft.com/cli/azure/
- **MSAL Docs:** https://learn.microsoft.com/azure/active-directory/develop/msal-overview

---

## ?? Important Azure CLI Query Examples

```powershell
# Get Tenant ID
az account show --query tenantId -o tsv

# Get Subscription ID
az account show --query id -o tsv

# Get User Object ID
az ad signed-in-user show --query id -o tsv

# Get User Principal Name
az ad signed-in-user show --query userPrincipalName -o tsv

# List Resource Groups
az group list --query "[].{Name:name, Location:location}" -o table

# List Key Vaults
az keyvault list --query "[].{Name:name, Location:location}" -o table

# List Storage Accounts
az storage account list --query "[].{Name:name, Location:location}" -o table

# List App Registrations
az ad app list --query "[].{Name:displayName, AppId:appId}" -o table
```

---

## ?? Exam Day Tips

- ? Know when to use Managed Identity vs Service Principal
- ? Understand delegated vs application permissions
- ? Remember SAS token expiration and permissions
- ? Know Key Vault soft delete behavior
- ? Understand OAuth 2.0 flow selection criteria
- ? Remember DefaultAzureCredential authentication chain order
- ? Know how to revoke SAS using stored policies
- ? Understand when certificates vs secrets vs keys

---

**Print this and keep it handy! ??**
