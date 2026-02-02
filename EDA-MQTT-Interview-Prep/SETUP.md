# Setup & Running Guide

## Prerequisites
- .NET 8.0 SDK
- Docker Desktop

## Quick Start

### 1. Start MQTT Broker
`ash
cd infrastructure
docker-compose up -d
``n
### 2. Verify MQTT Broker
`ash
docker logs mqtt-broker
``n
### 3. Run Services

Open 4 separate terminals and run:

**Terminal 1 - Order Service (API)**
`ash
cd src/OrderService
dotnet run
``nAPI will be available at: http://localhost:5000

**Terminal 2 - Payment Service**
`ash
cd src/PaymentService
dotnet run
``n
**Terminal 3 - Inventory Service**
`ash
cd src/InventoryService
dotnet run
``n
**Terminal 4 - Notification Service**
`ash
cd src/NotificationService
dotnet run
``n
### 4. Test the System

**Using curl:**
`ash
curl -X POST http://localhost:5000/api/orders \
  -H \"Content-Type: application/json\" \
  -d '{"customerId": "C123", "items": [{"productId": "P001", "quantity": 2, "price": 29.99}]}'
``n
**Using PowerShell:**
`powershell
Invoke-RestMethod -Method POST -Uri http://localhost:5000/api/orders `n  -ContentType "application/json" `n  -Body '{\"customerId\": \"C123\", \"items\": [{\"productId\": \"P001\", \"quantity\": 2, \"price\": 29.99}]}'
``n
## What to Observe

1. **OrderService** logs: Order created and event published
2. **PaymentService** logs: Payment processing
3. **InventoryService** logs: Inventory reservation
4. **NotificationService** logs: Notifications sent

## Architecture Flow
``nPOST /api/orders
       |
       v
   OrderService --[orders/created]--> MQTT Broker
                                           |
                    +----------------------+--------------------+
                    |                      |                    |
              PaymentService      InventoryService    NotificationService
                    |                      |                    |
      [payments/processed]      [inventory/reserved]      (email logs)
``n
## Troubleshooting

### MQTT Broker Not Starting
`ash
docker-compose down
docker-compose up -d
``n
### Services Can't Connect
- Check if MQTT broker is running: docker ps
- Check broker logs: docker logs mqtt-broker
- Verify port 1883 is not in use

### Build Errors
`ash
dotnet clean
dotnet restore
dotnet build
``n
## Next Steps
1. Study the code in each service
2. Read docs/Interview-Guide.md
3. Try modifying event handlers
4. Add error handling and retries
5. Implement additional services

Good luck! 🚀