# ?? PowerShell Scripts Reference

This directory contains helper scripts to manage and test the EDA-MQTT POC.

## ?? Available Scripts

| Script | Purpose | Usage |
|--------|---------|-------|
| **Start-AllServices.ps1** | Start all services automatically | `.\scripts\Start-AllServices.ps1` |
| **Stop-AllServices.ps1** | Stop all running services | `.\scripts\Stop-AllServices.ps1` |
| **Find-EDServices.ps1** | Find running services and PIDs | `.\scripts\Find-EDServices.ps1` |
| **Test-OrderAPI.ps1** | Create a single test order | `.\scripts\Test-OrderAPI.ps1` |
| **Test-MultipleOrders.ps1** | Create multiple test orders | `.\scripts\Test-MultipleOrders.ps1 -Count 5` |

---

## ?? Quick Start

### **Start Everything:**
```powershell
.\scripts\Start-AllServices.ps1
```

This will:
1. Start the MQTT broker (Docker)
2. Open 4 new PowerShell windows
3. Start OrderService, PaymentService, InventoryService, NotificationService

### **Test the System:**
```powershell
.\scripts\Test-OrderAPI.ps1
```

Watch all 4 service windows to see the event flow!

### **Stop Everything:**
```powershell
.\scripts\Stop-AllServices.ps1
```

---

## ?? Detailed Usage

### 1. **Start-AllServices.ps1**

**Basic Usage:**
```powershell
.\scripts\Start-AllServices.ps1
```

**Options:**
```powershell
# Skip starting broker (if already running)
.\scripts\Start-AllServices.ps1 -SkipBroker

# Wait for keypress before continuing
.\scripts\Start-AllServices.ps1 -WaitForKeyPress
```

**What it does:**
- ? Starts Docker MQTT broker
- ? Opens separate PowerShell windows for each service
- ? Starts OrderService on port 5000
- ? Starts PaymentService, InventoryService, NotificationService
- ? Sets window titles for easy identification

---

### 2. **Stop-AllServices.ps1**

**Basic Usage:**
```powershell
.\scripts\Stop-AllServices.ps1
```

**Options:**
```powershell
# Keep MQTT broker running
.\scripts\Stop-AllServices.ps1 -KeepBroker
```

**What it does:**
- ? Finds all running EDA service processes
- ? Stops each service gracefully
- ? Stops the MQTT broker (optional)
- ? Verifies clean shutdown

---

### 3. **Find-EDServices.ps1**

**Usage:**
```powershell
.\scripts\Find-EDServices.ps1
```

**Example Output:**
```
==================================================
  EDA-MQTT POC - Running Services
==================================================

Service: OrderService
  Process ID: 12345
  Started: 1/14/2026 8:00:00 AM
  Memory: 85.32 MB

Service: PaymentService
  Process ID: 67890
  Started: 1/14/2026 8:01:00 AM
  Memory: 72.15 MB

...
```

**Use Case:**
- Check which services are running
- Get Process IDs for debugging
- Attach debugger to specific service

---

### 4. **Test-OrderAPI.ps1**

**Basic Usage:**
```powershell
.\scripts\Test-OrderAPI.ps1
```

**With Parameters:**
```powershell
# Create custom order
.\scripts\Test-OrderAPI.ps1 `
  -CustomerId "C456" `
  -ProductId "P002" `
  -Quantity 5 `
  -Price 49.99

# Test different endpoint
.\scripts\Test-OrderAPI.ps1 -BaseUrl "http://localhost:5001"
```

**What it does:**
- ? Creates a test order via REST API
- ? Shows request body and response
- ? Displays order summary
- ? Provides troubleshooting tips on error

---

### 5. **Test-MultipleOrders.ps1**

**Basic Usage:**
```powershell
# Create 3 orders (default)
.\scripts\Test-MultipleOrders.ps1

# Create 10 orders
.\scripts\Test-MultipleOrders.ps1 -Count 10
```

**What it does:**
- ? Creates multiple random orders
- ? Uses random customers and products
- ? Shows progress and summary
- ? Lists all created Order IDs
- ? Adds small delay between requests

---

## ?? Common Workflows

### **Workflow 1: First Time Setup**

```powershell
# 1. Start everything
.\scripts\Start-AllServices.ps1

# 2. Wait ~5 seconds for services to start

# 3. Test with single order
.\scripts\Test-OrderAPI.ps1

# 4. Watch the event flow in all 4 windows

# 5. Stop when done
.\scripts\Stop-AllServices.ps1
```

---

### **Workflow 2: Development/Debugging**

