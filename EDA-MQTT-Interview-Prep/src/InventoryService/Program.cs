using Shared;
using Shared.Events;
using System.Text.Json;

Console.WriteLine("Inventory Service Starting...");
var mqtt = new MqttClientHelper();// Uses default broker settings from MqttClientHelper
await mqtt.ConnectAsync();// Connect to MQTT broker

await mqtt.SubscribeAsync("orders/created", async (payload) => // Subscribe to order created events
{
    var orderEvent = JsonSerializer.Deserialize<OrderCreatedEvent>(payload);
    Console.WriteLine($"[Inventory] Reserving inventory for Order {orderEvent?.OrderId}");
    await Task.Delay(800);
    var inventoryEvent = new InventoryReservedEvent
    {
        OrderId = orderEvent?.OrderId ?? string.Empty,
        ReservationId = Guid.NewGuid().ToString(),
        Success = true,
        ReservedAt = DateTime.UtcNow
    };
    await mqtt.PublishAsync("inventory/reserved", inventoryEvent);// Publish inventory reserved event
    Console.WriteLine($"[Inventory] Reservation successful for Order {orderEvent?.OrderId}");
});

Console.WriteLine("Inventory Service Running. Press Ctrl+C to exit.");
await Task.Delay(Timeout.Infinite);// Keep the service running