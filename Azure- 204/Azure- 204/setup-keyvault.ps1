# Quick Setup Script for Azure Key Vault Access
# This script will:
# 1. Login to Azure (if not already logged in)
# 2. Grant you access to the Key Vault
# 3. Create test secrets

param(
    [string]$KeyVaultName = "kv-az204-9199",
    [string]$SubscriptionId = "c01b8f09-10c7-4d70-b719-05b1b231f9d3"
)

Write-Host "????????????????????????????????????????????????????????" -ForegroundColor Cyan
Write-Host "  Azure Key Vault Quick Setup" -ForegroundColor Cyan
Write-Host "????????????????????????????????????????????????????????" -ForegroundColor Cyan
Write-Host ""

# Check if Azure CLI is installed
try {
    $azVersion = az version 2>&1
    Write-Host "? Azure CLI is installed" -ForegroundColor Green
} catch {
    Write-Host "? Azure CLI is not installed. Please install it from:" -ForegroundColor Red
    Write-Host "   https://docs.microsoft.com/cli/azure/install-azure-cli" -ForegroundColor Yellow
    exit 1
}

# Login check
Write-Host "Checking Azure login status..." -ForegroundColor Yellow
$accountInfo = az account show 2>&1
if ($LASTEXITCODE -ne 0) {
    Write-Host "Not logged in. Starting login process..." -ForegroundColor Yellow
    az login
    if ($LASTEXITCODE -ne 0) {
        Write-Host "? Login failed" -ForegroundColor Red
        exit 1
    }
} else {
    Write-Host "? Already logged in" -ForegroundColor Green
}

# Set subscription
Write-Host "Setting subscription: $SubscriptionId..." -ForegroundColor Yellow
az account set --subscription $SubscriptionId
if ($LASTEXITCODE -ne 0) {
    Write-Host "? Failed to set subscription" -ForegroundColor Red
    exit 1
}
Write-Host "? Subscription set" -ForegroundColor Green

# Get current user
Write-Host "Getting current user..." -ForegroundColor Yellow
$currentUser = az ad signed-in-user show --query userPrincipalName -o tsv
if ($LASTEXITCODE -ne 0) {
    Write-Host "??  Could not get UPN, trying with object ID..." -ForegroundColor Yellow
    $objectId = az ad signed-in-user show --query id -o tsv
    if ($LASTEXITCODE -ne 0) {
        Write-Host "? Failed to get user information" -ForegroundColor Red
        exit 1
    }
    Write-Host "? Current user object ID: $objectId" -ForegroundColor Green
    $useObjectId = $true
} else {
    Write-Host "? Current user: $currentUser" -ForegroundColor Green
    $useObjectId = $false
}

# Check if Key Vault exists
Write-Host "Checking if Key Vault exists..." -ForegroundColor Yellow
$kvExists = az keyvault show --name $KeyVaultName 2>&1
if ($LASTEXITCODE -ne 0) {
    Write-Host "? Key Vault '$KeyVaultName' does not exist" -ForegroundColor Red
    Write-Host "   Please create it first or update the KeyVaultName parameter" -ForegroundColor Yellow
    exit 1
}
Write-Host "? Key Vault found" -ForegroundColor Green

# Grant access
Write-Host ""
Write-Host "Granting access to Key Vault..." -ForegroundColor Yellow
if ($useObjectId) {
    az keyvault set-policy --name $KeyVaultName `
        --object-id $objectId `
        --secret-permissions get list set delete
} else {
    az keyvault set-policy --name $KeyVaultName `
        --upn $currentUser `
        --secret-permissions get list set delete
}

if ($LASTEXITCODE -ne 0) {
    Write-Host "? Failed to grant access" -ForegroundColor Red
    exit 1
}
Write-Host "? Access granted successfully" -ForegroundColor Green

# Create test secrets
Write-Host ""
Write-Host "Creating test secrets..." -ForegroundColor Yellow

$secrets = @{
    "DatabaseConnectionString" = "Server=tcp:demo-server.database.windows.net,1433;Database=demo-db;User ID=demo-user;Password=Demo123!;Encrypt=True;"
    "ApiKey" = "demo-api-key-abc123xyz789"
    "StorageAccountKey" = "demo-storage-key-def456uvw012"
}

$successCount = 0
foreach ($secret in $secrets.GetEnumerator()) {
    Write-Host "  Creating secret: $($secret.Key)..." -ForegroundColor Cyan
    
    az keyvault secret set --vault-name $KeyVaultName `
        --name $secret.Key `
        --value $secret.Value `
        --output none
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "    ? Created" -ForegroundColor Green
        $successCount++
    } else {
        Write-Host "    ? Failed" -ForegroundColor Red
    }
}

Write-Host ""
Write-Host "????????????????????????????????????????????????????????" -ForegroundColor Cyan
if ($successCount -eq $secrets.Count) {
    Write-Host "  ? Setup Complete!" -ForegroundColor Green
} else {
    Write-Host "  ??  Setup completed with errors" -ForegroundColor Yellow
}
Write-Host "????????????????????????????????????????????????????????" -ForegroundColor Cyan
Write-Host ""
Write-Host "Summary:" -ForegroundColor Cyan
Write-Host "  • Key Vault: $KeyVaultName" -ForegroundColor White
Write-Host "  • Secrets Created: $successCount / $($secrets.Count)" -ForegroundColor White
Write-Host ""
Write-Host "You can now run the application and test Key Vault integration!" -ForegroundColor Green
Write-Host ""
Write-Host "To verify, run:" -ForegroundColor Yellow
Write-Host "  az keyvault secret list --vault-name $KeyVaultName" -ForegroundColor Cyan
