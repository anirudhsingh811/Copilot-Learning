# Quick Reference Card

## Commands Cheat Sheet

### Start/Stop MQTT Broker
`ash
# Start
cd infrastructure
docker-compose up -d

# Stop
docker-compose down

# View logs
docker logs mqtt-broker
docker logs -f mqtt-broker  # Follow logs
``n
### Run Services
`ash
# OrderService (Terminal 1)
cd src/OrderService && dotnet run

# PaymentService (Terminal 2)
cd src/PaymentService && dotnet run

# InventoryService (Terminal 3)
cd src/InventoryService && dotnet run

# NotificationService (Terminal 4)
cd src/NotificationService && dotnet run
``n
### Build & Test
`ash
# Build all
dotnet build

# Build specific project
dotnet build src/OrderService/OrderService.csproj

# Clean
dotnet clean
``n
### Test API
`ash
# Create Order (Bash/curl)
curl -X POST http://localhost:5000/api/orders \
  -H "Content-Type: application/json" \
  -d '{"customerId": "C123", "items": [{"productId": "P001", "quantity": 2, "price": 29.99}]}'
``n
`powershell
# Create Order (PowerShell)
$body = @{
    customerId = "C123"
    items = @(
        @{
            productId = "P001"
            quantity = 2
            price = 29.99
        }
    )
} | ConvertTo-Json

Invoke-RestMethod -Method POST -Uri http://localhost:5000/api/orders `n  -ContentType "application/json" `n  -Body $body
``n
## MQTT Topics
| Topic | Publisher | Subscribers | Purpose |
|-------|-----------|-------------|---------|
| orders/created | OrderService | Payment, Inventory, Notification | New order event |
| payments/processed | PaymentService | Notification | Payment completed |
| inventory/reserved | InventoryService | Notification | Inventory reserved |

## Service Ports
| Service | Port | Type | URL |
|---------|------|------|-----|
| MQTT Broker | 1883 | MQTT | mqtt://localhost:1883 |
| MQTT WebSocket | 9001 | WS | ws://localhost:9001 |
| OrderService | 5000 | HTTP | http://localhost:5000 |
| Swagger UI | 5000 | HTTP | http://localhost:5000/swagger |

## Key Files
`	ext
📁 Infrastructure
  └── docker-compose.yml          MQTT broker setup
  └── mosquitto/config/mosquitto.conf

📁 Shared Library
  ├── MqttClientHelper.cs         MQTT wrapper
  ├── Events/
  │   ├── OrderCreatedEvent.cs
  │   ├── PaymentProcessedEvent.cs
  │   └── InventoryReservedEvent.cs
  └── Models/OrderItem.cs

📁 Services
  ├── OrderService/Program.cs      REST API
  ├── PaymentService/Program.cs    Payment processor
  ├── InventoryService/Program.cs  Inventory manager
  └── NotificationService/Program.cs

📁 Documentation
  ├── README.md                    Quick start
  ├── SETUP.md                     Detailed setup
  ├── Interview-Guide.md           Theory & Q&A
  └── PROJECT-STRUCTURE.md         Architecture
``n
## Interview Questions (Quick Answers)

### Q: What is Event-Driven Architecture?
A: Architecture pattern where services communicate via events. Provides loose coupling, scalability, and async processing.

### Q: MQTT vs HTTP?
A: MQTT is lightweight pub/sub protocol (IoT, real-time). HTTP is request/response (REST APIs). MQTT has lower overhead and built-in QoS.

### Q: QoS Levels?
A: QoS 0 (at most once), QoS 1 (at least once), QoS 2 (exactly once). Trade-off between performance and reliability.

### Q: How to ensure idempotency?
A: Use unique request IDs, database constraints, and deduplication logic. Check if operation already completed before processing.

### Q: Event ordering?
A: Use timestamps, sequence numbers, or ordered topics. Consider partition keys in distributed systems.

### Q: Saga Pattern?
A: Manage distributed transactions with compensating actions. Each service performs local transaction and publishes event. On failure, trigger compensation.

## Common Issues & Solutions

### Issue: Services can't connect to MQTT
**Solution:**
`ash
# Check if broker is running
docker ps | grep mqtt-broker

# Restart broker
cd infrastructure && docker-compose restart
``n
### Issue: Port already in use
**Solution:**
`ash
# Find process using port 1883
netstat -ano | findstr :1883

# Kill process or change port in docker-compose.yml
``n
### Issue: Build errors
**Solution:**
`ash
dotnet clean
rm -rf src/*/bin src/*/obj
dotnet restore
dotnet build
``n
## Debugging Tips
1. **Enable verbose logging**: Check console output in each service
2. **Use MQTT client**: Tools like MQTT Explorer or mosquitto_sub
3. **Check broker logs**: docker logs mqtt-broker
4. **Test API**: Use Swagger UI at http://localhost:5000/swagger
5. **Correlation IDs**: Add to events for tracing (future enhancement)

## MQTT Test Commands
`ash
# Subscribe to all topics (requires mosquitto-clients)
mosquitto_sub -h localhost -t '#' -v

# Subscribe to specific topic
mosquitto_sub -h localhost -t 'orders/created'

# Publish test message
mosquitto_pub -h localhost -t 'test/topic' -m 'Hello MQTT'
``n
## Next Steps for Enhancement
1. ✅ Basic EDA with MQTT (Completed)
2. ⬜ Add error handling and retries
3. ⬜ Implement correlation IDs
4. ⬜ Add dead-letter queues
5. ⬜ Implement Saga pattern
6. ⬜ Add health checks
7. ⬜ Implement event sourcing
8. ⬜ Add authentication
9. ⬜ Create monitoring dashboard
10. ⬜ Add unit tests

## Resources
- [MQTT.org](https://mqtt.org)
- [MQTTnet GitHub](https://github.com/dotnet/MQTTnet)
- [Microservices Patterns](https://microservices.io/patterns)
- [Martin Fowler - EDA](https://martinfowler.com/articles/201701-event-driven.html)

---
**Pro Tip**: Run all 4 services in separate terminal windows to see the event flow in real-time! 🚀