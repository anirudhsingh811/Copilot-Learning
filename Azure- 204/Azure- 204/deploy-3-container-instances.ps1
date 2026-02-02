# ============================================================
# DEPLOY 3: Azure Container Instances (ACI)
# ============================================================

Write-Host @"
??????????????????????????????????????????????????????????????
?                                                            ?
?     DEPLOY 3: Azure Container Instances                   ?
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
Write-Host "   ? Simple container (hello-world app)" -ForegroundColor White

Write-Host "`n?? ACI = Azure Container Instances" -ForegroundColor Yellow
Write-Host "   Run containers WITHOUT managing VMs" -ForegroundColor Gray
Write-Host "   Fast startup (seconds)" -ForegroundColor Gray
Write-Host "   Pay per second" -ForegroundColor Gray
Write-Host "   Perfect for: Batch jobs, testing, simple apps" -ForegroundColor Gray

Write-Host "`n??  Time: ~2 minutes" -ForegroundColor Yellow
Write-Host "?? Cost: ~$1/day" -ForegroundColor Yellow

Write-Host "`nReady? Press Enter..." -ForegroundColor Cyan
Read-Host

# ============================================================
# DEPLOYMENT
# ============================================================

Invoke-Step `
    -Title "Create Container Instance" `
    -Description @"
Running a simple hello-world container
Image: mcr.microsoft.com/azuredocs/aci-helloworld
Port: 80 (HTTP)
Your URL: http://$($names.AciDnsLabel).eastus.azurecontainers.io

This starts in seconds (vs. VMs that take minutes)
"@ `
    -Command {
        az container create `
            --name $names.AciSimple `
            --resource-group $resourceGroup `
            --image mcr.microsoft.com/azuredocs/aci-helloworld `
            --dns-name-label $names.AciDnsLabel `
            --ports 80
    }

# ============================================================
# COMPLETION
# ============================================================

Write-Host "`n`n??????????????????????????????????????????" -ForegroundColor Green
Write-Host "?     ? CONTAINER DEPLOYED!             ?" -ForegroundColor Green
Write-Host "??????????????????????????????????????????" -ForegroundColor Green

Write-Host "`n?? Resource Created:" -ForegroundColor Cyan
Write-Host "   Container: $($names.AciSimple)" -ForegroundColor White

Write-Host "`n?? Access your container:" -ForegroundColor Cyan
Write-Host "   http://$($names.AciDnsLabel).eastus.azurecontainers.io" -ForegroundColor White

Write-Host "`n?? Try these commands:" -ForegroundColor Yellow
Write-Host "   # View container logs" -ForegroundColor Gray
Write-Host "   az container logs --name $($names.AciSimple) --resource-group $resourceGroup" -ForegroundColor Cyan
Write-Host "`n   # Check status" -ForegroundColor Gray
Write-Host "   az container show --name $($names.AciSimple) --resource-group $resourceGroup" -ForegroundColor Cyan

Write-Host "`n?? Next (Optional):" -ForegroundColor Yellow
Write-Host "   .\deploy-4-aks.ps1 (WARNING: Takes 20 min, costs $150/month)" -ForegroundColor Gray
Write-Host "   .\deploy-5-supporting-services.ps1 (Redis, CosmosDB, etc.)" -ForegroundColor Gray
Write-Host "`n   Or skip to final step:" -ForegroundColor White
Write-Host "   .\deploy-6-output-config.ps1 (Get appsettings.json)" -ForegroundColor Cyan
