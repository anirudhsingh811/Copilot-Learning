# ?? API Testing Quick Reference - PowerShell

## ?? Quick Commands

### **Test Single Order (Simplest)**
```powershell
.\scripts\Test-OrderAPI.ps1
```

### **Test Multiple Orders**
```powershell
.\scripts\Test-MultipleOrders.ps1 -Count 5
```

### **Manual Test (One-Liner)**
```powershell
Invoke-RestMethod -Uri http://localhost:5000/api/orders -Method POST -ContentType "application/json" -Body '{"customerId": "C123", "items": [{"productId": "P001", "quantity": 2, "price": 29.99}]}'
```

---

## ?? Method 1: Using Invoke-RestMethod (Recommended)

### **Basic Request:**
```powershell
Invoke-RestMethod -Uri http://localhost:5000/api/orders `
  -Method POST `
  -ContentType "application/json" `
  -Body '{"customerId": "C123", "items": [{"productId": "P001", "quantity": 2, "price": 29.99}]}'
```

### **With PowerShell Object (Cleaner):**
```powershell
$order = @{
    customerId = "C123"
    items = @(
        @{
            productId = "P001"
            quantity = 2
            price = 29.99
        }
    )
} | ConvertTo-Json

Invoke-RestMethod -Uri http://localhost:5000/api/orders `
  -Method POST `
  -ContentType "application/json" `
  -Body $order
```

### **Save Response:**
```powershell
$response = Invoke-RestMethod -Uri http://localhost:5000/api/orders `
  -Method POST `
  -ContentType "application/json" `
  -Body '{"customerId": "C123", "items": [{"productId": "P001", "quantity": 2, "price": 29.99}]}'

Write-Host "Order ID: $($response.OrderId)"
Write-Host "Message: $($response.Message)"
```

---

## ?? Method 2: Using Invoke-WebRequest (More Verbose)

### **Basic Request:**
```powershell
$response = Invoke-WebRequest -Uri http://localhost:5000/api/orders `
  -Method POST `
  -ContentType "application/json" `
  -Body '{"customerId": "C123", "items": [{"productId": "P001", "quantity": 2, "price": 29.99}]}'

# Show status
Write-Host "Status: $($response.StatusCode)"

# Parse JSON response
$result = $response.Content | ConvertFrom-Json
Write-Host "Order ID: $($result.OrderId)"
```

---

## ?? Method 3: Using curl.exe (Real curl, not alias)

### **Windows has curl.exe built-in (Windows 10 1803+):**

```powershell
curl.exe -X POST http://localhost:5000/api/orders `
  -H "Content-Type: application/json" `
  -d "{\"customerId\": \"C123\", \"items\": [{\"productId\": \"P001\", \"quantity\": 2, \"price\": 29.99}]}"
```

**Note:** Must escape double quotes with `\"`

---

## ? Common Errors & Fixes

### **Error 1: "Cannot bind parameter 'Headers'"**

**Problem:** Using `curl` (which is alias for Invoke-WebRequest) with Unix syntax

**Fix:** Use `curl.exe` or `Invoke-RestMethod` instead:
```powershell
# Don't do this:
curl -H "Content-Type: application/json" ...

# Do this instead:
curl.exe -H "Content-Type: application/json" ...
# Or:
Invoke-RestMethod -ContentType "application/json" ...
```

---

### **Error 2: "Connection refused" or "Cannot connect"**

**Problem:** OrderService not running

**Fix:**
```powershell
# Check if service is running
netstat -ano | findstr :5000

# If not running, start it:
dotnet run --project src\OrderService
```

---

### **Error 3: "JSON deserialization error"**

**Problem:** Invalid JSON format

**Fix:** Use PowerShell objects instead:
```powershell
$body = @{
    customerId = "C123"
    items = @(@{
        productId = "P001"
        quantity = 2
        price = 29.99
    })
} | ConvertTo-Json

Invoke-RestMethod -Uri http://localhost:5000/api/orders `
  -Method POST `
  -ContentType "application/json" `
  -Body $body
```

---

## ?? Test Scenarios

### **Scenario 1: Simple Order**
```powershell
Invoke-RestMethod -Uri http://localhost:5000/api/orders `
  -Method POST `
  -ContentType "application/json" `
  -Body '{"customerId": "C123", "items": [{"productId": "P001", "quantity": 2, "price": 29.99}]}'
```

### **Scenario 2: Multiple Items**
```powershell
$order = @{
    customerId = "C456"
    items = @(
        @{ productId = "P001"; quantity = 2; price = 29.99 },
        @{ productId = "P002"; quantity = 1; price = 49.99 }
    )
} | ConvertTo-Json -Depth 10

Invoke-RestMethod -Uri http://localhost:5000/api/orders `
  -Method POST `
  -ContentType "application/json" `
  -Body $order
```

### **Scenario 3: Large Quantity**
```powershell
.\scripts\Test-OrderAPI.ps1 -CustomerId "C789" -ProductId "P003" -Quantity 100 -Price 9.99
```

### **Scenario 4: Stress Test**
```powershell
.\scripts\Test-MultipleOrders.ps1 -Count 10
```

---

## ?? Complete Test Flow

### **Step 1: Start Infrastructure**
```powershell
# Start MQTT Broker
cd infrastructure
docker-compose up -d
cd ..
```

### **Step 2: Start Services**
```powershell
# Terminal 1
dotnet run --project src\OrderService

