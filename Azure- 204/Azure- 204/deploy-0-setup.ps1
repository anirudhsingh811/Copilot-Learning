# ============================================================
# SETUP: Initialize Variables and Resource Group
# Run this FIRST before deploying any services
# ============================================================

Write-Host @"
??????????????????????????????????????????????????????????????
?                                                            ?
?     AZ-204: Setup & Resource Group Creation               ?
?                                                            ?
??????????????????????????????????????????????????????????????
"@ -ForegroundColor Cyan

# ============================================================
# CONFIGURATION
# ============================================================

$script:resourceGroup = "rg-az204-compute"
$script:location = "eastus"
$script:subscriptionId = "c01b8f09-10c7-4d70-b719-05b1b231f9d3"
$script:randomSuffix = Get-Random -Maximum 9999

Write-Host "`n?? Random Suffix: $randomSuffix" -ForegroundColor Yellow
Write-Host "   This ensures your resource names are globally unique!" -ForegroundColor Gray

# ============================================================
# GENERATE RESOURCE NAMES
# ============================================================

$script:resourceNames = @{
    # App Service
    AppServicePlan = "plan-az204-prod"
    WebApp = "webapp-az204-$randomSuffix"
    AppInsights = "ai-az204-$randomSuffix"
    
    # Functions
    StorageAccount = "staz204$randomSuffix"
    FunctionApp = "func-az204-$randomSuffix"
    FunctionPlan = "plan-func-premium"
    FunctionAppPremium = "func-az204-premium-$randomSuffix"
    
    # Container Instances
    AciSimple = "aci-simple-$randomSuffix"
    AciDnsLabel = "aci-az204-$randomSuffix"
    
    # AKS
    AksCluster = "aks-az204-prod"
    
    # Supporting Services
    KeyVault = "kv-az204-$randomSuffix"
    Redis = "redis-az204-$randomSuffix"
    ServiceBus = "sb-az204-$randomSuffix"
    CosmosDb = "cosmos-az204-$randomSuffix"
}

# Save to file for other scripts to use
$config = @{
    ResourceGroup = $resourceGroup
    Location = $location
    SubscriptionId = $subscriptionId
    RandomSuffix = $randomSuffix
    ResourceNames = $resourceNames
}

$config | ConvertTo-Json -Depth 10 | Out-File "deployment-config.json" -Force

Write-Host "`n? Configuration saved to: deployment-config.json" -ForegroundColor Green

# ============================================================
# CREATE RESOURCE GROUP
# ============================================================

Write-Host "`n????????????????????????????????????????" -ForegroundColor Cyan
Write-Host "?? Creating Resource Group" -ForegroundColor Yellow
Write-Host "????????????????????????????????????????" -ForegroundColor Cyan

Write-Host "`n?? What this does:" -ForegroundColor Green
Write-Host "Resource Groups are logical containers for Azure resources." -ForegroundColor White
Write-Host "Name: $resourceGroup" -ForegroundColor White
Write-Host "Location: $location" -ForegroundColor White

Write-Host "`n?? Command:" -ForegroundColor Magenta
Write-Host "az group create --name $resourceGroup --location $location" -ForegroundColor Gray

Write-Host "`nPress Enter to execute..." -ForegroundColor Yellow
Read-Host

Write-Host "`n??  Executing..." -ForegroundColor Cyan
az group create --name $resourceGroup --location $location

if ($LASTEXITCODE -eq 0) {
    Write-Host "? Success! Resource group created." -ForegroundColor Green
} else {
    Write-Host "? Failed to create resource group." -ForegroundColor Red
    exit 1
}

# ============================================================
# SUMMARY
# ============================================================

Write-Host "`n`n??????????????????????????????????????????" -ForegroundColor Green
Write-Host "?     ? SETUP COMPLETE!                 ?" -ForegroundColor Green
Write-Host "??????????????????????????????????????????" -ForegroundColor Green

Write-Host "`n?? Resource Names (Suffix: $randomSuffix):" -ForegroundColor Cyan
Write-Host "   Web App:        $($resourceNames.WebApp)" -ForegroundColor White
Write-Host "   Function App:   $($resourceNames.FunctionApp)" -ForegroundColor White
Write-Host "   Storage:        $($resourceNames.StorageAccount)" -ForegroundColor White
Write-Host "   Key Vault:      $($resourceNames.KeyVault)" -ForegroundColor White

Write-Host "`n?? Next Steps:" -ForegroundColor Yellow
Write-Host "   Run these in order:" -ForegroundColor White
Write-Host "   1. .\deploy-1-appservice.ps1" -ForegroundColor Gray
Write-Host "   2. .\deploy-2-functions.ps1" -ForegroundColor Gray
Write-Host "   3. .\deploy-3-container-instances.ps1" -ForegroundColor Gray
Write-Host "   4. .\deploy-4-aks.ps1 (optional, takes 20 min)" -ForegroundColor Gray
Write-Host "   5. .\deploy-5-supporting-services.ps1 (optional)" -ForegroundColor Gray

Write-Host "`n?? TIP: Each script loads config from deployment-config.json" -ForegroundColor Yellow
