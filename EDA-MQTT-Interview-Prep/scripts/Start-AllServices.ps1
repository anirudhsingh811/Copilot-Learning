# Start-AllServices.ps1
# Convenience script to start all EDA services

param(
    [switch]$SkipBroker,
    [switch]$WaitForKeyPress
)

Write-Host "==================================================" -ForegroundColor Cyan
Write-Host "  Starting EDA-MQTT POC Services" -ForegroundColor Cyan
Write-Host "==================================================" -ForegroundColor Cyan
Write-Host ""

# Check if Docker is running
try {
    $null = docker ps 2>&1
} catch {
    Write-Host "? Docker is not running!" -ForegroundColor Red
    Write-Host ""
    Write-Host "Please start Docker Desktop first." -ForegroundColor Yellow
    exit 1
}

# Start MQTT Broker
if (-not $SkipBroker) {
    Write-Host "Starting MQTT Broker..." -ForegroundColor Yellow
    Push-Location infrastructure
    docker-compose up -d
    Pop-Location
    
    # Wait for broker to be ready
    Write-Host "Waiting for broker to start..." -ForegroundColor Gray
    Start-Sleep -Seconds 3
    
    $brokerRunning = docker ps --filter "name=mqtt-broker" --format "{{.Names}}"
    if ($brokerRunning) {
        Write-Host "? MQTT Broker is running" -ForegroundColor Green
    } else {
        Write-Host "? Failed to start MQTT Broker" -ForegroundColor Red
        exit 1
    }
} else {
    Write-Host "Skipping MQTT Broker (already running)" -ForegroundColor Gray
}

Write-Host ""
Write-Host "Starting microservices..." -ForegroundColor Yellow
Write-Host ""

# Services to start
$services = @(
    @{ Name = "OrderService"; Path = "src\OrderService"; Port = 5000 }
    @{ Name = "PaymentService"; Path = "src\PaymentService"; Port = $null }
    @{ Name = "InventoryService"; Path = "src\InventoryService"; Port = $null }
    @{ Name = "NotificationService"; Path = "src\NotificationService"; Port = $null }
)

Write-Host "Services will be started in new PowerShell windows." -ForegroundColor Cyan
Write-Host "Check each window for startup messages and logs." -ForegroundColor Cyan
Write-Host ""

foreach ($service in $services) {
    Write-Host "Starting $($service.Name)..." -ForegroundColor Yellow
    
    $command = "dotnet run --project $($service.Path)"
    
    # Create a new PowerShell window for each service
    $psCommand = @"
`$host.UI.RawUI.WindowTitle = '$($service.Name)'
Write-Host '========================================' -ForegroundColor Cyan
Write-Host ' $($service.Name)' -ForegroundColor Cyan
Write-Host '========================================' -ForegroundColor Cyan
Write-Host ''
cd '$PWD'
$command
Write-Host ''
Write-Host 'Press any key to exit...' -ForegroundColor Yellow
`$null = `$Host.UI.RawUI.ReadKey('NoEcho,IncludeKeyDown')
"@
    
    Start-Process powershell -ArgumentList "-NoExit", "-Command", $psCommand
    
    # Small delay between starting services
    Start-Sleep -Milliseconds 500
}

Write-Host ""
Write-Host "==================================================" -ForegroundColor Cyan
Write-Host "  All services are starting!" -ForegroundColor Green
Write-Host "==================================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "What to do next:" -ForegroundColor White
Write-Host ""
Write-Host "1. Wait ~5 seconds for all services to start" -ForegroundColor Gray
Write-Host ""
Write-Host "2. Check each window for 'Running' or 'listening' messages:" -ForegroundColor Gray
Write-Host "   - OrderService: Should show 'Now listening on: http://localhost:5000'" -ForegroundColor Gray
Write-Host "   - PaymentService: Should show 'Payment Service Running'" -ForegroundColor Gray
Write-Host "   - InventoryService: Should show 'Inventory Service Running'" -ForegroundColor Gray
Write-Host "   - NotificationService: Should show 'Notification Service Running'" -ForegroundColor Gray
Write-Host ""
Write-Host "3. Test the system:" -ForegroundColor Gray
Write-Host "   .\scripts\Test-OrderAPI.ps1" -ForegroundColor Yellow
Write-Host ""
Write-Host "4. Watch the event flow in all 4 service windows" -ForegroundColor Gray
Write-Host ""
Write-Host "To stop all services:" -ForegroundColor White
Write-Host "  - Close each PowerShell window" -ForegroundColor Gray
Write-Host "  - Or press Ctrl+C in each window" -ForegroundColor Gray
Write-Host ""
Write-Host "To stop the MQTT broker:" -ForegroundColor White
Write-Host "  docker-compose -f infrastructure\docker-compose.yml down" -ForegroundColor Yellow
Write-Host ""
Write-Host "==================================================" -ForegroundColor Cyan

if ($WaitForKeyPress) {
    Write-Host ""
    Write-Host "Press any key to continue..." -ForegroundColor Yellow
    $null = $Host.UI.RawUI.ReadKey('NoEcho,IncludeKeyDown')
}
