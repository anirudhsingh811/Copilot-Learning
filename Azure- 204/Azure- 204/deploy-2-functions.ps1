# ============================================================
# DEPLOY 2: Azure Functions
# ============================================================

Write-Host @"
??????????????????????????????????????????????????????????????
?                                                            ?
?     DEPLOY 2: Azure Functions                             ?
?                                                            ?
??????????????????????????????????????????????????????????????
"@ -ForegroundColor Green

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
# INFO
# ============================================================

Write-Host "`n?? What we'll deploy:" -ForegroundColor Cyan
Write-Host "   ? Storage Account (required for Functions)" -ForegroundColor White
Write-Host "   ? Function App - Consumption Plan (serverless)" -ForegroundColor White
Write-Host "   ? Function App - Premium Plan (no cold start)" -ForegroundColor White
Write-Host "   ? Managed Identity (secure access)" -ForegroundColor White

Write-Host "`n??  Time: ~5 minutes" -ForegroundColor Yellow
Write-Host "?? Cost: Consumption ~$0, Premium ~$5/day" -ForegroundColor Yellow

Write-Host "`nReady? Press Enter..." -ForegroundColor Cyan
Read-Host

# ============================================================
# DEPLOYMENTS
# ============================================================

# Step 1: Storage Account
Invoke-Step `
    -Title "Create Storage Account" `
    -Description @"
Functions need storage for:
- Code and configuration
- Trigger/execution logs
- Durable Functions state
Name: $($names.StorageAccount) (must be globally unique)
"@ `
    -Command {
        az storage account create `
            --name $names.StorageAccount `
            --resource-group $resourceGroup `
            --location $location `
            --sku Standard_LRS
    }

# Step 2: Function App (Consumption)
Invoke-Step `
    -Title "Create Function App (Consumption Plan)" `
    -Description @"
Consumption Plan = TRUE SERVERLESS
? Pay per execution
? Auto-scales to zero
? Cold start (first request slow)
Perfect for: Webhooks, scheduled jobs, event processing
"@ `
    -Command {
        az functionapp create `
            --name $names.FunctionApp `
            --resource-group $resourceGroup `
            --storage-account $names.StorageAccount `
            --consumption-plan-location $location `
            --runtime dotnet-isolated `
            --runtime-version 8 `
            --functions-version 4
    }

# Step 3: Function Plan (Premium)
Invoke-Step `
    -Title "Create Premium Function Plan" `
    -Description @"
Premium Plan = NO COLD STARTS
? Always warm
? VNet integration
? Longer execution time (up to 60 min)
? More expensive (~$150/month)
Use when: Performance is critical
"@ `
    -Command {
        az functionapp plan create `
            --name $names.FunctionPlan `
            --resource-group $resourceGroup `
            --location $location `
            --sku EP1
    }

# Step 4: Function App (Premium)
Invoke-Step `
    -Title "Create Function App (Premium Plan)" `
    -Description @"
Same Function App, but on Premium plan
Compare performance later!
"@ `
    -Command {
        az functionapp create `
            --name $names.FunctionAppPremium `
            --resource-group $resourceGroup `
            --plan $names.FunctionPlan `
            --storage-account $names.StorageAccount `
            --runtime dotnet-isolated `
            --runtime-version 8 `
            --functions-version 4
    }

# Step 5: Managed Identity
Invoke-Step `
    -Title "Enable Managed Identity" `
    -Description @"
Managed Identity = NO PASSWORDS/CONNECTION STRINGS
Azure creates an identity for your app
Grant permissions to that identity
Code authenticates automatically
SECURITY BEST PRACTICE for AZ-204!
"@ `
    -Command {
        az functionapp identity assign `
            --name $names.FunctionApp `
            --resource-group $resourceGroup
    }

# ============================================================
# COMPLETION
# ============================================================

Write-Host "`n`n??????????????????????????????????????????" -ForegroundColor Green
Write-Host "?     ? FUNCTIONS DEPLOYED!             ?" -ForegroundColor Green
Write-Host "??????????????????????????????????????????" -ForegroundColor Green

Write-Host "`n?? Resources Created:" -ForegroundColor Cyan
Write-Host "   Storage:             $($names.StorageAccount)" -ForegroundColor White
Write-Host "   Function (Consumption): $($names.FunctionApp)" -ForegroundColor White
Write-Host "   Function (Premium):  $($names.FunctionAppPremium)" -ForegroundColor White

Write-Host "`n?? Compare the two!" -ForegroundColor Yellow
Write-Host "   Consumption: Cold start on first request" -ForegroundColor Gray
Write-Host "   Premium:     Always warm, no delay" -ForegroundColor Gray

Write-Host "`n?? Next:" -ForegroundColor Yellow
Write-Host "   .\deploy-3-container-instances.ps1" -ForegroundColor Cyan
