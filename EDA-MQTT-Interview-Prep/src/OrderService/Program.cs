using Shared;
using Shared.Events;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();// ANI - Does this single line add swagger support?
builder.Services.AddSingleton<MqttClientHelper>();//-->GetHashCode Registration of Mqtt 

var app = builder.Build();// -->GetHashCode Build the app
if (app.Environment.IsDevelopment()) { app.UseSwagger(); app.UseSwaggerUI(); }// -->GetHashCode Enable Swagger in development

var mqttClient = app.Services.GetRequiredService<MqttClientHelper>();// -->GetHashCode Get the MqttClientHelper service
await mqttClient.ConnectAsync();//-->make connection to MQTT broker
app.MapPost("/api/orders", async (CreateOrderRequest request, MqttClientHelper mqtt) => //-->Define POST endpoint for creating orders, mqtt injected
{
    var orderId = Guid.NewGuid().ToString();
    var orderEvent = new OrderCreatedEvent
    {
        OrderId = orderId,
        CustomerId = request.CustomerId,
        Items = request.Items.Select(i => new Shared.Events.OrderItem { ProductId = i.ProductId, Quantity = i.Quantity, Price = i.Price }).ToList(),
        TotalAmount = request.Items.Sum(i => i.Price * i.Quantity),
        CreatedAt = DateTime.UtcNow
    };
    await mqtt.PublishAsync("orders/created", orderEvent);//-->Publish order created event to MQTT topic
    return Results.Ok(new { OrderId = orderId, Message = "Order created successfully" });
});

app.Run();//-->Run the application

record CreateOrderRequest(string CustomerId, List<OrderItemDto> Items);//-->Define request DTO for creating orders
record OrderItemDto(string ProductId, int Quantity, decimal Price);