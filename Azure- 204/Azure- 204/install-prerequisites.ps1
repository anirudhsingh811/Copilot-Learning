# AZ-204 Prerequisites Installation Script
# Run this script to install all required tools

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "AZ-204 Prerequisites Installer" -ForegroundColor Cyan
Write-Host "========================================`n" -ForegroundColor Cyan

# Function to check if a command exists
function Test-Command {
    param($Command)
    $null -ne (Get-Command $Command -ErrorAction SilentlyContinue)
}

# Check .NET SDK
Write-Host "Checking .NET SDK..." -ForegroundColor Yellow
if (Test-Command "dotnet") {
    $dotnetVersion = dotnet --version
    Write-Host "? .NET SDK installed: $dotnetVersion" -ForegroundColor Green
    
    $majorVersion = [int]$dotnetVersion.Split('.')[0]
    if ($majorVersion -lt 8) {
        Write-Host "? Warning: .NET 8.0+ recommended for this project" -ForegroundColor Yellow
        Write-Host "  Download: https://dotnet.microsoft.com/download" -ForegroundColor Yellow
    }
} else {
    Write-Host "? .NET SDK not found" -ForegroundColor Red
    Write-Host "  Installing .NET 9 SDK..." -ForegroundColor Yellow
    
    if (Test-Command "winget") {
        winget install Microsoft.DotNet.SDK.9
    } else {
        Write-Host "  Please download from: https://dotnet.microsoft.com/download" -ForegroundColor Yellow
        Start-Process "https://dotnet.microsoft.com/download"
    }
}

Write-Host ""

# Check Azure CLI
Write-Host "Checking Azure CLI..." -ForegroundColor Yellow
if (Test-Command "az") {
    $azVersion = az version --query '\"azure-cli\"' -o tsv 2>$null
    Write-Host "? Azure CLI installed: $azVersion" -ForegroundColor Green
} else {
    Write-Host "? Azure CLI not found" -ForegroundColor Red
    Write-Host "  Installing Azure CLI..." -ForegroundColor Yellow
    
    if (Test-Command "winget") {
        Write-Host "  Using winget..." -ForegroundColor Cyan
        winget install -e --id Microsoft.AzureCLI
        
        Write-Host "`n? IMPORTANT: Close and reopen PowerShell after installation!" -ForegroundColor Yellow
        Write-Host "  Then run 'az --version' to verify." -ForegroundColor Yellow
    } else {
        Write-Host "  Downloading installer..." -ForegroundColor Cyan
        $installerUrl = "https://aka.ms/installazurecliwindows"
        Start-Process $installerUrl
        
        Write-Host "`n? IMPORTANT: After installing, close and reopen PowerShell!" -ForegroundColor Yellow
    }
}

Write-Host ""

# Check Azure PowerShell Module (optional)
Write-Host "Checking Azure PowerShell Module (optional)..." -ForegroundColor Yellow
if (Get-Module -ListAvailable -Name Az) {
    $azModuleVersion = (Get-Module -ListAvailable -Name Az | Select-Object -First 1).Version
    Write-Host "? Azure PowerShell Module installed: $azModuleVersion" -ForegroundColor Green
} else {
    Write-Host "? Azure PowerShell Module not installed (optional)" -ForegroundColor Gray
    Write-Host "  This is optional. Azure CLI is sufficient." -ForegroundColor Gray
    Write-Host "  To install: Install-Module -Name Az -Repository PSGallery -Force" -ForegroundColor Gray
}

Write-Host ""

# Check Git (optional but useful)
Write-Host "Checking Git (optional)..." -ForegroundColor Yellow
if (Test-Command "git") {
    $gitVersion = git --version
    Write-Host "? Git installed: $gitVersion" -ForegroundColor Green
} else {
    Write-Host "? Git not found (optional)" -ForegroundColor Gray
    Write-Host "  Useful for version control." -ForegroundColor Gray
    Write-Host "  Download: https://git-scm.com/download/win" -ForegroundColor Gray
}

Write-Host ""

# Check kubectl (needed for AKS)
Write-Host "Checking kubectl (needed for AKS)..." -ForegroundColor Yellow
if (Test-Command "kubectl") {
    $kubectlVersion = kubectl version --client --short 2>$null
    Write-Host "? kubectl installed" -ForegroundColor Green
} else {
    Write-Host "? kubectl not found" -ForegroundColor Gray
    Write-Host "  Will be needed for AKS deployment." -ForegroundColor Gray
    Write-Host "  Install with: az aks install-cli" -ForegroundColor Gray
}

Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "Summary" -ForegroundColor Cyan
Write-Host "========================================`n" -ForegroundColor Cyan

# Summary
$requiredTools = @(
    @{ Name = ".NET SDK 8.0+"; Check = (Test-Command "dotnet"); Required = $true },
    @{ Name = "Azure CLI"; Check = (Test-Command "az"); Required = $true },
    @{ Name = "Azure PowerShell"; Check = (Get-Module -ListAvailable -Name Az); Required = $false },
    @{ Name = "kubectl"; Check = (Test-Command "kubectl"); Required = $false },
    @{ Name = "Git"; Check = (Test-Command "git"); Required = $false }
)

$allRequired = $true
foreach ($tool in $requiredTools) {
    $status = if ($tool.Check) { "?" } else { if ($tool.Required) { "?"; $allRequired = $false } else { "?" } }
    $color = if ($tool.Check) { "Green" } elseif ($tool.Required) { "Red" } else { "Gray" }
    $required = if ($tool.Required) { "(Required)" } else { "(Optional)" }
    
    Write-Host "$status $($tool.Name) $required" -ForegroundColor $color
}

Write-Host ""

if ($allRequired) {
    Write-Host "? All required tools are installed!" -ForegroundColor Green
    Write-Host "`nNext steps:" -ForegroundColor Cyan
    Write-Host "1. Login to Azure: az login" -ForegroundColor White
    Write-Host "2. Set subscription: az account set --subscription 'YOUR-ID'" -ForegroundColor White
    Write-Host "3. Run deployment: .\deploy-all-services.ps1" -ForegroundColor White
} else {
    Write-Host "? Some required tools are missing!" -ForegroundColor Yellow
    Write-Host "`nPlease install missing tools and rerun this script." -ForegroundColor Yellow
    Write-Host "Or manually verify with:" -ForegroundColor Yellow
    Write-Host "  dotnet --version" -ForegroundColor Gray
    Write-Host "  az --version" -ForegroundColor Gray
}

Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "Quick Links" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ".NET SDK:    https://dotnet.microsoft.com/download" -ForegroundColor Gray
Write-Host "Azure CLI:   https://aka.ms/installazurecliwindows" -ForegroundColor Gray
Write-Host "Azure Free:  https://azure.microsoft.com/free/" -ForegroundColor Gray
Write-Host "Docs:        https://learn.microsoft.com/azure/" -ForegroundColor Gray

Write-Host "`nPress any key to continue..." -ForegroundColor Gray
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
