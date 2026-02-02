# ?? AZ-204 Security Module - Setup Checklist

Use this checklist to track your setup progress. Check off items as you complete them!

## ?? Phase 1: Prerequisites (5 minutes)

- [ ] **Azure Subscription**
  - [ ] Have an active Azure subscription (free tier works)
  - [ ] Can access Azure Portal: https://portal.azure.com
  
- [ ] **Azure CLI**
  - [ ] Installed Azure CLI
  - [ ] Ran `az login` successfully
  - [ ] Can run `az account show` to see subscription details
  
- [ ] **.NET SDK**
  - [ ] .NET 9 SDK is installed
  - [ ] Can run `dotnet --version` and see version 9.x
  
- [ ] **Code Editor**
  - [ ] Have Visual Studio, VS Code, or another IDE installed

---

## ?? Phase 2: Azure Key Vault Setup (5 minutes)

- [ ] **Resource Group**
  ```powershell
  az group create --name rg-az204-security --location eastus
  ```
  - [ ] Created resource group `rg-az204-security`
  - [ ] Verified in Azure Portal

- [ ] **Key Vault**
  ```powershell
  az keyvault create --name kv-az204-demo-<unique> --resource-group rg-az204-security --location eastus
  ```
  - [ ] Created Key Vault with unique name
  - [ ] **Note your Key Vault name:** `____________________`
  - [ ] **Note your Key Vault URI:** `https://__________.vault.azure.net/`

- [ ] **Key Vault Permissions**
  ```powershell
  az keyvault set-policy --name kv-az204-demo-<unique> --upn <your-email> --secret-permissions all --key-permissions all --certificate-permissions all
  ```
  - [ ] Granted yourself all permissions
  - [ ] Can access Key Vault in Portal

---

## ?? Phase 3: App Registration (MSAL & Graph) (5 minutes)

- [ ] **Create App Registration**
  - [ ] Go to: Azure Portal > Azure AD > App Registrations > New registration
  - [ ] Name: `AZ204-Security-Demo`
  - [ ] Account type: "Single tenant"
  - [ ] Redirect URI: `http://localhost` (Public client/native)
  - [ ] Clicked "Register"

- [ ] **Note Configuration Values**
  - [ ] **Tenant ID:** `____________________________________`
  - [ ] **Client ID (Application ID):** `____________________________________`

- [ ] **Configure API Permissions**
  - [ ] Added Microsoft Graph > Delegated permissions
  - [ ] Added `User.Read` permission
  - [ ] Added `Mail.Read` permission
  - [ ] Added `Calendars.Read` permission
  - [ ] Clicked "Grant admin consent"
  - [ ] All permissions show green checkmark

