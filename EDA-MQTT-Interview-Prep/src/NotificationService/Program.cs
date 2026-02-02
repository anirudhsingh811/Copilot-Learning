using Shared;
using Shared.Events;
using System.Text.Json;

Console.WriteLine("Notification Service Starting...");
var mqtt = new MqttClientHelper();
await mqtt.ConnectAsync();

await mqtt.SubscribeAsync("orders/created", async (payload) =>
{
    var orderEvent = JsonSerializer.Deserialize<OrderCreatedEvent>(payload);
    Console.WriteLine($"[Notification] Sending order confirmation email for Order {orderEvent?.OrderId} to customer {orderEvent?.CustomerId}");
    await Task.CompletedTask;
});

await mqtt.SubscribeAsync("payments/processed", async (payload) =>
{
    var paymentEvent = JsonSerializer.Deserialize<PaymentProcessedEvent>(payload);
    Console.WriteLine($"[Notification] Sending payment confirmation for Order {paymentEvent?.OrderId}");
    await Task.CompletedTask;
});

await mqtt.SubscribeAsync("inventory/reserved", async (payload) =>
{
    var inventoryEvent = JsonSerializer.Deserialize<InventoryReservedEvent>(payload);
    Console.WriteLine($"[Notification] Sending inventory confirmation for Order {inventoryEvent?.OrderId}");
    await Task.CompletedTask;
});

Console.WriteLine("Notification Service Running. Press Ctrl+C to exit.");
await Task.Delay(Timeout.Infinite);