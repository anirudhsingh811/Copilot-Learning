# Find-EDServices.ps1
# Finds all running EDA POC services and their Process IDs

Write-Host "==================================================" -ForegroundColor Cyan
Write-Host "  EDA-MQTT POC - Running Services" -ForegroundColor Cyan
Write-Host "==================================================" -ForegroundColor Cyan
Write-Host ""

# Get all dotnet processes with command line info
$processes = Get-CimInstance Win32_Process | Where-Object { 
    $_.Name -eq "dotnet.exe" -and 
    $_.CommandLine -like "*EDA-MQTT-Interview-Prep*"
}

if ($processes.Count -eq 0) {
    Write-Host "No services are currently running." -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Start services with:" -ForegroundColor Gray
    Write-Host "  dotnet run --project src\OrderService" -ForegroundColor Gray
    Write-Host "  dotnet run --project src\PaymentService" -ForegroundColor Gray
    Write-Host "  dotnet run --project src\InventoryService" -ForegroundColor Gray
    Write-Host "  dotnet run --project src\NotificationService" -ForegroundColor Gray
    exit
}

foreach ($proc in $processes) {
    # Extract service name from command line
    $serviceName = "Unknown"
    if ($proc.CommandLine -match "OrderService") { $serviceName = "OrderService" }
    elseif ($proc.CommandLine -match "PaymentService") { $serviceName = "PaymentService" }
    elseif ($proc.CommandLine -match "InventoryService") { $serviceName = "InventoryService" }
    elseif ($proc.CommandLine -match "NotificationService") { $serviceName = "NotificationService" }
    
    # Get additional process info
    $processInfo = Get-Process -Id $proc.ProcessId
    
    Write-Host "Service: " -NoNewline
    Write-Host $serviceName -ForegroundColor Green
    Write-Host "  Process ID: " -NoNewline
    Write-Host $proc.ProcessId -ForegroundColor Yellow
    Write-Host "  Started: " -NoNewline
    Write-Host $processInfo.StartTime -ForegroundColor Gray
    Write-Host "  Memory: " -NoNewline
    Write-Host "$([math]::Round($processInfo.WorkingSet64 / 1MB, 2)) MB" -ForegroundColor Gray
    Write-Host ""
}

Write-Host "==================================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "To attach debugger in Visual Studio:" -ForegroundColor White
Write-Host "  1. Press Ctrl + Alt + P" -ForegroundColor Gray
Write-Host "  2. Type 'dotnet' in search box" -ForegroundColor Gray
Write-Host "  3. Find process by ID or command line" -ForegroundColor Gray
Write-Host "  4. Select and click 'Attach'" -ForegroundColor Gray
Write-Host ""
