# ?? Quick Start - 5 Minutes to Running Functions

## Step 1: Install Tools (One-time setup)

```powershell
# Install Azure Functions Core Tools
npm install -g azure-functions-core-tools@4 --unsafe-perm true

# Install Azurite (Storage Emulator)
npm install -g azurite

# Verify installation
func --version
azurite --version
```

## Step 2: Start Storage Emulator

```powershell
# Open a NEW terminal window and run:
azurite --silent
```

Keep this terminal open while testing functions.

## Step 3: Run the Functions

```powershell
# In your main terminal:
cd AZ204-RealFunctions
func start
```

You should see output like:
```
Functions:
        ArchiveDocument: blobTrigger
        BatchProcessOrders: queueTrigger
        DailyCleanupJob: timerTrigger
        GenerateDailyReports: timerTrigger
        GetOrderStatus: [GET] http://localhost:7071/api/orders/{orderId}
        HandlePoisonMessage: queueTrigger
        HourlyHealthCheck: timerTrigger
        HttpGetExample: [GET] http://localhost:7071/api/hello
        ImageThumbnailGenerator: blobTrigger
        ProcessCsvImport: blobTrigger
        ProcessNotification: queueTrigger
        ProcessOrder: [POST] http://localhost:7071/api/orders
        ProcessOrderFromQueue: queueTrigger
        ProcessPendingNotifications: timerTrigger
```

## Step 4: Test HTTP Functions

### Test 1: Simple GET Request
Open browser and visit:
```
http://localhost:7071/api/hello?name=Azure
```

You should see:
```json
{
  "message": "Hello, Azure!",
  "timestamp": "2024-01-15T10:30:00Z",
  "method": "GET"
}
```

### Test 2: Create an Order (POST)

In PowerShell:
```powershell
$order = @{
    ProductName = "Azure Functions Book"
    Quantity = 2
    UnitPrice = 49.99
    CustomerEmail = "student@example.com"
} | ConvertTo-Json

Invoke-RestMethod -Uri "http://localhost:7071/api/orders" `
    -Method Post `
    -Body $order `
    -ContentType "application/json"
```

You'll get a response:
```json
{
  "orderId": "abc123-...",
  "message": "Order received and queued for processing",
  "totalAmount": 99.98,
  "estimatedProcessingTime": "2-5 minutes"
}
```

**Watch the terminal** - you'll see the order being processed by the queue trigger function! ??

### Test 3: Check Order Status
```
http://localhost:7071/api/orders/abc123
```

## Step 5: Test Timer Functions (Manual Trigger)

Timer functions run on schedule, but you can trigger them manually:

```powershell
# Trigger daily cleanup
Invoke-RestMethod -Uri "http://localhost:7071/admin/functions/DailyCleanupJob" -Method Post

# Trigger health check
Invoke-RestMethod -Uri "http://localhost:7071/admin/functions/HourlyHealthCheck" -Method Post

# Trigger report generation
Invoke-RestMethod -Uri "http://localhost:7071/admin/functions/GenerateDailyReports" -Method Post
```

Watch your terminal for the output logs! ??

## What You Just Did! ?

1. ? Started real Azure Functions locally
2. ? Called HTTP triggers (GET/POST)
3. ? Saw queue trigger automatically process messages
4. ? Manually triggered timer functions
5. ? Saw real-time logging

## Next Steps

### Deploy to Azure (5 minutes)
```bash
az login

# Create Function App
az functionapp create \
  --resource-group YourResourceGroup \
  --consumption-plan-location eastus \
  --runtime dotnet-isolated \
  --functions-version 4 \
  --name func-az204-yourname \
  --storage-account yourstorageaccount

# Deploy
func azure functionapp publish func-az204-yourname
```

### Monitor in Azure Portal
1. Go to your Function App
2. Click "Functions" to see all your functions
3. Click on any function to see execution history
4. Click "Monitor" to see Application Insights data

## Troubleshooting

**Problem: "No job functions found"**
```bash
dotnet clean
dotnet build
func start
```

**Problem: "Storage emulator not running"**
- Make sure Azurite is running in another terminal
- Run: `azurite --silent`

**Problem: "Port 7071 already in use"**
```bash
# Stop other func processes
Get-Process func | Stop-Process
```

**Problem: Timer functions not running**
- They run on schedule (e.g., daily at 2 AM)
- Use the manual trigger commands above for testing

## Pro Tips ??

1. **Keep logs visible**: Watch the `func start` terminal to see real-time execution
2. **Use Postman**: Better for testing complex HTTP requests
3. **Check Azurite**: If queue functions don't trigger, check Azurite is running
4. **Test locally first**: Always test locally before deploying to Azure
5. **Use Application Insights**: In production, monitor everything!

---

**You now have production-ready Azure Functions! ??**

Compare this to the console app simulations - these are REAL functions that:
- ? Run in Azure
- ? Scale automatically
- ? Integrate with Azure services
- ? Support all trigger types
- ? Include monitoring and logging