```powershell
# 1. Start broker only
cd infrastructure
docker-compose up -d
cd ..

# 2. Start 3 services normally
# (Open 3 separate PowerShell windows)
dotnet run --project src\PaymentService
dotnet run --project src\InventoryService
dotnet run --project src\NotificationService

# 3. Debug OrderService in Visual Studio (F5)

# 4. Test with script
.\scripts\Test-OrderAPI.ps1

# 5. Step through breakpoints in VS
```

---

### **Workflow 3: Load Testing**

```powershell
# 1. Start all services
.\scripts\Start-AllServices.ps1

# 2. Create many orders
.\scripts\Test-MultipleOrders.ps1 -Count 50

# 3. Watch all service windows for processing

# 4. Check MQTT broker logs
docker logs -f mqtt-broker
```

---

### **Workflow 4: Attach Debugger**

```powershell
# 1. Start services
.\scripts\Start-AllServices.ps1

# 2. Find Process IDs
.\scripts\Find-EDServices.ps1

# 3. In Visual Studio:
#    - Press Ctrl + Alt + P
#    - Type "dotnet"
#    - Find service by Process ID
#    - Click Attach

# 4. Set breakpoints

# 5. Trigger event
.\scripts\Test-OrderAPI.ps1
```

---

## ?? Troubleshooting

### **Problem: "Execution policy doesn't allow running scripts"**

```powershell
# Check current policy
Get-ExecutionPolicy

# Set policy (run as Administrator)
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser

# Or run with bypass
PowerShell -ExecutionPolicy Bypass -File .\scripts\Start-AllServices.ps1
```

---

### **Problem: "Docker is not running"**

```powershell
# Check if Docker is running
docker version

# Start Docker Desktop
Start-Process "C:\Program Files\Docker\Docker\Docker Desktop.exe"

# Wait for Docker to start, then retry
.\scripts\Start-AllServices.ps1
```

---

### **Problem: "Port 5000 is already in use"**

```powershell
# Find what's using port 5000
netstat -ano | findstr :5000

# Kill the process (replace PID with actual Process ID)
Stop-Process -Id <PID> -Force

# Or change OrderService port in Program.cs
```

---

### **Problem: "Services not receiving events"**

```powershell
# 1. Check if MQTT broker is running
docker ps | findstr mqtt-broker

# 2. Check broker logs
docker logs mqtt-broker

# 3. Verify services are subscribed
.\scripts\Find-EDServices.ps1

# 4. Restart broker if needed
cd infrastructure
docker-compose restart mosquitto
cd ..
```

---

## ?? Additional Resources

- **API-TESTING-GUIDE.md** - Comprehensive API testing reference
- **DEBUGGING-GUIDE.md** - Debugging and breakpoint locations
- **MQTT-BROKER-CONFIG-GUIDE.md** - MQTT broker configuration
- **POC-FLOW-DIAGRAM.md** - System architecture and flow

---

## ?? Script Parameters Reference

### **Start-AllServices.ps1**
```powershell
-SkipBroker       # Don't start MQTT broker
-WaitForKeyPress  # Pause before exiting
```

### **Stop-AllServices.ps1**
```powershell
-KeepBroker       # Don't stop MQTT broker
```

### **Test-OrderAPI.ps1**
```powershell
-CustomerId  <string>   # Customer ID (default: "C123")
-ProductId   <string>   # Product ID (default: "P001")
-Quantity    <int>      # Quantity (default: 2)
-Price       <decimal>  # Price (default: 29.99)
-BaseUrl     <string>   # API URL (default: http://localhost:5000)
```

### **Test-MultipleOrders.ps1**
```powershell
-Count       <int>      # Number of orders (default: 3)
-BaseUrl     <string>   # API URL (default: http://localhost:5000)
```

---

## ?? Tips & Tricks

### **Tip 1: Use aliases**
```powershell
# Add to PowerShell profile
Set-Alias -Name start-eda -Value "C:\path\to\Start-AllServices.ps1"
Set-Alias -Name stop-eda -Value "C:\path\to\Stop-AllServices.ps1"
Set-Alias -Name test-eda -Value "C:\path\to\Test-OrderAPI.ps1"

# Then use:
start-eda
test-eda
stop-eda
```

### **Tip 2: Keep services visible**
```powershell
# Arrange windows side-by-side
# Press Win + Left/Right arrow on each window
# Or use PowerToys FancyZones
```

### **Tip 3: Monitor in real-time**
```powershell
# Keep MQTT Explorer open
# Watch topics: orders/created, payments/processed, inventory/reserved

# Or use command line
docker exec mqtt-broker mosquitto_sub -t "#" -v
```

### **Tip 4: Quick restart**
```powershell
# One-liner to restart everything
.\scripts\Stop-AllServices.ps1; Start-Sleep 2; .\scripts\Start-AllServices.ps1
```

---

**Last Updated:** 2026-01-14  
**Project:** EDA-MQTT-Interview-Prep  
**PowerShell Version:** 5.1+ or 7+
