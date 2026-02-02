# Complete deployment script for AZ-204 Simple Web App

param(
    [string]$ResourceGroup = "rg-az204-compute",
    [string]$Location = "eastus",
    [string]$AppServicePlan = "plan-az204-simple",
    [string]$WebAppName = "webapp-az204-9199",
    [string]$KeyVaultName = "kv-az204-9199"
)

Write-Host "🚀 Starting deployment..." -ForegroundColor Cyan
Write-Host "Resource Group: $ResourceGroup" -ForegroundColor Gray
Write-Host "Web App Name: $WebAppName" -ForegroundColor Gray

# Enable Managed Identity
Write-Host "`n🔑 Enabling Managed Identity..." -ForegroundColor Yellow
az webapp identity assign `
  --name $WebAppName `
  --resource-group $ResourceGroup `
  --output none

# Wait a bit for identity to propagate
Start-Sleep -Seconds 5

# Grant Key Vault access
Write-Host "`n🔐 Granting Key Vault access..." -ForegroundColor Yellow
$principalId = az webapp identity show `
  --name $WebAppName `
  --resource-group $ResourceGroup `
  --query principalId -o tsv

$kvId = az keyvault show --name $KeyVaultName --query id -o tsv

az role assignment create `
  --role "Key Vault Secrets User" `
  --assignee $principalId `
  --scope $kvId `
  --output none

# Configure settings
Write-Host "`n⚙️  Configuring App Settings..." -ForegroundColor Yellow
az webapp config appsettings set `
  --name $WebAppName `
  --resource-group $ResourceGroup `
  --settings `
    "KeyVault:Uri=https://$KeyVaultName.vault.azure.net/" `
    "ASPNETCORE_ENVIRONMENT=Production" `
  --output none

# Enable HTTPS only
az webapp update `
  --name $WebAppName `
  --resource-group $ResourceGroup `
  --https-only true `
  --output none

# Publish app
Write-Host "`n📦 Publishing application..." -ForegroundColor Yellow
$currentDir = Get-Location
Set-Location "$PSScriptRoot"

# Explicitly build and publish the SimpleWebApp project
Write-Host "  Building SimpleWebApp.csproj..." -ForegroundColor Gray
dotnet publish SimpleWebApp.csproj -c Release -o ./publish --nologo --verbosity minimal

if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Build failed!" -ForegroundColor Red
    Set-Location $currentDir
    exit 1
}

# Verify publish output
Write-Host "`n🔍 Verifying publish output..." -ForegroundColor Yellow
$publishedFiles = Get-ChildItem ./publish -Recurse -File
Write-Host "  Total files: $($publishedFiles.Count)" -ForegroundColor Gray
$runtimeConfigFiles = $publishedFiles | Where-Object {$_.Name -like '*.runtimeconfig.json'}
Write-Host "  Runtime configs: $($runtimeConfigFiles.Count)" -ForegroundColor Gray

if ($runtimeConfigFiles.Count -eq 0) {
    Write-Host "❌ ERROR: No .runtimeconfig.json found!" -ForegroundColor Red
    Set-Location $currentDir
    exit 1
}

Write-Host "  ✅ Publish successful!" -ForegroundColor Green

Compress-Archive -Path ./publish/* -DestinationPath ./app.zip -Force
$zipSize = (Get-Item ./app.zip).Length / 1MB
Write-Host "  ZIP size: $([math]::Round($zipSize, 2)) MB" -ForegroundColor Gray

# Deploy
Write-Host "`n🚀 Deploying to Azure..." -ForegroundColor Yellow
az webapp deployment source config-zip --name $WebAppName --resource-group $ResourceGroup --src ./app.zip --timeout 300

# Cleanup
Write-Host "`n🧹 Cleaning up..." -ForegroundColor Yellow
Remove-Item ./publish -Recurse -Force -ErrorAction SilentlyContinue
Remove-Item ./app.zip -Force -ErrorAction SilentlyContinue

Set-Location $currentDir

# Wait for app to start
Write-Host "`n⏳ Waiting for app to start (30 seconds)..." -ForegroundColor Yellow
Start-Sleep -Seconds 30

# Test
Write-Host "`n🔍 Testing deployment..." -ForegroundColor Yellow
[System.Net.ServicePointManager]::ServerCertificateValidationCallback = {$true}

$hostname = az webapp show --name $WebAppName --resource-group $ResourceGroup --query "defaultHostName" -o tsv

Write-Host "Testing endpoints on: $hostname" -ForegroundColor Cyan

$endpoints = @("/", "/health", "/api/info")
foreach ($endpoint in $endpoints) {
    try {
        $testUrl = "https://$hostname$endpoint"
        $response = Invoke-RestMethod -Uri $testUrl -TimeoutSec 10
        Write-Host "  ✅ $endpoint : OK" -ForegroundColor Green
        if ($endpoint -eq "/") {
            Write-Host "     Response: $($response.message)" -ForegroundColor Gray
        }
    } catch {
        Write-Host "  ❌ $endpoint : Failed - $($_.Exception.Message)" -ForegroundColor Red
    }
}

# Done
Write-Host "`n✅ Deployment complete!" -ForegroundColor Green
Write-Host "`n🌐 Your App URLs:" -ForegroundColor Cyan
Write-Host "  Home:    https://$hostname" -ForegroundColor White
Write-Host "  Swagger: https://$hostname/swagger" -ForegroundColor White
Write-Host "  Health:  https://$hostname/health" -ForegroundColor White
Write-Host "  Info:    https://$hostname/api/info" -ForegroundColor White

Write-Host "`n📝 Test Commands:" -ForegroundColor Cyan
Write-Host "  curl https://$hostname/" -ForegroundColor Gray
Write-Host "  curl https://$hostname/health" -ForegroundColor Gray
Write-Host "  curl https://$hostname/api/info" -ForegroundColor Gray

# Open in browser
Write-Host "`n🌐 Opening Swagger UI in browser..." -ForegroundColor Yellow
Start-Process "https://$hostname/swagger"