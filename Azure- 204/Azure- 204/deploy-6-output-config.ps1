# ============================================================
# DEPLOY 6: Output Configuration for appsettings.json
# Run this LAST to get all connection strings
# ============================================================

Write-Host @"
??????????????????????????????????????????????????????????????
?                                                            ?
?     OUTPUT: Configuration for appsettings.json            ?
?                                                            ?
??????????????????????????????????????????????????????????????
"@ -ForegroundColor Magenta

# Load configuration
if (-not (Test-Path "deployment-config.json")) {
    Write-Host "? Run .\deploy-0-setup.ps1 first!" -ForegroundColor Red
    exit 1
}

$config = Get-Content "deployment-config.json" | ConvertFrom-Json
$resourceGroup = $config.ResourceGroup
$location = $config.Location
$subscriptionId = $config.SubscriptionId
$randomSuffix = $config.RandomSuffix
$names = $config.ResourceNames

Write-Host "`n?? Fetching connection strings and keys from Azure..." -ForegroundColor Cyan
Write-Host "   This may take a minute..." -ForegroundColor Gray

# ============================================================
# FETCH CONNECTION STRINGS
# ============================================================

# Key Vault
$keyVaultUrl = "https://$($names.KeyVault).vault.azure.net/"

# Application Insights
$appInsightsConnectionString = az monitor app-insights component show `
    --app $names.AppInsights `
    --resource-group $resourceGroup `
    --query connectionString `
    --output tsv 2>$null

$appInsightsInstrumentationKey = az monitor app-insights component show `
    --app $names.AppInsights `
    --resource-group $resourceGroup `
    --query instrumentationKey `
    --output tsv 2>$null

# Storage Account
$storageConnectionString = az storage account show-connection-string `
    --name $names.StorageAccount `
    --resource-group $resourceGroup `
    --query connectionString `
    --output tsv 2>$null

# Redis (if deployed)
$redisConnectionString = az redis list-keys `
    --name $names.Redis `
    --resource-group $resourceGroup `
    --query primaryKey `
    --output tsv 2>$null

if ($redisConnectionString) {
    $redisConnectionString = "$($names.Redis).redis.cache.windows.net:6380,password=$redisConnectionString,ssl=True,abortConnect=False"
}

# Service Bus (if deployed)
$serviceBusConnectionString = az servicebus namespace authorization-rule keys list `
    --namespace-name $names.ServiceBus `
    --resource-group $resourceGroup `
    --name RootManageSharedAccessKey `
    --query primaryConnectionString `
    --output tsv 2>$null

# Cosmos DB (if deployed)
$cosmosConnectionString = az cosmosdb keys list `
    --name $names.CosmosDb `
    --resource-group $resourceGroup `
    --type connection-strings `
    --query "connectionStrings[0].connectionString" `
    --output tsv 2>$null

Write-Host "? Connection strings fetched!" -ForegroundColor Green

# ============================================================
# OUTPUT JSON
# ============================================================

Write-Host "`n`n========================================" -ForegroundColor Magenta
Write-Host "COPY THIS TO YOUR appsettings.json" -ForegroundColor Magenta
Write-Host "========================================`n" -ForegroundColor Magenta

$jsonOutput = @"
{
  "Azure": {
    "SubscriptionId": "$subscriptionId",
    "TenantId": "<your-tenant-id>",
    "ResourceGroup": "$resourceGroup",
    "Location": "$location"
  },
  "AppService": {
    "WebAppName": "$($names.WebApp)",
    "AppServicePlan": "$($names.AppServicePlan)",
    "DeploymentSlots": [ "staging", "production" ]
  },
  "Functions": {
    "FunctionAppName": "$($names.FunctionApp)",
    "StorageAccountName": "$($names.StorageAccount)",
    "HostingPlan": "Premium"
  },
  "ContainerInstances": {
    "ContainerGroupName": "$($names.AciSimple)",
    "Image": "mcr.microsoft.com/azuredocs/aci-helloworld"
  },
  "AKS": {
    "ClusterName": "$($names.AksCluster)",
    "KubernetesVersion": "1.28.0",
    "NodeCount": 3,
    "NodeVmSize": "Standard_DS2_v2"
  },
  "KeyVault": {
    "VaultUrl": "$keyVaultUrl",
    "UseKeyVaultReferences": true
  },
  "ApplicationInsights": {
    "ConnectionString": "$appInsightsConnectionString",
    "InstrumentationKey": "$appInsightsInstrumentationKey"
  },
  "Redis": {
    "ConnectionString": "$redisConnectionString",
    "InstanceName": "AZ204_"
  },
  "ServiceBus": {
    "ConnectionString": "$serviceBusConnectionString",
    "QueueName": "orders-queue",
    "TopicName": "order-events"
  },
  "CosmosDB": {
    "ConnectionString": "$cosmosConnectionString",
    "DatabaseName": "OrdersDB",
    "ContainerName": "Orders"
  },
  "Storage": {
    "ConnectionString": "$storageConnectionString",
    "BlobContainerName": "uploads",
    "QueueName": "processing-queue"
  }
}
"@

