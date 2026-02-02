# ============================================================
# DEPLOY 5: Supporting Services - OPTIONAL
# Redis, CosmosDB, Service Bus, Key Vault
# ============================================================

Write-Host @"
??????????????????????????????????????????????????????????????
?                                                            ?
?     DEPLOY 5: Supporting Services (Optional)              ?
?                                                            ?
??????????????????????????????????????????????????????????????
"@ -ForegroundColor Cyan

Write-Host "`n?? These services add functionality but increase cost" -ForegroundColor Yellow
Write-Host "   Key Vault:    Secret management (~$0.03/day)" -ForegroundColor White
Write-Host "   Redis Cache:  In-memory cache (~$0.50/day)" -ForegroundColor White
Write-Host "   Service Bus:  Message queue (~$0.05/day)" -ForegroundColor White
Write-Host "   Cosmos DB:    NoSQL database (~$1/day)" -ForegroundColor White

$continue = Read-Host "`nDeploy supporting services? (Y/N)"
if ($continue -ne "Y") {
    Write-Host "`n??  Skipping supporting services" -ForegroundColor Cyan
    Write-Host "`n   Next: .\deploy-6-output-config.ps1" -ForegroundColor White
    exit 0
}

# Load configuration
if (-not (Test-Path "deployment-config.json")) {
    Write-Host "? Run .\deploy-0-setup.ps1 first!" -ForegroundColor Red
    exit 1
}

$config = Get-Content "deployment-config.json" | ConvertFrom-Json
$resourceGroup = $config.ResourceGroup
$location = $config.Location
$names = $config.ResourceNames

Write-Host "? Configuration loaded" -ForegroundColor Green

# Helper function
function Invoke-Step {
    param([string]$Title, [string]$Description, [scriptblock]$Command)
    Write-Host "`n????????????????????????????????????????" -ForegroundColor Cyan
    Write-Host "?? $Title" -ForegroundColor Yellow
    Write-Host "????????????????????????????????????????" -ForegroundColor Cyan
    Write-Host "`n$Description" -ForegroundColor White
    Write-Host "`nPress Enter..." -ForegroundColor Yellow
    Read-Host
    Write-Host "??  Executing..." -ForegroundColor Cyan
    try {
        & $Command
        if ($LASTEXITCODE -eq 0) { Write-Host "? Success!" -ForegroundColor Green }
        else { throw "Failed" }
    } catch {
        Write-Host "? Error: $_" -ForegroundColor Red
        if ((Read-Host "Continue? (Y/N)") -ne "Y") { exit 1 }
    }
}

# ============================================================
# DEPLOYMENTS
# ============================================================

Write-Host "`n??  Time: ~10-15 minutes" -ForegroundColor Yellow
Write-Host "?? Total cost: ~$2/day" -ForegroundColor Yellow

Write-Host "`nReady? Press Enter..." -ForegroundColor Cyan
Read-Host

# Key Vault
Invoke-Step `
    -Title "Create Azure Key Vault" `
    -Description @"
Key Vault = Secure secret storage
Store: Passwords, connection strings, certificates, API keys
NEVER store secrets in code or config files!
AZ-204 Best Practice!
"@ `
    -Command {
        az keyvault create `
            --name $names.KeyVault `
            --resource-group $resourceGroup `
            --location $location `
            --enable-rbac-authorization false
    }

# Redis Cache
Invoke-Step `
    -Title "Create Azure Cache for Redis" `
    -Description @"
Redis = In-memory cache
Super fast (microseconds)
Use for: Session state, frequently accessed data
Reduces database load
"@ `
    -Command {
        az redis create `
            --name $names.Redis `
            --resource-group $resourceGroup `
            --location $location `
            --sku Basic `
            --vm-size c0
    }

# Service Bus
Invoke-Step `
    -Title "Create Azure Service Bus" `
    -Description @"
Service Bus = Enterprise message queue
Reliable message delivery
Supports: Queues, Topics, Subscriptions
Use for: Decoupling services, async processing
"@ `
    -Command {
        az servicebus namespace create `
            --name $names.ServiceBus `
            --resource-group $resourceGroup `
            --location $location `
            --sku Standard
    }

# Service Bus Queue
Invoke-Step `
    -Title "Create Service Bus Queue" `
    -Description @"
Queue = First-In-First-Out message processing
Messages processed once
Use for: Order processing, background jobs
"@ `
    -Command {
        az servicebus queue create `
            --namespace-name $names.ServiceBus `
            --resource-group $resourceGroup `
            --name "orders-queue"
    }

# Cosmos DB
Invoke-Step `
    -Title "Create Azure Cosmos DB" `
    -Description @"
Cosmos DB = Globally distributed NoSQL database
Millisecond latency
Multi-region replication
Use for: Web apps, IoT, gaming
Takes ~5 minutes to create...
"@ `
    -Command {
        az cosmosdb create `
            --name $names.CosmosDb `
            --resource-group $resourceGroup `
            --locations regionName=$location failoverPriority=0 `
            --default-consistency-level Session
    }

# Cosmos DB Database
Invoke-Step `
    -Title "Create Cosmos DB Database" `
    -Description "Creating 'OrdersDB' database" `
    -Command {
        az cosmosdb sql database create `
            --account-name $names.CosmosDb `
            --resource-group $resourceGroup `
            --name "OrdersDB"
    }

# Cosmos DB Container
Invoke-Step `
    -Title "Create Cosmos DB Container" `
    -Description @"
Container = Where data is actually stored
Partition Key: /orderId (important for scaling!)
Throughput: 400 RU/s (Request Units)
"@ `
    -Command {
        az cosmosdb sql container create `
            --account-name $names.CosmosDb `
            --resource-group $resourceGroup `
            --database-name "OrdersDB" `
            --name "Orders" `
            --partition-key-path "/orderId" `
            --throughput 400
    }

# ============================================================
# COMPLETION
# ============================================================

Write-Host "`n`n??????????????????????????????????????????" -ForegroundColor Green
Write-Host "?     ? SUPPORTING SERVICES DEPLOYED!   ?" -ForegroundColor Green
Write-Host "??????????????????????????????????????????" -ForegroundColor Green

Write-Host "`n?? Resources Created:" -ForegroundColor Cyan
Write-Host "   Key Vault:    $($names.KeyVault)" -ForegroundColor White
Write-Host "   Redis Cache:  $($names.Redis)" -ForegroundColor White
Write-Host "   Service Bus:  $($names.ServiceBus)" -ForegroundColor White
Write-Host "   Cosmos DB:    $($names.CosmosDb)" -ForegroundColor White

Write-Host "`n?? Next:" -ForegroundColor Yellow
Write-Host "   .\deploy-6-output-config.ps1" -ForegroundColor Cyan
Write-Host "   (Generates appsettings.json with all connection strings)" -ForegroundColor Gray
