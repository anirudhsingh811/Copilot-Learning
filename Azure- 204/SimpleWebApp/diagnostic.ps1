$webAppName = "webapp-az204-9199"
$rg = "rg-az204-compute"

Write-Host "Checking Web App status..." -ForegroundColor Cyan
$exists = az webapp show --name $webAppName --resource-group $rg 2>$null
if ($exists) {
    Write-Host "✓ Web App exists" -ForegroundColor Green
    az webapp show --name $webAppName --resource-group $rg --query "{state:state, defaultHostName:defaultHostName}" -o table
    
    Write-Host "`nRecent deployments:" -ForegroundColor Cyan
    az webapp deployment list --name $webAppName --resource-group $rg --query "[0:3].{time:endTime, status:status, id:id}" -o table
} else {
    Write-Host "✗ Web App does not exist - uncomment creation section!" -ForegroundColor Red
}