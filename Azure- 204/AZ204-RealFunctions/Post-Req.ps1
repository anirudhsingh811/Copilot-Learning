$order = @{
    ProductName = "Azure Functions Book"
    Quantity = 2
    UnitPrice = 49.99
    CustomerEmail = "student@example.com"
} | ConvertTo-Json

try {
    $result = Invoke-RestMethod -Uri "http://localhost:7071/api/orders" `
        -Method Post `
        -Body $order `
        -ContentType "application/json"
    
    Write-Host "✅ Order created successfully!" -ForegroundColor Green
    Write-Host "Order ID: $($result.httpResponse.orderId)"
    Write-Host "Total: $($result.httpResponse.totalAmount)"
    Write-Host "Message: $($result.httpResponse.message)"
} catch {
    Write-Host "❌ Error: $_" -ForegroundColor Red
}