# ============================================================
# DEPLOY 1: Azure App Service
# ============================================================

Write-Host @"
??????????????????????????????????????????????????????????????
?                                                            ?
?     DEPLOY 1: Azure App Service                           ?
?                                                            ?
??????????????????????????????????????????????????????????????
"@ -ForegroundColor Green

# ============================================================
# LOAD CONFIGURATION
# ============================================================

if (-not (Test-Path "deployment-config.json")) {
    Write-Host "? Error: deployment-config.json not found!" -ForegroundColor Red
    Write-Host "   Run .\deploy-0-setup.ps1 first!" -ForegroundColor Yellow
    exit 1
}

$config = Get-Content "deployment-config.json" | ConvertFrom-Json
$resourceGroup = $config.ResourceGroup
$location = $config.Location
$names = $config.ResourceNames

Write-Host "? Configuration loaded" -ForegroundColor Green
Write-Host "   Resource Group: $resourceGroup" -ForegroundColor Gray
Write-Host "   Web App: $($names.WebApp)" -ForegroundColor Gray

# Helper function
function Invoke-Step {
    param([string]$Title, [string]$Description, [scriptblock]$Command)
    
    Write-Host "`n????????????????????????????????????????" -ForegroundColor Cyan
    Write-Host "?? STEP: $Title" -ForegroundColor Yellow
    Write-Host "????????????????????????????????????????" -ForegroundColor Cyan
    Write-Host "`n?? $Description" -ForegroundColor White
    Write-Host "`n?? Command:" -ForegroundColor Magenta
    Write-Host $Command.ToString() -ForegroundColor Gray
    Write-Host "`nPress Enter to execute..." -ForegroundColor Yellow
    Read-Host
    Write-Host "??  Executing..." -ForegroundColor Cyan
    
    try {
        & $Command
        if ($LASTEXITCODE -eq 0) {
            Write-Host "? Success!" -ForegroundColor Green
        } else {
            throw "Command failed with exit code $LASTEXITCODE"
        }
    } catch {
        Write-Host "? Error: $_" -ForegroundColor Red
        $continue = Read-Host "Continue anyway? (Y/N)"
        if ($continue -ne "Y") { exit 1 }
    }
}

# ============================================================
# DEPLOYMENTS
# ============================================================

Write-Host "`n?? What we'll deploy:" -ForegroundColor Cyan
Write-Host "   ? App Service Plan (compute resources)" -ForegroundColor White
Write-Host "   ? Web App (your application)" -ForegroundColor White
Write-Host "   ? Deployment Slot (staging)" -ForegroundColor White
Write-Host "   ? Auto-scaling rules" -ForegroundColor White
Write-Host "   ? Application Insights (monitoring)" -ForegroundColor White

Write-Host "`n??  Time: ~5-7 minutes" -ForegroundColor Yellow
Write-Host "?? Cost: ~$2.30/day (S1 tier)" -ForegroundColor Yellow

Write-Host "`nReady? Press Enter..." -ForegroundColor Cyan
Read-Host

# Step 1: Create App Service Plan
Invoke-Step `
    -Title "Create App Service Plan" `
    -Description @"
App Service Plan = Compute resources (CPU/RAM/Disk)
SKU: S1 (Standard - $70/month)
OS: Linux
Can host multiple web apps on this plan
"@ `
    -Command {
        az appservice plan create `
            --name $names.AppServicePlan `
            --resource-group $resourceGroup `
            --sku S1 `
            --is-linux
    }

# Step 2: Create Web App
Invoke-Step `
    -Title "Create Web App" `
    -Description @"
Your actual web application
Runtime: .NET 8.0
URL: https://$($names.WebApp).azurewebsites.net
"@ `
    -Command {
        az webapp create `
            --name $names.WebApp `
            --resource-group $resourceGroup `
            --plan $names.AppServicePlan `
            --runtime 'DOTNET|8.0'
    }

# Step 3: Create Deployment Slot
Invoke-Step `
    -Title "Create Deployment Slot (Staging)" `
    -Description @"
Deployment Slots = Zero-downtime deployments
Deploy to staging ? Test ? Swap to production
Staging URL: https://$($names.WebApp)-staging.azurewebsites.net
"@ `
    -Command {
        az webapp deployment slot create `
            --name $names.WebApp `
            --resource-group $resourceGroup `
            --slot staging
    }

# Step 4: Configure Auto-scaling
Invoke-Step `
    -Title "Configure Auto-scaling" `
    -Description @"
Auto-scale = Automatically add/remove instances
Min: 1, Max: 5, Default: 2
Saves money when traffic is low
"@ `
    -Command {
        az monitor autoscale create `
            --resource-group $resourceGroup `
            --resource $names.AppServicePlan `
            --resource-type "Microsoft.Web/serverfarms" `
            --name "autoscale-plan" `
            --min-count 1 `
            --max-count 5 `
            --count 2
    }

# Step 5: Add CPU-based Scaling Rule
Invoke-Step `
    -Title "Add CPU-based Scaling Rule" `
    -Description @"
Rule: If CPU > 70% for 5 min ? Add 1 instance
Scale OUT = Add more servers (NOT scale UP = bigger server)
Important for AZ-204 exam!
"@ `
    -Command {
        az monitor autoscale rule create `
            --resource-group $resourceGroup `
            --autoscale-name "autoscale-plan" `
            --condition "CpuPercentage > 70 avg 5m" `
            --scale out 1
    }

# Step 6: Enable Application Insights
Invoke-Step `
    -Title "Enable Application Insights" `
    -Description @"
Application Insights = Monitoring & Diagnostics
Tracks: Requests, exceptions, dependencies, custom metrics
Essential for production!
"@ `
    -Command {
        az monitor app-insights component create `
            --app $names.AppInsights `
            --location $location `
            --resource-group $resourceGroup `
            --application-type web
    }

# ============================================================
# COMPLETION
# ============================================================

Write-Host "`n`n??????????????????????????????????????????" -ForegroundColor Green
Write-Host "?     ? APP SERVICE DEPLOYED!           ?" -ForegroundColor Green
Write-Host "??????????????????????????????????????????" -ForegroundColor Green

Write-Host "`n?? Resources Created:" -ForegroundColor Cyan
Write-Host "   App Service Plan: $($names.AppServicePlan)" -ForegroundColor White
Write-Host "   Web App:          $($names.WebApp)" -ForegroundColor White
Write-Host "   Deployment Slot:  staging" -ForegroundColor White
Write-Host "   App Insights:     $($names.AppInsights)" -ForegroundColor White

Write-Host "`n?? Your URLs:" -ForegroundColor Cyan
Write-Host "   Production: https://$($names.WebApp).azurewebsites.net" -ForegroundColor White
Write-Host "   Staging:    https://$($names.WebApp)-staging.azurewebsites.net" -ForegroundColor White

Write-Host "`n?? Next Steps:" -ForegroundColor Yellow
Write-Host "   1. Deploy your app: az webapp deploy ..." -ForegroundColor Gray
Write-Host "   2. Test in staging slot" -ForegroundColor Gray
Write-Host "   3. Swap to production" -ForegroundColor Gray
Write-Host "`n   OR run next deployment:" -ForegroundColor White
Write-Host "   .\deploy-2-functions.ps1" -ForegroundColor Cyan

Write-Host "`n?? View in Azure Portal:" -ForegroundColor Yellow
Write-Host "   https://portal.azure.com/#@/resource/subscriptions/$($config.SubscriptionId)/resourceGroups/$resourceGroup/overview" -ForegroundColor Gray
