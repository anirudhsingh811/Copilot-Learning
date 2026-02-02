# Stop-AllServices.ps1
# Stops all EDA POC services and the MQTT broker

param(
    [switch]$KeepBroker
)

Write-Host "==================================================" -ForegroundColor Cyan
Write-Host "  Stopping EDA-MQTT POC Services" -ForegroundColor Cyan
Write-Host "==================================================" -ForegroundColor Cyan
Write-Host ""

# Find and stop all dotnet processes related to this project
$processes = Get-CimInstance Win32_Process | Where-Object { 
    $_.Name -eq "dotnet.exe" -and 
    $_.CommandLine -like "*EDA-MQTT-Interview-Prep*"
}

if ($processes.Count -eq 0) {
    Write-Host "No services are currently running." -ForegroundColor Yellow
} else {
    Write-Host "Found $($processes.Count) service(s) running" -ForegroundColor Yellow
    Write-Host ""
    
    foreach ($proc in $processes) {
        $serviceName = "Unknown"
        if ($proc.CommandLine -match "OrderService") { $serviceName = "OrderService" }
        elseif ($proc.CommandLine -match "PaymentService") { $serviceName = "PaymentService" }
        elseif ($proc.CommandLine -match "InventoryService") { $serviceName = "InventoryService" }
        elseif ($proc.CommandLine -match "NotificationService") { $serviceName = "NotificationService" }
        
        try {
            Write-Host "Stopping $serviceName (PID: $($proc.ProcessId))..." -ForegroundColor Yellow
            Stop-Process -Id $proc.ProcessId -Force
            Write-Host "  ? Stopped" -ForegroundColor Green
        }
        catch {
            Write-Host "  ? Failed to stop: $($_.Exception.Message)" -ForegroundColor Red
        }
    }
}

Write-Host ""

# Stop MQTT Broker
if (-not $KeepBroker) {
    Write-Host "Stopping MQTT Broker..." -ForegroundColor Yellow
    
    try {
        Push-Location infrastructure
        docker-compose down
        Pop-Location
        Write-Host "  ? MQTT Broker stopped" -ForegroundColor Green
    }
    catch {
        Write-Host "  ? Failed to stop broker: $($_.Exception.Message)" -ForegroundColor Red
    }
} else {
    Write-Host "Keeping MQTT Broker running (use -KeepBroker flag to stop it)" -ForegroundColor Gray
}

Write-Host ""
Write-Host "==================================================" -ForegroundColor Cyan
Write-Host "  All services stopped" -ForegroundColor Green
Write-Host "==================================================" -ForegroundColor Cyan
Write-Host ""

# Verify nothing is running
$remainingProcesses = Get-CimInstance Win32_Process | Where-Object { 
    $_.Name -eq "dotnet.exe" -and 
    $_.CommandLine -like "*EDA-MQTT-Interview-Prep*"
}

if ($remainingProcesses.Count -gt 0) {
    Write-Host "Warning: $($remainingProcesses.Count) process(es) still running" -ForegroundColor Yellow
    Write-Host "Run this script again or manually kill processes" -ForegroundColor Gray
} else {
    Write-Host "? Clean shutdown - no services running" -ForegroundColor Green
}

Write-Host ""
