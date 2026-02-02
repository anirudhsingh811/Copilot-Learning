# ============================================================
# DEPLOY 4: Azure Kubernetes Service (AKS) - OPTIONAL
# ============================================================

Write-Host @"
??????????????????????????????????????????????????????????????
?                                                            ?
?     DEPLOY 4: Azure Kubernetes Service (AKS)              ?
?                                                            ?
??????????????????????????????????????????????????????????????
"@ -ForegroundColor Green

Write-Host "`n??  WARNING!" -ForegroundColor Red
Write-Host "   Time: 15-20 minutes" -ForegroundColor Yellow
Write-Host "   Cost: ~$150/month (3 nodes)" -ForegroundColor Yellow
Write-Host "   This is EXPENSIVE - only deploy if needed for learning!" -ForegroundColor Yellow

$continue = Read-Host "`nDeploy AKS? (Y/N)"
if ($continue -ne "Y") {
    Write-Host "`n??  Skipping AKS deployment" -ForegroundColor Cyan
    Write-Host "   Smart choice! You can learn AKS concepts without spending money." -ForegroundColor Gray
    Write-Host "`n   Next: .\deploy-5-supporting-services.ps1" -ForegroundColor White
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
    Write-Host "??  Executing (this takes ~15-20 min)..." -ForegroundColor Cyan
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
Write-Host "   ? AKS Cluster (3 nodes, auto-scaling 2-5)" -ForegroundColor White
Write-Host "   ? Azure CNI networking" -ForegroundColor White
Write-Host "   ? Monitoring enabled" -ForegroundColor White
Write-Host "   ? Availability Zones" -ForegroundColor White

Write-Host "`n?? Kubernetes = Container orchestration platform" -ForegroundColor Yellow
Write-Host "   Manage 100s/1000s of containers" -ForegroundColor Gray
Write-Host "   Auto-healing, auto-scaling, load balancing" -ForegroundColor Gray
Write-Host "   Industry standard for microservices" -ForegroundColor Gray

Write-Host "`n? Go grab a coffee! This takes 15-20 minutes..." -ForegroundColor Yellow

Write-Host "`nReady? Press Enter..." -ForegroundColor Cyan
Read-Host

# ============================================================
# DEPLOYMENT
# ============================================================

Invoke-Step `
    -Title "Create AKS Cluster" `
    -Description @"
Creating production-ready Kubernetes cluster:
- 3 nodes (Standard_DS2_v2)
- Auto-scaling: 2-5 nodes
- Azure CNI networking
- Azure Monitor integration
- Managed Identity
- Availability Zones (1, 2, 3)

??  THIS WILL TAKE 15-20 MINUTES!
Go do something else while this runs...
"@ `
    -Command {
        az aks create `
            --name $names.AksCluster `
            --resource-group $resourceGroup `
            --location $location `
            --node-count 3 `
            --node-vm-size Standard_DS2_v2 `
            --enable-managed-identity `
            --enable-addons monitoring `
            --enable-cluster-autoscaler `
            --min-count 2 `
            --max-count 5 `
            --network-plugin azure `
            --network-policy azure `
            --kubernetes-version 1.28.0 `
            --zones 1 2 3
    }

# Get credentials
Invoke-Step `
    -Title "Get AKS Credentials" `
    -Description @"
Downloading kubectl configuration
This lets you run: kubectl get nodes
"@ `
    -Command {
        az aks get-credentials `
            --name $names.AksCluster `
            --resource-group $resourceGroup `
            --overwrite-existing
    }

# ============================================================
# COMPLETION
# ============================================================

Write-Host "`n`n??????????????????????????????????????????" -ForegroundColor Green
Write-Host "?     ? AKS DEPLOYED!                   ?" -ForegroundColor Green
Write-Host "??????????????????????????????????????????" -ForegroundColor Green

Write-Host "`n?? Resource Created:" -ForegroundColor Cyan
Write-Host "   AKS Cluster: $($names.AksCluster)" -ForegroundColor White
Write-Host "   Nodes: 3 (auto-scales 2-5)" -ForegroundColor White

Write-Host "`n?? Try these commands:" -ForegroundColor Yellow
Write-Host "   kubectl get nodes" -ForegroundColor Cyan
Write-Host "   kubectl get namespaces" -ForegroundColor Cyan
Write-Host "   kubectl get pods --all-namespaces" -ForegroundColor Cyan

Write-Host "`n?? Next:" -ForegroundColor Yellow
Write-Host "   .\deploy-5-supporting-services.ps1 (Optional)" -ForegroundColor Cyan
Write-Host "   .\deploy-6-output-config.ps1 (Get appsettings.json)" -ForegroundColor Cyan

Write-Host "`n?? REMINDER: This costs ~$150/month!" -ForegroundColor Red
Write-Host "   Delete when done: az group delete --name $resourceGroup" -ForegroundColor Yellow