Write-Host $jsonOutput -ForegroundColor White

# Save to file
$jsonOutput | Out-File "appsettings-generated.json" -Force
Write-Host "`n? Also saved to: appsettings-generated.json" -ForegroundColor Green

# ============================================================
# SUMMARY
# ============================================================

Write-Host "`n`n??????????????????????????????????????????" -ForegroundColor Green
Write-Host "?     ?? ALL DEPLOYMENTS COMPLETE! ??   ?" -ForegroundColor Green
Write-Host "??????????????????????????????????????????" -ForegroundColor Green

Write-Host "`n?? Resource Summary (Suffix: $randomSuffix):" -ForegroundColor Cyan
Write-Host "   Resource Group: $resourceGroup" -ForegroundColor White
Write-Host "   App Service:    $($names.WebApp)" -ForegroundColor White
Write-Host "   Functions:      $($names.FunctionApp)" -ForegroundColor White
Write-Host "   Storage:        $($names.StorageAccount)" -ForegroundColor White
if ($names.KeyVault) { Write-Host "   Key Vault:      $($names.KeyVault)" -ForegroundColor White }
if ($names.Redis) { Write-Host "   Redis:          $($names.Redis)" -ForegroundColor White }
if ($names.ServiceBus) { Write-Host "   Service Bus:    $($names.ServiceBus)" -ForegroundColor White }
if ($names.CosmosDb) { Write-Host "   Cosmos DB:      $($names.CosmosDb)" -ForegroundColor White }
if ($names.AksCluster) { Write-Host "   AKS:            $($names.AksCluster)" -ForegroundColor White }

Write-Host "`n?? Your URLs:" -ForegroundColor Cyan
Write-Host "   Web App:        https://$($names.WebApp).azurewebsites.net" -ForegroundColor White
Write-Host "   Web App (Staging): https://$($names.WebApp)-staging.azurewebsites.net" -ForegroundColor White
Write-Host "   Container:      http://$($names.AciDnsLabel).eastus.azurecontainers.io" -ForegroundColor White
Write-Host "   Azure Portal:   https://portal.azure.com" -ForegroundColor White

Write-Host "`n?? Next Steps:" -ForegroundColor Yellow
Write-Host "   1. Copy the JSON above to your appsettings.json" -ForegroundColor White
Write-Host "   2. Replace <your-tenant-id> with your actual tenant:" -ForegroundColor White
Write-Host "      az account show --query tenantId --output tsv" -ForegroundColor Cyan
Write-Host "   3. Run your application:" -ForegroundColor White
Write-Host "      dotnet run" -ForegroundColor Cyan

Write-Host "`n?? Cost Reminder:" -ForegroundColor Red
Write-Host "   These resources cost money! Delete when done:" -ForegroundColor Yellow
Write-Host "   az group delete --name $resourceGroup --yes --no-wait" -ForegroundColor Cyan

Write-Host "`n?? Learning Complete!" -ForegroundColor Green
Write-Host "   You've deployed production-ready Azure infrastructure!" -ForegroundColor Gray
Write-Host "   Check the Azure Portal to see everything you created." -ForegroundColor Gray

Write-Host "`n========================================" -ForegroundColor Magenta
