# Event-Driven Architecture (EDA) with MQTT - Interview Preparation

This repository contains comprehensive materials to prepare you for system design interviews focusing on Event-Driven Architecture and MQTT.

## ?? Contents

1. **Theory & Concepts** (`docs/Interview-Guide.md`)
2. **Proof of Concept Project** - E-commerce order processing with .NET 8 & MQTT
3. **Code Examples** - Real-world patterns and practices

## ??? POC Architecture

```
Order API ? Order Service ? MQTT Broker ? [Payment, Inventory, Notification Services]
```

## ?? Quick Start

```bash
# 1. Start MQTT Broker
cd infrastructure && docker-compose up -d

# 2. Run services (in separate terminals)
dotnet run --project src/OrderService
dotnet run --project src/PaymentService
dotnet run --project src/InventoryService
dotnet run --project src/NotificationService

# 3. Test
curl -X POST http://localhost:5000/api/orders \
  -H "Content-Type: application/json" \
  -d '{"customerId": "C123", "items": [{"productId": "P001", "quantity": 2, "price": 29.99}]}'
```

## ?? Study Guide

- **Week 1**: Theory (docs/Interview-Guide.md)
- **Week 2**: Run POC, modify code
- **Week 3**: Practice system designs
- **Week 4**: Mock interviews

See `docs/Interview-Guide.md` for comprehensive theory and interview Q&A.

Good luck! ??
