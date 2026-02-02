# Event-Driven Architecture & MQTT - Interview Guide

## Table of Contents
1. [Core Concepts](#core-concepts)
2. [MQTT Protocol](#mqtt-protocol)
3. [System Design Patterns](#system-design-patterns)
4. [Interview Questions & Answers](#interview-qa)
5. [Practice Scenarios](#practice-scenarios)

## Core Concepts

### What is Event-Driven Architecture?
Event-Driven Architecture (EDA) is a software architecture paradigm where the flow of the program is determined by events - significant changes in state.

**Key Characteristics:**
- **Loose Coupling**: Services don't need to know about each other
- **Asynchronous**: Events are processed asynchronously
- **Scalability**: Easy to scale individual components
- **Real-time Processing**: Immediate response to events

### Why Use EDA?
- Decouples microservices
- Enables real-time data processing
- Improves scalability and resilience
- Supports complex event processing

## MQTT Protocol

### What is MQTT?
MQTT (Message Queuing Telemetry Transport) is a lightweight, publish-subscribe network protocol for IoT and microservices.

**Key Features:**
- Lightweight (minimal packet overhead)
- Publish/Subscribe model
- QoS levels (0, 1, 2)
- Retained messages
- Last Will and Testament

### MQTT vs Other Protocols
| Feature | MQTT | HTTP | WebSockets | RabbitMQ |
|---------|------|------|------------|----------|
| Model | Pub/Sub | Request/Response | Full-Duplex | Multiple |
| Overhead | Low | High | Medium | Medium |
| QoS | Yes | No | No | Yes |
| Best For | IoT, Real-time | REST APIs | Real-time Web | Enterprise |

## System Design Patterns

### 1. Event Sourcing
Store all changes as sequence of events rather than current state.

### 2. CQRS (Command Query Responsibility Segregation)
Separate read and write operations for better performance and scalability.

### 3. Saga Pattern
Manage distributed transactions across multiple services using events.

### 4. Event Notification
Services publish events when something interesting happens.

## Interview Q&A

### Q1: Explain the difference between Event-Driven and Request-Response
**Answer:** Request-Response is synchronous - client waits for response. Event-Driven is asynchronous - publishers emit events without waiting. EDA provides better decoupling and scalability.

### Q2: What are MQTT QoS levels?
**Answer:**
- **QoS 0**: At most once (fire and forget)
- **QoS 1**: At least once (acknowledged delivery)
- **QoS 2**: Exactly once (4-way handshake)

### Q3: How do you handle event ordering?
**Answer:** Use message ordering keys, sequence numbers, or timestamps. Consider using ordered topics or partition keys in distributed systems.

### Q4: What is the Saga pattern?
**Answer:** The Saga pattern manages distributed transactions using a sequence of local transactions coordinated through events. If one fails, compensating transactions are triggered.

### Q5: How do you ensure exactly-once delivery?
**Answer:** Use idempotency keys, database transactions, and deduplication. MQTT QoS 2 helps, but application-level idempotency is crucial.

## Practice Scenarios

### Scenario 1: E-commerce Order Processing
Design an event-driven system for order processing with payment, inventory, and shipping services.

**Solution:**
1. OrderService publishes OrderCreated event
2. PaymentService processes payment, publishes PaymentProcessed
3. InventoryService reserves items, publishes InventoryReserved
4. ShippingService creates shipment, publishes ShipmentCreated
5. NotificationService sends emails for each stage

### Scenario 2: IoT Sensor Network
Design a system for thousands of sensors sending temperature data.

**Solution:**
- Use MQTT with hierarchical topics (sensors/{location}/{deviceId}/temperature)
- Implement retained messages for last known state
- Use QoS 1 for important alerts
- Aggregate data using stream processing

## Best Practices
1. Use clear, hierarchical topic structures
2. Implement idempotent event handlers
3. Add correlation IDs for tracing
4. Use dead letter queues for failed events
5. Monitor event lag and processing times
6. Version your event schemas
7. Implement circuit breakers
8. Use appropriate QoS levels

## Common Pitfalls
1. **Event Storm**: Too many fine-grained events
2. **Tight Coupling**: Events contain too much domain knowledge
3. **No Ordering**: Events processed out of order
4. **No Idempotency**: Duplicate processing causes issues
5. **Poor Error Handling**: No retry or compensation logic

## Additional Resources
- [MQTT.org](https://mqtt.org)
- [Martin Fowler - Event-Driven Architecture](https://martinfowler.com/articles/201701-event-driven.html)
- [Microservices Patterns Book](https://microservices.io/patterns)

Good luck with your interview! 🚀