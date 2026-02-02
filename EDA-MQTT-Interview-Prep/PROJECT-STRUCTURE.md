# Project Structure

## Overview
This repository demonstrates Event-Driven Architecture (EDA) using MQTT for interview preparation. It implements a microservices-based e-commerce order processing system.

## Directory Layout
``n EDA-MQTT-Interview-Prep/
 ├── docs/
 │   └── Interview-Guide.md          # Comprehensive interview Q&A
 ├── infrastructure/
 │   ├── docker-compose.yml          # MQTT broker setup
 │   └── mosquitto/
 │       ├── config/
 │       │   └── mosquitto.conf      # MQTT broker configuration
 │       ├── data/                    # Persistence storage
 │       └── log/                     # Broker logs
 ├── src/
 │   ├── Shared/                      # Shared library
 │   │   ├── Events/                  # Event definitions
 │   │   │   ├── OrderCreatedEvent.cs
 │   │   │   ├── PaymentProcessedEvent.cs
 │   │   │   └── InventoryReservedEvent.cs
 │   │   ├── Models/
 │   │   │   └── OrderItem.cs
 │   │   └── MqttClientHelper.cs     # MQTT helper class
 │   ├── OrderService/               # REST API service
 │   │   └── Program.cs              # Order API endpoint
 │   ├── PaymentService/             # Payment processor
 │   │   └── Program.cs
 │   ├── InventoryService/           # Inventory manager
 │   │   └── Program.cs
 │   └── NotificationService/        # Notification sender
 │       └── Program.cs
 ├── tests/                           # (Future: Unit tests)
 ├── README.md                        # Quick start guide
 ├── SETUP.md                         # Detailed setup instructions
 ├── PROJECT-STRUCTURE.md             # This file
 └── EDA-MQTT-Interview-Prep.sln      # Solution file
``n
## Components

### 1. Infrastructure
**MQTT Broker (Eclipse Mosquitto)**
- Port 1883: MQTT protocol
- Port 9001: WebSocket protocol
- Configured for anonymous access (development only)
- Persistent storage enabled

### 2. Shared Library
Contains common code used by all services:
- **Events**: Domain events (OrderCreated, PaymentProcessed, etc.)
- **Models**: Shared data models
- **MqttClientHelper**: MQTT client wrapper with publish/subscribe functionality

### 3. OrderService
**Type**: ASP.NET Core Web API
**Port**: 5000
**Responsibilities**:
- Expose REST API for creating orders
- Publish OrderCreated events to MQTT
- API Endpoint: POST /api/orders

### 4. PaymentService
**Type**: Console application
**Responsibilities**:
- Subscribe to OrderCreated events
- Process payments (simulated)
- Publish PaymentProcessed events

### 5. InventoryService
**Type**: Console application
**Responsibilities**:
- Subscribe to OrderCreated events
- Reserve inventory (simulated)
- Publish InventoryReserved events

### 6. NotificationService
**Type**: Console application
**Responsibilities**:
- Subscribe to all events
- Send notifications (logged to console)
- Tracks order lifecycle

## Event Flow
``markdown
┌──────────────┐
│   Client     │
└──────┬───────┘
       │ POST /api/orders
       ▼
┌────────────────────┐
│   OrderService     │
│   (REST API)       │
└────────┬───────────┘
         │ Publish: OrderCreated
         ▼
┌─────────────────────────────────────┐
│         MQTT Broker (Mosquitto)      │
│     Topic: orders/created           │
└──┬──────────────────┬───────────────┘
   │                  │
   │                  └──────────────────┐
   ▼                                   ▼
┌──────────────────┐          ┌────────────────────┐
│ PaymentService   │          │ InventoryService   │
│ Subscribe        │          │ Subscribe          │
└────────┬─────────┘          └─────────┬──────────┘
         │                              │
         │ Publish                      │ Publish
         │ PaymentProcessed             │ InventoryReserved
         ▼                              ▼
┌───────────────────────────────────────────────┐
│           MQTT Broker                          │
└────────────────────┬──────────────────────────┘
                     │
                     ▼
          ┌──────────────────────┐
          │ NotificationService  │
          │ Subscribe to all     │
          └──────────────────────┘
``n
## MQTT Topics
- orders/created - New order events
- payments/processed - Payment completion events
- inventory/reserved - Inventory reservation events

## Technologies Used
- **.NET 8.0**: Modern C# features and performance
- **MQTTnet 4.3.3**: MQTT client library
- **Eclipse Mosquitto 2.0**: MQTT broker
- **Docker**: Container orchestration
- **Minimal APIs**: Lightweight REST endpoints

## Design Patterns Demonstrated
1. **Event-Driven Architecture**: Loose coupling between services
2. **Publish-Subscribe Pattern**: MQTT topics for event distribution
3. **Microservices**: Independent, scalable services
4. **Domain Events**: Business-meaningful event names
5. **Asynchronous Processing**: Non-blocking event handling

## Best Practices Implemented
1. **Clear topic hierarchy**: Logical organization
2. **Typed events**: Strongly-typed event classes
3. **JSON serialization**: Human-readable payloads
4. **Async/await**: Proper async patterns
5. **Dependency Injection**: Service registration
6. **Logging**: Console output for visibility

## Development Tips
- Each service logs events for easy debugging
- MQTT broker logs available via docker logs mqtt-broker`n- Swagger UI available at http://localhost:5000/swagger
- Services can be stopped/started independently
- MQTT messages are visible with MQTT client tools

## Learning Path
1. **Week 1**: Study docs/Interview-Guide.md theory
2. **Week 2**: Run and modify the POC code
3. **Week 3**: Practice system design scenarios
4. **Week 4**: Mock interviews and refinement

## Extension Ideas
1. Add error handling and retries
2. Implement dead-letter queues
3. Add correlation IDs for tracing
4. Implement the Saga pattern
5. Add health checks
6. Implement event sourcing
7. Add API authentication
8. Create a dashboard service

## Interview Focus Areas
- Explain the event flow
- Discuss scaling strategies
- Handle failure scenarios
- Compare with alternatives (RabbitMQ, Kafka, etc.)
- Explain QoS levels and when to use them
- Discuss idempotency

Good luck! 🚀