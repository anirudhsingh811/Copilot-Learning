# Setup Local Storage Resources for Azure Functions
# This script creates all required queues and blob containers in Azurite

Write-Host "?? Setting up local storage resources for Azure Functions..." -ForegroundColor Cyan
Write-Host ""

# Connection string for local Azurite emulator
$connectionString = "UseDevelopmentStorage=true"

try {
    # Import Azure Storage module
    Write-Host "?? Checking Azure Storage module..." -ForegroundColor Yellow
    if (-not (Get-Module -ListAvailable -Name Az.Storage)) {
        Write-Host "Installing Az.Storage module..." -ForegroundColor Yellow
        Install-Module -Name Az.Storage -Force -Scope CurrentUser -Repository PSGallery
    }
    Import-Module Az.Storage -ErrorAction Stop
    Write-Host "? Az.Storage module loaded" -ForegroundColor Green
    Write-Host ""

    # Create storage context
    Write-Host "?? Connecting to Azurite..." -ForegroundColor Yellow
    $context = New-AzStorageContext -ConnectionString $connectionString
    Write-Host "? Connected to local storage emulator" -ForegroundColor Green
    Write-Host ""

    # =============================================
    # CREATE QUEUES
    # =============================================
    Write-Host "?? Creating queues..." -ForegroundColor Cyan
    
    $queues = @(
        "order-queue",
        "notifications-queue", 
        "batch-orders-queue"
    )

    foreach ($queueName in $queues) {
        try {
            $existingQueue = Get-AzStorageQueue -Name $queueName -Context $context -ErrorAction SilentlyContinue
            
            if ($existingQueue) {
                Write-Host "  ??  Queue '$queueName' already exists" -ForegroundColor Yellow
            } else {
                New-AzStorageQueue -Name $queueName -Context $context | Out-Null
                Write-Host "  ? Created queue: $queueName" -ForegroundColor Green
            }
        }
        catch {
            Write-Host "  ? Failed to create queue '$queueName': $_" -ForegroundColor Red
        }
    }
    Write-Host ""

    # =============================================
    # CREATE BLOB CONTAINERS
    # =============================================
    Write-Host "???  Creating blob containers..." -ForegroundColor Cyan
    
    $containers = @(
        "uploads",
        "data-imports",
        "documents"
    )

    foreach ($containerName in $containers) {
        try {
            $existingContainer = Get-AzStorageContainer -Name $containerName -Context $context -ErrorAction SilentlyContinue
            
            if ($existingContainer) {
                Write-Host "  ??  Container '$containerName' already exists" -ForegroundColor Yellow
            } else {
                New-AzStorageContainer -Name $containerName -Context $context -Permission Off | Out-Null
                Write-Host "  ? Created container: $containerName" -ForegroundColor Green
            }
        }
        catch {
            Write-Host "  ? Failed to create container '$containerName': $_" -ForegroundColor Red
        }
    }
    Write-Host ""

    # =============================================
    # SUMMARY
    # =============================================
    Write-Host "? Setup Complete!" -ForegroundColor Green
    Write-Host ""
    Write-Host "?? Summary:" -ForegroundColor Cyan
    Write-Host "  Queues created: $($queues.Count)" -ForegroundColor White
    foreach ($q in $queues) {
        Write-Host "    • $q" -ForegroundColor Gray
    }
    Write-Host ""
    Write-Host "  Containers created: $($containers.Count)" -ForegroundColor White
    foreach ($c in $containers) {
        Write-Host "    • $c" -ForegroundColor Gray
    }
    Write-Host ""
    Write-Host "?? Your Azure Functions are now ready to run!" -ForegroundColor Green
    Write-Host "   Run: func start" -ForegroundColor Yellow
}
catch {
    Write-Host ""
    Write-Host "? ERROR: $_" -ForegroundColor Red
    Write-Host ""
    Write-Host "?? Troubleshooting:" -ForegroundColor Yellow
    Write-Host "  1. Make sure Azurite is running:" -ForegroundColor White
    Write-Host "     azurite --silent --location c:\azurite" -ForegroundColor Gray
    Write-Host ""
    Write-Host "  2. Check if Az.Storage module is installed:" -ForegroundColor White
    Write-Host "     Get-Module -ListAvailable Az.Storage" -ForegroundColor Gray
    Write-Host ""
    exit 1
}
