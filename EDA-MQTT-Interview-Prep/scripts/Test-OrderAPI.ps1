# Test-OrderAPI.ps1
# Script to test OrderService API

param(
    [string]$CustomerId = "C123",
    [string]$ProductId = "P001",
    [int]$Quantity = 2,
    [decimal]$Price = 29.99,
    [string]$BaseUrl = "http://localhost:5000"
)

Write-Host "==================================================" -ForegroundColor Cyan
Write-Host "  Testing OrderService API" -ForegroundColor Cyan
Write-Host "==================================================" -ForegroundColor Cyan
Write-Host ""

# Build request body
$requestBody = @{
    customerId = $CustomerId
    items = @(
        @{
            productId = $ProductId
            quantity = $Quantity
            price = $Price
        }
    )
}

# Convert to JSON
$jsonBody = $requestBody | ConvertTo-Json -Depth 10

Write-Host "Request URL: $BaseUrl/api/orders" -ForegroundColor Yellow
Write-Host "Request Body:" -ForegroundColor Yellow
Write-Host $jsonBody -ForegroundColor Gray
Write-Host ""

try {
    # Make the API call
    Write-Host "Sending request..." -ForegroundColor Cyan
    
    $response = Invoke-RestMethod `
        -Uri "$BaseUrl/api/orders" `
        -Method POST `
        -ContentType "application/json" `
        -Body $jsonBody
    
    Write-Host "? Success!" -ForegroundColor Green
    Write-Host ""
    Write-Host "Response:" -ForegroundColor Green
    Write-Host "  Order ID: $($response.OrderId)" -ForegroundColor White
    Write-Host "  Message: $($response.Message)" -ForegroundColor White
    Write-Host ""
    
    # Calculate total
    $total = $Quantity * $Price
    Write-Host "Order Summary:" -ForegroundColor Cyan
    Write-Host "  Customer: $CustomerId" -ForegroundColor Gray
    Write-Host "  Product: $ProductId" -ForegroundColor Gray
    Write-Host "  Quantity: $Quantity" -ForegroundColor Gray
    Write-Host "  Price: `$$Price" -ForegroundColor Gray
    Write-Host "  Total: `$$total" -ForegroundColor Gray
    
    Write-Host ""
    Write-Host "==================================================" -ForegroundColor Cyan
    Write-Host "Check other service consoles for event processing!" -ForegroundColor Yellow
    Write-Host "  - PaymentService: Should process payment" -ForegroundColor Gray
    Write-Host "  - InventoryService: Should reserve inventory" -ForegroundColor Gray
    Write-Host "  - NotificationService: Should send notifications" -ForegroundColor Gray
    Write-Host "==================================================" -ForegroundColor Cyan
}
catch {
    Write-Host "? Error occurred!" -ForegroundColor Red
    Write-Host ""
    Write-Host "Error Details:" -ForegroundColor Red
    Write-Host "  Message: $($_.Exception.Message)" -ForegroundColor White
    
    if ($_.Exception.Response) {
        $statusCode = $_.Exception.Response.StatusCode.value__
        Write-Host "  Status Code: $statusCode" -ForegroundColor White
    }
    
    Write-Host ""
    Write-Host "Troubleshooting:" -ForegroundColor Yellow
    Write-Host "  1. Is OrderService running?" -ForegroundColor Gray
    Write-Host "     Run: dotnet run --project src\OrderService" -ForegroundColor Gray
    Write-Host ""
    Write-Host "  2. Is it listening on port 5000?" -ForegroundColor Gray
    Write-Host "     Check console output for: 'Now listening on: http://localhost:5000'" -ForegroundColor Gray
    Write-Host ""
    Write-Host "  3. Is the MQTT broker running?" -ForegroundColor Gray
    Write-Host "     Run: docker ps | findstr mqtt-broker" -ForegroundColor Gray
    Write-Host ""
}
