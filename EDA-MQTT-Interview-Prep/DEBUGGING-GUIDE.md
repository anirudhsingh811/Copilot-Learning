# ?? Debugging Guide & System Dependencies

## ?? Table of Contents
1. [System Dependencies](#system-dependencies)
2. [Critical Debug Points](#critical-debug-points)
3. [Debugging Strategy](#debugging-strategy)
4. [Visual Studio Debug Configuration](#visual-studio-debug-configuration)
5. [Common Issues & Troubleshooting](#common-issues--troubleshooting)

---

## ?? System Dependencies

### 1. **Required Software & Links**

#### A. .NET 8 SDK (Required)
- **Link**: https://dotnet.microsoft.com/download/dotnet/8.0
- **Version**: .NET 8.0 or later
- **Verify Installation**:
  ```powershell
  dotnet --version
  # Should show: 8.0.x
  ```

#### B. Docker Desktop (Required for MQTT Broker)
- **Link**: https://www.docker.com/products/docker-desktop/
- **Platforms**: Windows, macOS, Linux
- **Verify Installation**:
  ```powershell
  docker --version
  docker-compose --version
  ```

#### C. Visual Studio 2022 (Recommended)
- **Link**: https://visualstudio.microsoft.com/downloads/
- **Edition**: Community (Free), Professional, or Enterprise
- **Minimum Version**: 17.8 or later (for .NET 8 support)
- **Required Workloads**:
  - ASP.NET and web development
  - .NET desktop development

#### D. Visual Studio Code (Alternative)
- **Link**: https://code.visualstudio.com/
- **Extensions**:
  - C# Dev Kit: https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csdevkit
  - C# Extension: https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csharp

#### E. MQTT Client Tools (Optional for Testing)
- **MQTT Explorer**: http://mqtt-explorer.com/
- **MQTTX**: https://mqttx.app/
- **Postman** (supports MQTT): https://www.postman.com/downloads/

---

### 2. **NuGet Package Dependencies**

All packages are already referenced in the `.csproj` files. They will be automatically restored when you build.

#### **OrderService Dependencies:**
```xml
<PackageReference Include="Microsoft.AspNetCore.OpenApi" Version="8.0.22" />
<PackageReference Include="MQTTnet" Version="4.3.3.952" />
<PackageReference Include="Swashbuckle.AspNetCore" Version="6.6.2" />
```
- **MQTTnet**: MQTT client library
  - NuGet: https://www.nuget.org/packages/MQTTnet/
- **Swashbuckle.AspNetCore**: Swagger/OpenAPI documentation
  - NuGet: https://www.nuget.org/packages/Swashbuckle.AspNetCore/
- **Microsoft.AspNetCore.OpenApi**: OpenAPI support
  - NuGet: https://www.nuget.org/packages/Microsoft.AspNetCore.OpenApi/

#### **PaymentService, InventoryService, NotificationService Dependencies:**
```xml
<PackageReference Include="MQTTnet" Version="4.3.3.952" />
```

#### **Shared Library Dependencies:**
```xml
<PackageReference Include="MQTTnet" Version="4.3.3.952" />
<PackageReference Include="System.Text.Json" Version="10.0.1" />
```
- **System.Text.Json**: JSON serialization/deserialization
  - NuGet: https://www.nuget.org/packages/System.Text.Json/

---

### 3. **Docker Image Dependencies**

#### **Eclipse Mosquitto MQTT Broker**
```yaml
image: eclipse-mosquitto:2.0
```
- **Docker Hub**: https://hub.docker.com/_/eclipse-mosquitto
- **Version**: 2.0
- **Ports**:
  - `1883`: MQTT protocol
  - `9001`: WebSocket (if needed)

---

## ?? Critical Debug Points

### **1. OrderService (src/OrderService/Program.cs)**

#### **Breakpoint Location #1: MQTT Connection**
```csharp
// Line ~13: After MQTT client initialization
var mqttClient = app.Services.GetRequiredService<MqttClientHelper>();
await mqttClient.ConnectAsync(); // ?? SET BREAKPOINT HERE
```
**Why**: Verify MQTT broker connection succeeds
**Check**:
- `mqttClient` is not null
- No exceptions thrown
- Connection state is connected

#### **Breakpoint Location #2: Order Creation - Entry Point**
```csharp
// Line ~15: Inside the POST endpoint handler
app.MapPost("/api/orders", async (CreateOrderRequest request, MqttClientHelper mqtt) =>
{
    // ?? SET BREAKPOINT HERE - First line inside handler
    var orderId = Guid.NewGuid().ToString();
```
**Why**: Capture incoming request data
**Inspect**:
- `request.CustomerId`
- `request.Items` collection
- Item count and values

#### **Breakpoint Location #3: Event Creation**
```csharp
// Line ~18: After creating OrderCreatedEvent
var orderEvent = new OrderCreatedEvent
{
    OrderId = orderId,
    CustomerId = request.CustomerId,
    Items = request.Items.Select(i => new Shared.Events.OrderItem { 
        ProductId = i.ProductId, 
        Quantity = i.Quantity, 
        Price = i.Price 
    }).ToList(),
    TotalAmount = request.Items.Sum(i => i.Price * i.Quantity),
    CreatedAt = DateTime.UtcNow
}; // ?? SET BREAKPOINT HERE
```
**Why**: Verify event object is created correctly
**Inspect**:
- `orderEvent.OrderId`
- `orderEvent.TotalAmount` calculation
- `orderEvent.Items.Count`

#### **Breakpoint Location #4: MQTT Publish**
```csharp
// Line ~26: Before publishing to MQTT
await mqtt.PublishAsync("orders/created", orderEvent); // ?? SET BREAKPOINT HERE
```
**Why**: Confirm event is being published
**Inspect**:
- `orderEvent` serialization
- Topic name: "orders/created"
- No exceptions during publish

#### **Breakpoint Location #5: Response Return**
```csharp
// Line ~27: Before returning response
return Results.Ok(new { OrderId = orderId, Message = "Order created successfully" }); // ?? SET BREAKPOINT HERE
```
**Why**: Verify response is sent back to client
**Inspect**:
- `orderId` value
- Response message

---

### **2. PaymentService (src/PaymentService/Program.cs)**

#### **Breakpoint Location #6: MQTT Connection**
```csharp
// Line ~5: After MQTT connection
var mqtt = new MqttClientHelper();
await mqtt.ConnectAsync(); // ?? SET BREAKPOINT HERE
```
**Why**: Ensure PaymentService connects to broker
**Check**: Connection state

#### **Breakpoint Location #7: Subscription Setup**
```csharp
// Line ~7: Before subscribing
await mqtt.SubscribeAsync("orders/created", async (payload) =>
{ // ?? SET BREAKPOINT HERE
    var orderEvent = JsonSerializer.Deserialize<OrderCreatedEvent>(payload);
```
**Why**: Confirm subscription is established
**Check**: Topic name and handler registration

#### **Breakpoint Location #8: Event Receipt**
```csharp
// Line ~9: Inside subscription handler when event arrives
var orderEvent = JsonSerializer.Deserialize<OrderCreatedEvent>(payload); // ?? SET BREAKPOINT HERE
Console.WriteLine($"[Payment] Processing payment for Order {orderEvent?.OrderId}");
```
**Why**: Verify event is received and deserialized
**Inspect**:
- `payload` (raw JSON string)
- `orderEvent` (deserialized object)
- `orderEvent?.OrderId`
- `orderEvent?.TotalAmount`

#### **Breakpoint Location #9: Payment Event Creation**
```csharp
// Line ~12: After payment processing delay
var paymentEvent = new PaymentProcessedEvent
{
    OrderId = orderEvent?.OrderId ?? string.Empty,
    PaymentId = Guid.NewGuid().ToString(),
    Amount = orderEvent?.TotalAmount ?? 0,
    Success = true,
    ProcessedAt = DateTime.UtcNow
}; // ?? SET BREAKPOINT HERE
```
**Why**: Verify payment event is created correctly
**Inspect**:
- `paymentEvent.PaymentId`
- `paymentEvent.Amount`
- `paymentEvent.Success`

#### **Breakpoint Location #10: Payment Event Publish**
```csharp
// Line ~19: Before publishing payment event
await mqtt.PublishAsync("payments/processed", paymentEvent); // ?? SET BREAKPOINT HERE
```
**Why**: Confirm payment event is published
**Check**: Topic name and event object

---

### **3. InventoryService (src/InventoryService/Program.cs)**

#### **Breakpoint Location #11: MQTT Connection**
```csharp
// Line ~5: After MQTT connection
var mqtt = new MqttClientHelper();
await mqtt.ConnectAsync(); // ?? SET BREAKPOINT HERE
```

#### **Breakpoint Location #12: Event Receipt**
```csharp
// Line ~9: Inside subscription handler
var orderEvent = JsonSerializer.Deserialize<OrderCreatedEvent>(payload); // ?? SET BREAKPOINT HERE
Console.WriteLine($"[Inventory] Reserving inventory for Order {orderEvent?.OrderId}");
```
**Why**: Verify order event is received
**Inspect**:
- `payload`
- `orderEvent?.OrderId`
- `orderEvent?.Items` (products to reserve)

#### **Breakpoint Location #13: Inventory Event Creation**
```csharp
// Line ~12: After reservation delay
var inventoryEvent = new InventoryReservedEvent
{
    OrderId = orderEvent?.OrderId ?? string.Empty,
    ReservationId = Guid.NewGuid().ToString(),
    Success = true,
    ReservedAt = DateTime.UtcNow
}; // ?? SET BREAKPOINT HERE
```
**Why**: Verify inventory reservation event
**Inspect**:
- `inventoryEvent.ReservationId`
- `inventoryEvent.Success`

#### **Breakpoint Location #14: Inventory Event Publish**
```csharp
// Line ~18: Before publishing
await mqtt.PublishAsync("inventory/reserved", inventoryEvent); // ?? SET BREAKPOINT HERE
```

---

### **4. NotificationService (src/NotificationService/Program.cs)**

#### **Breakpoint Location #15: MQTT Connection**
```csharp
// Line ~5: After MQTT connection
var mqtt = new MqttClientHelper();
await mqtt.ConnectAsync(); // ?? SET BREAKPOINT HERE
```

#### **Breakpoint Location #16: Order Notification**
```csharp
// Line ~9: When order event arrives
var orderEvent = JsonSerializer.Deserialize<OrderCreatedEvent>(payload); // ?? SET BREAKPOINT HERE
Console.WriteLine($"[Notification] Sending order confirmation email for Order {orderEvent?.OrderId} to customer {orderEvent?.CustomerId}");
```
**Why**: Verify order confirmation notification
**Inspect**:
- `orderEvent?.CustomerId`
- `orderEvent?.OrderId`

#### **Breakpoint Location #17: Payment Notification**
```csharp
// Line ~15: When payment event arrives
var paymentEvent = JsonSerializer.Deserialize<PaymentProcessedEvent>(payload); // ?? SET BREAKPOINT HERE
Console.WriteLine($"[Notification] Sending payment confirmation for Order {paymentEvent?.OrderId}");
```
**Why**: Verify payment notification
**Inspect**:
- `paymentEvent?.OrderId`
- `paymentEvent?.Success`

#### **Breakpoint Location #18: Inventory Notification**
```csharp
// Line ~21: When inventory event arrives
var inventoryEvent = JsonSerializer.Deserialize<InventoryReservedEvent>(payload); // ?? SET BREAKPOINT HERE
Console.WriteLine($"[Notification] Sending inventory confirmation for Order {inventoryEvent?.OrderId}");
```
**Why**: Verify inventory notification
**Inspect**:
- `inventoryEvent?.OrderId`
- `inventoryEvent?.Success`

---

### **5. Shared Library (src/Shared/MqttClientHelper.cs)**

#### **Breakpoint Location #19: MQTT Client Connection**
```csharp
// Line ~18: Inside ConnectAsync method
public async Task ConnectAsync()
{
    var options = new MqttClientOptionsBuilder()
        .WithTcpServer(_brokerHost, _brokerPort)
        .Build(); // ?? SET BREAKPOINT HERE
    
    await _client.ConnectAsync(options, CancellationToken.None); // ?? SET BREAKPOINT HERE
}
```
**Why**: Debug connection issues
**Inspect**:
- `_brokerHost` (should be "localhost")
- `_brokerPort` (should be 1883)
- Connection options
- Exceptions during connection

#### **Breakpoint Location #20: Message Publishing**
```csharp
// Line ~23: Inside PublishAsync method
public async Task PublishAsync<T>(string topic, T message)
{
    var payload = JsonSerializer.Serialize(message); // ?? SET BREAKPOINT HERE
    var mqttMessage = new MqttApplicationMessageBuilder()
        .WithTopic(topic)
        .WithPayload(payload)
        .Build(); // ?? SET BREAKPOINT HERE
    
    await _client.PublishAsync(mqttMessage); // ?? SET BREAKPOINT HERE
}
```
**Why**: Debug message publishing
**Inspect**:
- `topic` name
- `message` object
- `payload` (serialized JSON)
- MQTT message properties

#### **Breakpoint Location #21: Message Subscription**
```csharp
// Line ~30: Inside SubscribeAsync method
public async Task SubscribeAsync(string topic, Func<string, Task> handler)
{
    _client.ApplicationMessageReceivedAsync += async e => 
    { 
        if (e.ApplicationMessage.Topic == topic) // ?? SET BREAKPOINT HERE
        { 
            var payload = System.Text.Encoding.UTF8.GetString(e.ApplicationMessage.Payload); // ?? SET BREAKPOINT HERE
            await handler(payload); // ?? SET BREAKPOINT HERE
        } 
    };
    await _client.SubscribeAsync(topic); // ?? SET BREAKPOINT HERE
}
```
**Why**: Debug message reception
**Inspect**:
- `topic` being subscribed
- Incoming `e.ApplicationMessage.Topic`
- `payload` (raw bytes and decoded string)
- Handler execution

---

## ?? Debugging Strategy

### **Sequential Debugging Approach:**

#### **Phase 1: Verify Infrastructure**
1. Start MQTT Broker first
2. Verify broker is running: `docker ps`
3. Check broker logs: `docker logs mqtt-broker`

#### **Phase 2: Debug Individual Services**
1. **Start with OrderService**
   - Set breakpoints #1, #2, #3, #4, #5
   - Run in debug mode
   - Make POST request
   - Step through each breakpoint

2. **Add PaymentService**
   - Set breakpoints #6, #7, #8, #9, #10
   - Run in separate debug session
   - Trigger order creation
   - Watch for event receipt

3. **Add InventoryService**
   - Set breakpoints #11, #12, #13, #14
   - Run in separate debug session
   - Trigger order creation
   - Watch for event receipt

4. **Add NotificationService**
   - Set breakpoints #15, #16, #17, #18
   - Run in separate debug session
   - Trigger order creation
   - Watch all three event types

#### **Phase 3: End-to-End Debugging**
1. Run all 4 services simultaneously
2. Set strategic breakpoints at key handoff points
3. Use conditional breakpoints for specific OrderIds
4. Use Tracepoints for non-breaking logging

---

## ?? Visual Studio Debug Configuration

### **Method 1: Multiple Startup Projects**

1. Right-click on Solution in Solution Explorer
2. Select "Configure Startup Projects"
3. Choose "Multiple startup projects"
4. Set the following to "Start":
   - OrderService
   - PaymentService
   - InventoryService
   - NotificationService

### **Method 2: Launch Profiles (launchSettings.json)**

Create or modify `Properties/launchSettings.json` in each project:

**OrderService:**
```json
{
  "profiles": {
    "OrderService": {
      "commandName": "Project",
      "launchBrowser": true,
      "launchUrl": "swagger",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      },
      "applicationUrl": "http://localhost:5000"
    }
  }
}
```

**PaymentService, InventoryService, NotificationService:**
```json
{
  "profiles": {
    "ServiceName": {
      "commandName": "Project",
      "environmentVariables": {
        "DOTNET_ENVIRONMENT": "Development"
      }
    }
  }
}
```

### **Method 3: Debug Multiple Processes**

1. Start OrderService in debug mode (F5)
2. Debug ? Attach to Process (Ctrl+Alt+P)
3. Find and attach to PaymentService.exe
4. Repeat for InventoryService and NotificationService

---

## ?? Common Issues & Troubleshooting

### **Issue 1: MQTT Connection Failed**

**Symptoms:**
```
Error: Connection refused to localhost:1883
```

**Debug Steps:**
1. Check if MQTT broker is running:
   ```powershell
   docker ps
   # Should show mqtt-broker container
   ```

2. Check broker logs:
   ```powershell
   docker logs mqtt-broker
   ```

3. Set breakpoint in `MqttClientHelper.ConnectAsync()` (#19)
4. Inspect `_brokerHost` and `_brokerPort`

**Solution:**
- Start Docker Desktop
- Run: `docker-compose up -d` in infrastructure folder
- Verify with: `docker ps`

---

### **Issue 2: Events Not Received**

**Symptoms:**
- OrderService publishes, but PaymentService doesn't receive

**Debug Steps:**
1. Set breakpoint at PaymentService event receipt (#8)
2. Check if subscription was established (#7)
3. Verify topic names match exactly:
   - Publisher: "orders/created"
   - Subscriber: "orders/created"
4. Use MQTT Explorer to monitor topics

**Solution:**
- Check topic spelling
- Ensure service is running before order creation
- Verify MQTT broker is routing messages

---

### **Issue 3: Deserialization Errors**

**Symptoms:**
```
JsonException: The JSON value could not be converted
```

**Debug Steps:**
1. Set breakpoint before deserialization
2. Inspect raw `payload` string
3. Check event class properties match JSON

**Solution:**
- Ensure event classes are in Shared project
- Verify property names match (case-sensitive)
- Check JSON format

---

### **Issue 4: Port Already in Use**

**Symptoms:**
```
Error: Port 5000 is already in use
```

**Debug Steps:**
1. Find process using port:
   ```powershell
   netstat -ano | findstr :5000
   ```

2. Kill process or change port in Program.cs:
   ```csharp
   app.Run("http://localhost:5001");
   ```

---

### **Issue 5: NuGet Packages Not Restored**

**Symptoms:**
```
Error: The type or namespace 'MQTTnet' could not be found
```

**Solution:**
```powershell
# Restore all packages
dotnet restore

# Or in Visual Studio
Right-click Solution ? Restore NuGet Packages
```

---

## ?? Useful Debug Tools & Commands

### **1. Check .NET Version**
```powershell
dotnet --version
dotnet --list-sdks
```

### **2. Check Running Services**
```powershell
# Windows
netstat -ano | findstr :5000
netstat -ano | findstr :1883

# Linux/Mac
lsof -i :5000
lsof -i :1883
```

### **3. Docker Commands**
```powershell
# Check running containers
docker ps

# Check all containers
docker ps -a

# View logs
docker logs mqtt-broker

# Follow logs in real-time
docker logs -f mqtt-broker

# Restart broker
docker restart mqtt-broker

# Stop broker
docker stop mqtt-broker

# Start broker
docker start mqtt-broker
```

### **4. Test MQTT Broker**
```powershell
# Using MQTT Explorer (GUI)
# Connect to: localhost:1883

# Using mosquitto_sub (if installed)
mosquitto_sub -h localhost -p 1883 -t "orders/#"
```

### **5. Test OrderService API**
```powershell
# Using PowerShell
Invoke-WebRequest -Uri "http://localhost:5000/api/orders" `
  -Method POST `
  -ContentType "application/json" `
  -Body '{"customerId": "C123", "items": [{"productId": "P001", "quantity": 2, "price": 29.99}]}'

# Using curl
curl -X POST http://localhost:5000/api/orders `
  -H "Content-Type: application/json" `
  -d '{"customerId": "C123", "items": [{"productId": "P001", "quantity": 2, "price": 29.99}]}'
```

---

## ?? Debug Output Monitoring

### **Expected Console Output Sequence:**

**OrderService:**
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5000
```

**PaymentService:**
```
Payment Service Starting...
Payment Service Running. Press Ctrl+C to exit.
[Payment] Processing payment for Order 3fa85f64-5717-4562-b3fc-2c963f66afa6
[Payment] Payment successful for Order 3fa85f64-5717-4562-b3fc-2c963f66afa6
```

**InventoryService:**
```
Inventory Service Starting...
Inventory Service Running. Press Ctrl+C to exit.
[Inventory] Reserving inventory for Order 3fa85f64-5717-4562-b3fc-2c963f66afa6
[Inventory] Reservation successful for Order 3fa85f64-5717-4562-b3fc-2c963f66afa6
```

**NotificationService:**
```
Notification Service Starting...
Notification Service Running. Press Ctrl+C to exit.
[Notification] Sending order confirmation email for Order 3fa85f64-5717-4562-b3fc-2c963f66afa6 to customer C123
[Notification] Sending payment confirmation for Order 3fa85f64-5717-4562-b3fc-2c963f66afa6
[Notification] Sending inventory confirmation for Order 3fa85f64-5717-4562-b3fc-2c963f66afa6
```

---

## ?? Quick Start Debug Checklist

- [ ] .NET 8 SDK installed and verified
- [ ] Docker Desktop installed and running
- [ ] Visual Studio 2022 or VS Code installed
- [ ] MQTT Broker started (`docker-compose up -d`)
- [ ] All NuGet packages restored (`dotnet restore`)
- [ ] Solution builds successfully (`dotnet build`)
- [ ] Set breakpoints at critical locations
- [ ] Start all 4 services in debug mode
- [ ] Make test API call to OrderService
- [ ] Monitor console outputs
- [ ] Use MQTT Explorer to verify message flow

---

**Happy Debugging! ???**
