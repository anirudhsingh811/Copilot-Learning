# Test-MultipleOrders.ps1
# Script to create multiple test orders

param(
    [int]$Count = 3,
    [string]$BaseUrl = "http://localhost:5000"
)

Write-Host "==================================================" -ForegroundColor Cyan
Write-Host "  Creating $Count Test Orders" -ForegroundColor Cyan
Write-Host "==================================================" -ForegroundColor Cyan
Write-Host ""

$customers = @("C123", "C456", "C789", "C101", "C202")
$products = @(
    @{ Id = "P001"; Name = "Laptop"; Price = 999.99 }
    @{ Id = "P002"; Name = "Mouse"; Price = 29.99 }
    @{ Id = "P003"; Name = "Keyboard"; Price = 79.99 }
    @{ Id = "P004"; Name = "Monitor"; Price = 299.99 }
    @{ Id = "P005"; Name = "Headset"; Price = 149.99 }
)

$successCount = 0
$failCount = 0
$orderIds = @()

for ($i = 1; $i -le $Count; $i++) {
    Write-Host "[$i/$Count] Creating order..." -ForegroundColor Yellow
    
    # Random customer and product
    $customer = $customers | Get-Random
    $product = $products | Get-Random
    $quantity = Get-Random -Minimum 1 -Maximum 5
    
    $requestBody = @{
        customerId = $customer
        items = @(
            @{
                productId = $product.Id
                quantity = $quantity
                price = $product.Price
            }
        )
    } | ConvertTo-Json
    
    try {
        $response = Invoke-RestMethod `
            -Uri "$BaseUrl/api/orders" `
            -Method POST `
            -ContentType "application/json" `
            -Body $requestBody `
            -ErrorAction Stop
        
        $orderIds += $response.OrderId
        $successCount++
        
        $total = $quantity * $product.Price
        Write-Host "  ? Order Created: $($response.OrderId)" -ForegroundColor Green
        Write-Host "    Customer: $customer | Product: $($product.Name) | Qty: $quantity | Total: `$$total" -ForegroundColor Gray
    }
    catch {
        $failCount++
        Write-Host "  ? Failed: $($_.Exception.Message)" -ForegroundColor Red
    }
    
    # Small delay between requests
    Start-Sleep -Milliseconds 500
    Write-Host ""
}

Write-Host "==================================================" -ForegroundColor Cyan
Write-Host "Summary:" -ForegroundColor Cyan
Write-Host "  Total Orders: $Count" -ForegroundColor White
Write-Host "  Successful: $successCount" -ForegroundColor Green
Write-Host "  Failed: $failCount" -ForegroundColor Red
Write-Host ""
Write-Host "Order IDs Created:" -ForegroundColor Cyan
$orderIds | ForEach-Object {
    Write-Host "  - $_" -ForegroundColor Gray
}
Write-Host "==================================================" -ForegroundColor Cyan