- [ ] **Client Secret (Optional - for Confidential Client demo)**
  - [ ] Go to: Certificates & secrets
  - [ ] Created new client secret
  - [ ] **Client Secret Value (SAVE NOW!):** `____________________________________`
  - [ ] ?? Secret saved somewhere secure (can't view again)

---

## ?? Phase 4: Storage Account (SAS demos) (5 minutes)

- [ ] **Create Storage Account**
  ```powershell
  az storage account create --name staz204demo<unique> --resource-group rg-az204-security --location eastus --sku Standard_LRS
  ```
  - [ ] Created storage account with unique name
  - [ ] **Storage Account Name:** `____________________`

- [ ] **Get Storage Account Key**
  ```powershell
  az storage account keys list --account-name staz204demo<unique> --resource-group rg-az204-security --query "[0].value" -o tsv
  ```
  - [ ] Copied storage account key
  - [ ] **Storage Account Key:** `____________________________________`

---

## ?? Phase 5: Application Configuration (5 minutes)

### Option A: User Secrets (Recommended)

- [ ] **Initialize User Secrets**
  ```powershell
  cd Azure-204-Module3-Security
  dotnet user-secrets init
  ```

- [ ] **Set Configuration Values**
  ```powershell
  dotnet user-secrets set "Azure:TenantId" "<your-tenant-id>"
  dotnet user-secrets set "Azure:ClientId" "<your-client-id>"
  dotnet user-secrets set "Azure:ClientSecret" "<your-client-secret>"
  dotnet user-secrets set "Azure:SubscriptionId" "<your-subscription-id>"
  dotnet user-secrets set "KeyVault:VaultUri" "https://kv-az204-demo-<unique>.vault.azure.net/"
  ```

- [ ] **Set Environment Variables**
  ```powershell
  $env:AZURE_STORAGE_ACCOUNT_NAME = "staz204demo<unique>"
  $env:AZURE_STORAGE_ACCOUNT_KEY = "<your-storage-key>"
  ```

- [ ] **Verify Configuration**
  ```powershell
  dotnet user-secrets list
  echo $env:AZURE_STORAGE_ACCOUNT_NAME
  ```

### Option B: appsettings.json (Alternative)

- [ ] Edited `appsettings.json` with all values
- [ ] Set environment variables for storage
- [ ] ?? Added `appsettings.json` to `.gitignore` if using source control

---

## ?? Phase 6: Build and Run (2 minutes)

- [ ] **Restore Packages**
  ```powershell
  dotnet restore
  ```

- [ ] **Build Project**
  ```powershell
  dotnet build
  ```
  - [ ] Build succeeded with no errors

- [ ] **Run Application**
  ```powershell
  dotnet run
  ```
  - [ ] Application started
  - [ ] See interactive menu
  - [ ] Menu displays correctly

---

## ? Phase 7: Test Each Demo (15 minutes)

- [ ] **[1] Key Vault Demo**
  - [ ] Can create secrets
  - [ ] Can create keys
  - [ ] Can create certificates
  - [ ] No permission errors

- [ ] **[2] MSAL Authentication Demo**
  - [ ] Public client authentication works
  - [ ] Browser opens for sign-in
  - [ ] Token acquired successfully
  - [ ] (Optional) Confidential client works if secret configured

- [ ] **[3] Microsoft Graph API Demo**
  - [ ] Can retrieve user profile
  - [ ] Can list emails (if you have any)
  - [ ] Can list calendar events
  - [ ] No permission errors

- [ ] **[4] Managed Identity Demo**
  - [ ] Shows authentication methods
  - [ ] DefaultAzureCredential works
  - [ ] Can list resource groups
  - [ ] Informational content displays

- [ ] **[5] Shared Access Signatures Demo**
  - [ ] Can create blob SAS tokens
  - [ ] Can test SAS permissions
  - [ ] Account SAS works
  - [ ] Stored access policy works

- [ ] **[6] Run All Demos**
  - [ ] All demos execute sequentially
  - [ ] No critical errors
  - [ ] Complete successfully

---

## ?? Phase 8: Learning Resources (Ongoing)

- [ ] Read the README.md file
- [ ] Review QUICKSTART.md
- [ ] Explore code in Services/ folder
- [ ] Check out [7] Setup Guide in menu
- [ ] Review [8] AZ-204 Exam Tips in menu
- [ ] Bookmarked Microsoft Learn resources
- [ ] Tried Microsoft Graph Explorer

---

## ?? Troubleshooting Checklist

If something doesn't work, check these:

### Key Vault Issues
- [ ] Key Vault name is globally unique
- [ ] Permissions are granted to your user account
- [ ] Logged in with correct Azure account (`az account show`)
- [ ] Key Vault URI is correct in configuration

### MSAL/Graph Issues
- [ ] Tenant ID and Client ID are correct
- [ ] Redirect URI is exactly `http://localhost`
- [ ] API permissions are granted AND admin consent given
- [ ] Using the correct Azure AD account

### Storage Issues
- [ ] Environment variables are set in current PowerShell session
- [ ] Storage account key is correct (not connection string)
- [ ] Storage account name is all lowercase, no special characters
- [ ] Account key hasn't been regenerated

### General Issues
- [ ] Running `az login` and authenticated
- [ ] .NET 9 SDK is installed
- [ ] All NuGet packages restored
- [ ] Build succeeds without errors
- [ ] Using PowerShell (not Command Prompt) for commands

---

## ?? Cost Tracking

Track your Azure usage to avoid surprises:

- [ ] Set up Azure cost alerts
- [ ] Understand pricing for:
  - [ ] Key Vault: ~$0.03 per 10,000 operations
  - [ ] Storage: ~$0.02 per GB
  - [ ] App Registration: Free
- [ ] Plan to cleanup resources when done

---

## ?? Cleanup Checklist (When Finished)

- [ ] Decided to cleanup resources
- [ ] Backed up any important data
- [ ] Run cleanup command:
  ```powershell
  az group delete --name rg-az204-security --yes --no-wait
  ```
- [ ] Deleted app registration:
  ```powershell
  az ad app delete --id <client-id>
  ```
- [ ] (Optional) Purged Key Vault if reusing name:
  ```powershell
  az keyvault purge --name kv-az204-demo-<unique>
  ```
- [ ] Verified in Azure Portal that resources are deleted

---

## ?? Learning Goals Checklist

By completing this module, you should be able to:

- [ ] Explain the difference between secrets, keys, and certificates
- [ ] Create and manage Key Vault resources
- [ ] Implement authentication using MSAL
- [ ] Understand OAuth 2.0 flows (Auth Code, Client Credentials, Device Code)
- [ ] Call Microsoft Graph API endpoints
- [ ] Implement Managed Identities
- [ ] Use DefaultAzureCredential for authentication
- [ ] Generate and use SAS tokens
- [ ] Implement stored access policies
- [ ] Understand when to use each authentication method
- [ ] Apply security best practices in Azure

---

## ?? Notes Section

Use this space for your own notes:

```
___________________________________________________________________________

___________________________________________________________________________

___________________________________________________________________________

___________________________________________________________________________

___________________________________________________________________________

___________________________________________________________________________
```

---

## ? Completion

- [ ] **All setup phases completed**
- [ ] **All demos tested successfully**
- [ ] **Understand key concepts**
- [ ] **Ready for AZ-204 Security exam questions**

**Congratulations! You're ready to master Azure Security! ??**

---

**Last Updated:** [Date you completed setup]  
**Next Review:** [Schedule next practice session]
