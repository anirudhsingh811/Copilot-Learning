using Shared;
using Shared.Events;
using System.Text.Json;

Console.WriteLine("Payment Service Starting...");
var mqtt = new MqttClientHelper();
await mqtt.ConnectAsync();

await mqtt.SubscribeAsync("orders/created", async (payload) =>
{
    var orderEvent = JsonSerializer.Deserialize<OrderCreatedEvent>(payload);
    Console.WriteLine($"[Payment] Processing payment for Order {orderEvent?.OrderId}");
    await Task.Delay(1000);
    var paymentEvent = new PaymentProcessedEvent
    {
        OrderId = orderEvent?.OrderId ?? string.Empty,
        PaymentId = Guid.NewGuid().ToString(),
        Amount = orderEvent?.TotalAmount ?? 0,
        Success = true,
        ProcessedAt = DateTime.UtcNow
    };
    await mqtt.PublishAsync("payments/processed", paymentEvent);
    Console.WriteLine($"[Payment] Payment successful for Order {orderEvent?.OrderId}");
});

Console.WriteLine("Payment Service Running. Press Ctrl+C to exit.");
await Task.Delay(Timeout.Infinite);