# Terminal 2
dotnet run --project src\PaymentService

# Terminal 3
dotnet run --project src\InventoryService

# Terminal 4
dotnet run --project src\NotificationService
```

### **Step 3: Verify Services are Running**
```powershell
.\scripts\Find-EDServices.ps1
```

### **Step 4: Create Test Order**
```powershell
.\scripts\Test-OrderAPI.ps1
```

### **Step 5: Watch Console Outputs**
- **OrderService:** Returns OrderId
- **PaymentService:** Processes payment (1 sec delay)
- **InventoryService:** Reserves inventory (0.8 sec delay)
- **NotificationService:** Sends 3 notifications

---

## ?? Debugging Requests

### **Show Full Request Details:**
```powershell
$response = Invoke-WebRequest -Uri http://localhost:5000/api/orders `
  -Method POST `
  -ContentType "application/json" `
  -Body '{"customerId": "C123", "items": [{"productId": "P001", "quantity": 2, "price": 29.99}]}' `
  -Verbose

Write-Host "Status Code: $($response.StatusCode)"
Write-Host "Headers: $($response.Headers)"
Write-Host "Content: $($response.Content)"
```

### **Test with Error Handling:**
```powershell
try {
    $response = Invoke-RestMethod -Uri http://localhost:5000/api/orders `
      -Method POST `
      -ContentType "application/json" `
      -Body '{"customerId": "C123", "items": [{"productId": "P001", "quantity": 2, "price": 29.99}]}'
    
    Write-Host "? Success: Order ID = $($response.OrderId)" -ForegroundColor Green
}
catch {
    Write-Host "? Error: $($_.Exception.Message)" -ForegroundColor Red
    
    if ($_.Exception.Response) {
        $reader = [System.IO.StreamReader]::new($_.Exception.Response.GetResponseStream())
        $responseBody = $reader.ReadToEnd()
        Write-Host "Response: $responseBody" -ForegroundColor Yellow
    }
}
```

---

## ?? Additional Test Commands

### **Check if OrderService is responding:**
```powershell
Invoke-RestMethod -Uri http://localhost:5000/swagger/index.html
```

### **Test with different customers:**
```powershell
$customers = @("C123", "C456", "C789")
foreach ($customer in $customers) {
    .\scripts\Test-OrderAPI.ps1 -CustomerId $customer
    Start-Sleep -Seconds 2
}
```

### **Monitor MQTT broker:**
```powershell
docker logs -f mqtt-broker
```

### **Check service processes:**
```powershell
.\scripts\Find-EDServices.ps1
```

---

## ? Quick Checklist

Before testing:
- [ ] Docker Desktop is running
- [ ] MQTT Broker is up: `docker ps | findstr mqtt-broker`
- [ ] OrderService is running on port 5000
- [ ] Other services (Payment, Inventory, Notification) are running
- [ ] No errors in any service console

Test command:
- [ ] `.\scripts\Test-OrderAPI.ps1` runs successfully
- [ ] OrderService returns OrderId
- [ ] PaymentService logs payment processing
- [ ] InventoryService logs inventory reservation
- [ ] NotificationService logs 3 notifications

---

## ?? Pretty Output Version

```powershell
function Test-Order {
    param(
        [string]$CustomerId = "C123",
        [string]$ProductId = "P001",
        [int]$Quantity = 2,
        [decimal]$Price = 29.99
    )
    
    $body = @{
        customerId = $CustomerId
        items = @(@{
            productId = $ProductId
            quantity = $Quantity
            price = $Price
        })
    } | ConvertTo-Json -Depth 10
    
    try {
        $response = Invoke-RestMethod -Uri http://localhost:5000/api/orders `
          -Method POST `
          -ContentType "application/json" `
          -Body $body
        
        Write-Host "? Order Created Successfully!" -ForegroundColor Green
        Write-Host "  Order ID: $($response.OrderId)" -ForegroundColor Cyan
        Write-Host "  Message: $($response.Message)" -ForegroundColor Gray
        Write-Host "  Total: `$$($Quantity * $Price)" -ForegroundColor Yellow
    }
    catch {
        Write-Host "? Failed to create order" -ForegroundColor Red
        Write-Host "  Error: $($_.Exception.Message)" -ForegroundColor Gray
    }
}

# Usage:
Test-Order
Test-Order -CustomerId "C456" -ProductId "P002" -Quantity 5 -Price 49.99
```

---

## ?? Related Scripts

- **Test-OrderAPI.ps1** - Create a single order with parameters
- **Test-MultipleOrders.ps1** - Create multiple random orders
- **Find-EDServices.ps1** - Find all running services and their PIDs

---

**Last Updated:** 2026-01-14  
**Project:** EDA-MQTT-Interview-Prep  
**PowerShell Version:** 5.1+ (Windows PowerShell) or 7+ (PowerShell Core)
