# ?? Azure Deployment Guide - AZ-204

Complete guide to deploying Azure resources for hands-on learning.

---

## ?? Table of Contents

1. [Prerequisites](#-prerequisites)
2. [Quick Start](#-quick-start)
3. [Deployment Scripts](#-deployment-scripts)
4. [Learning Paths](#-learning-paths)
5. [Troubleshooting](#-troubleshooting)
6. [Cleanup](#-cleanup)

---

## ? Prerequisites

### Required Tools

```powershell
# 1. Install Azure CLI
winget install -e --id Microsoft.AzureCLI

# 2. Close and reopen PowerShell (IMPORTANT!)

# 3. Verify installation
az --version

# 4. Login to Azure
az login
```

### Quick Check

Run the verification script:
```powershell
.\install-prerequisites.ps1
```

This checks if you have:
- ? Azure CLI installed
- ? Active Azure subscription
- ? Proper permissions

---

## ?? Quick Start

### Minimum Deployment (Recommended First Time)

```powershell
# 1. Setup (Required)
.\deploy-0-setup.ps1

# 2. Deploy App Service
.\deploy-1-appservice.ps1

# 3. Deploy Functions
.\deploy-2-functions.ps1

# 4. Generate Config
.\deploy-6-output-config.ps1
```

**?? Time:** 15 minutes  
**?? Cost:** ~$2/day  
**? Gets you:** Web apps, Functions, and working configuration

---

## ?? Deployment Scripts

### Overview

| # | Script | What It Deploys | Time | Daily Cost |
|---|--------|----------------|------|------------|
| **0** | `deploy-0-setup.ps1` | Resource Group + generates names | 2 min | Free |
| **1** | `deploy-1-appservice.ps1` | Web App + deployment slots | 5 min | $2 |
| **2** | `deploy-2-functions.ps1` | Function Apps (Consumption + Premium) | 5 min | $0-5 |
| **3** | `deploy-3-container-instances.ps1` | Container instance | 2 min | $1 |
| **4** | `deploy-4-aks.ps1` | Kubernetes cluster ?? | 20 min | **$150/mo** |
| **5** | `deploy-5-supporting-services.ps1` | Redis, CosmosDB, Service Bus | 15 min | $2 |
| **6** | `deploy-6-output-config.ps1` | Generates `appsettings.json` | 2 min | Free |

---

### Script Details

#### 0?? Setup (REQUIRED - Run First!)

```powershell
.\deploy-0-setup.ps1
```

**Creates:**
- Resource Group: `rg-az204-compute`
- Generates unique resource names
- Saves to `deployment-config.json`

**Output:**
```json
{
  "ResourceGroup": "rg-az204-compute",
  "RandomSuffix": "7823",
  "ResourceNames": {
    "WebApp": "webapp-az204-7823",
    "FunctionApp": "func-az204-7823"
  }
}
```

---

#### 1?? App Service

```powershell
.\deploy-1-appservice.ps1
```

**Deploys:**
- ? App Service Plan (P0v3)
- ? Web App (.NET 8)
- ? Deployment Slots (staging, production)
- ? Auto-scaling rules
- ? Application Insights

**Learning:**
- App Service tiers
- Deployment slots for zero-downtime
- Auto-scaling configuration
- Monitoring setup

---

#### 2?? Functions

```powershell
.\deploy-2-functions.ps1
```

**Deploys:**
- ? Storage Account (required)
- ? Function App (Consumption)
- ? Function App (Premium)
- ? Application Insights integration
- ? Managed Identity

**Learning:**
- Consumption vs Premium plans
- Cold start differences
- Managed Identity for security
- Storage requirements

---

#### 3?? Container Instances

```powershell
.\deploy-3-container-instances.ps1
```

**Deploys:**
- ? Container instance with public IP
- ? Sample "Hello World" container
- ? Fast startup demo

**Learning:**
- Container basics
- Public vs private endpoints
- Quick deployment scenarios

---

#### 4?? AKS (Optional - Expensive!)

```powershell
.\deploy-4-aks.ps1
```

?? **WARNING:** This is expensive (~$150/month)

**Deploys:**
- ? AKS Cluster (3 nodes)
- ? System node pool
- ? Azure CNI networking
- ? Managed Identity
- ? Auto-scaling enabled

**Learning:**
- Kubernetes fundamentals
- Node pools
- Cluster autoscaling
- Production-ready configuration

?? **Tip:** Skip this unless you specifically need Kubernetes!

---

#### 5?? Supporting Services (Optional)

```powershell
.\deploy-5-supporting-services.ps1
```

**Deploys:**
- ? Redis Cache (Basic tier)
- ? Azure Service Bus (Standard)
- ? Cosmos DB (Serverless)
- ? Storage Account (queues, blobs)

**Learning:**
- Distributed caching
- Message queuing
- NoSQL databases
- Storage patterns

---

#### 6?? Output Config (REQUIRED - Run Last!)

```powershell
.\deploy-6-output-config.ps1
```

**Generates:**
- Complete `appsettings.json`
- Connection strings
- Resource IDs
- Ready-to-use configuration

**Copy output to:** `Azure- 204/appsettings.json`

---

## ?? Learning Paths

### Path 1: Beginner (Day 1)
**Focus:** Web apps and basics

```powershell
.\deploy-0-setup.ps1
.\deploy-1-appservice.ps1
.\deploy-6-output-config.ps1
```

**Learn:**
- Resource Groups
- App Service Plans
- Deployment slots
- Auto-scaling

**Time:** 10 minutes  
**Cost:** $2/day

---

### Path 2: Serverless (Day 2)
**Focus:** Functions and event-driven

```powershell
.\deploy-0-setup.ps1
.\deploy-2-functions.ps1
.\deploy-6-output-config.ps1
```

**Learn:**
- Consumption vs Premium
- Triggers and bindings
- Managed Identity
- Durable Functions

**Time:** 10 minutes  
**Cost:** $0-5/day

---

### Path 3: Containers (Day 3)
**Focus:** Container basics

```powershell
.\deploy-0-setup.ps1
.\deploy-3-container-instances.ps1
.\deploy-6-output-config.ps1
```

**Learn:**
- Container deployment
- Public endpoints
- Fast startup times
- When to use ACI

**Time:** 5 minutes  
**Cost:** $1/day

---

### Path 4: Full Stack (Week 1)
**Focus:** Complete environment

```powershell
# Run all scripts 0-6 (skip 4 if budget-conscious)
.\deploy-0-setup.ps1
.\deploy-1-appservice.ps1
.\deploy-2-functions.ps1
.\deploy-3-container-instances.ps1
.\deploy-5-supporting-services.ps1  # Optional
.\deploy-6-output-config.ps1
```

**Learn:**
- Complete Azure ecosystem
- Service integration
- Production patterns
- Real-world scenarios

**Time:** 30 minutes (without AKS)  
**Cost:** $5/day (without AKS)

---

## ?? How It Works

### Interactive Learning

Each script:
1. **?? Explains** what it will do
2. **?? Waits** for you to press Enter
3. **?? Shows** the actual Azure CLI command
4. **? Confirms** success or lets you continue on error

### Example Output:

```
???????????????????????????????????
 STEP: Create Web App
???????????????????????????????????

?? What this does:
Creates the actual web application
Runtime: .NET 8.0
URL: https://webapp-az204-7823.azurewebsites.net

Press Enter to execute...

??  Executing...
? Success! Web app created.
```

### Shared Configuration

- `deploy-0-setup.ps1` creates `deployment-config.json`
- All other scripts read from this file
- Ensures consistent naming across resources

---

## ? Troubleshooting

### "az is not recognized"

```powershell
# Install Azure CLI
winget install -e --id Microsoft.AzureCLI

# IMPORTANT: Close and reopen PowerShell!

# Verify
az --version
```

---

### "deployment-config.json not found"

```powershell
# You must run setup first:
.\deploy-0-setup.ps1
```

---

### Deployment Fails

**Script asks:** "Continue? (Y/N)"
- **N** = Stop and fix the issue
- **Y** = Skip that step and continue

**Common fixes:**
- Check if you're logged in: `az login`
- Verify subscription: `az account show`
- Check quota limits in Azure Portal

---

### Forgot Resource Names

```powershell
# Check config file:
Get-Content deployment-config.json

# Or list in Azure:
az resource list --resource-group rg-az204-compute --output table
```

---

### "Quota exceeded"

Some regions have limited capacity. Try:
```powershell
# Change location in deployment-config.json
# Common alternatives: eastus, westus2, westeurope
```

---

## ?? Cleanup

### Delete Everything

```powershell
az group delete --name rg-az204-compute --yes --no-wait
```

**This deletes:**
- ? ALL resources in the resource group
- ? No charges after deletion
- ? Takes 5-10 minutes

### Verify Deletion

```powershell
az group list --output table
```

---

## ?? Cost Management

### Monitor Costs

```powershell
# View costs in Azure Portal:
# Cost Management + Billing > Cost Analysis
```

### Save Money

1. **Deploy only what you need** - Skip AKS if not required
2. **Delete when done** - Run cleanup script daily
3. **Use Consumption plans** - Functions scale to zero
4. **Stop services** - Stop (don't delete) App Service when not in use

### Estimated Monthly Costs

| Configuration | Per Day | Per Month |
|--------------|---------|-----------|
| Minimum (App Service + Functions) | $2 | $60 |
| Core (+ Containers) | $3 | $90 |
| Full (+ Supporting) | $5 | $150 |
| Everything (+ AKS) | $10 | $300 |

---

## ?? Tips & Best Practices

### ? DO:
- Start with minimum deployment
- Read each step carefully
- Understand before pressing Enter
- Delete resources when not in use
- Use Cost Management to track spending

### ? DON'T:
- Rush through scripts
- Skip the setup script
- Leave AKS running overnight
- Forget to run output-config script
- Deploy to production subscription

---

## ?? Need Help?

### Alternative: PowerShell Az Module

If Azure CLI doesn't work:
```powershell
Install-Module -Name Az -Force
Connect-AzAccount
```

### Check Status

```powershell
# List all resources
az resource list --resource-group rg-az204-compute --output table

# Check specific service
az webapp show --name webapp-az204-XXXX --resource-group rg-az204-compute
```

### Azure Portal

Visit [portal.azure.com](https://portal.azure.com) to:
- See visual overview
- Check costs
- Monitor resources
- Troubleshoot issues

---

## ?? Next Steps

After deployment:

1. **Copy config**
   ```powershell
   # Output from deploy-6-output-config.ps1
   # Copy to: Azure- 204/appsettings.json
   ```

2. **Run the application**
   ```powershell
   cd "Azure- 204"
   dotnet run
   ```

3. **Try the demos**
   - Select option 1-4 from menu
   - Test each service
   - Learn by doing!

4. **Explore Azure Portal**
   - See what you created
   - Check monitoring
   - Review logs

---

## ?? Resources

- [Azure CLI Documentation](https://docs.microsoft.com/cli/azure/)
- [App Service Pricing](https://azure.microsoft.com/pricing/details/app-service/)
- [Functions Pricing](https://azure.microsoft.com/pricing/details/functions/)
- [AZ-204 Exam Guide](https://docs.microsoft.com/certifications/exams/az-204)

---

## ? You're Ready!

Start with:
```powershell
az login
.\deploy-0-setup.ps1
```

**Happy Learning! ??**
