using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace AZ204.RealFunctions;
// Learning Summary: This code defines real queue-triggered Azure Functions using C# 12.0 features.
// It includes two functions: one for processing order messages and another for processing notification messages.
// The code demonstrates dependency injection for logging, queue message handling, JSON serialization, and output bindings to Azure Storage Queues.     

/// <summary>
/// Real Queue Triggered Azure Functions
/// Processes messages from Azure Storage Queues
/// </summary>
public class QueueTriggerFunctions
{
    private readonly ILogger<QueueTriggerFunctions> _logger;

    public QueueTriggerFunctions(ILogger<QueueTriggerFunctions> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Processes order messages from the queue
    /// Triggered when a message is added to "order-queue"
    /// </summary>
    [Function("ProcessOrderFromQueue")]// Function name
    public async Task ProcessOrder(
        [QueueTrigger("order-queue", Connection = "AzureWebJobsStorage")] string orderMessage,// Queue trigger binding
        FunctionContext context) // Function context for additional info
    {
        _logger.LogInformation($"?? Processing order from queue at: {DateTime.UtcNow}");
        _logger.LogInformation($"Message: {orderMessage}");

        try
        {
            // Deserialize the order
            var order = JsonSerializer.Deserialize<OrderMessage>(orderMessage);
            
            if (order == null)
            {
                _logger.LogError("? Failed to deserialize order message");
                return;
            }

            _logger.LogInformation($"?? Order ID: {order.OrderId}");
            _logger.LogInformation($"   Product: {order.ProductName}");
            _logger.LogInformation($"   Quantity: {order.Quantity}");
            _logger.LogInformation($"   Total: ${order.TotalAmount:N2}");
            _logger.LogInformation($"   Customer: {order.CustomerEmail}");

            // Step 1: Validate inventory
            _logger.LogInformation("? Step 1: Validating inventory...");
            await Task.Delay(500);
            _logger.LogInformation("   ? Inventory available");

            // Step 2: Reserve items
            _logger.LogInformation("? Step 2: Reserving items...");
            await Task.Delay(500);
            _logger.LogInformation($"   ? Reserved {order.Quantity} units");

            // Step 3: Process payment
            _logger.LogInformation("? Step 3: Processing payment...");
            await Task.Delay(1000);
            _logger.LogInformation($"   ? Payment of ${order.TotalAmount:N2} processed");

            // Step 4: Update order status
            _logger.LogInformation("? Step 4: Updating order status...");
            await Task.Delay(300);
            _logger.LogInformation("   ? Order status: Confirmed");

            // Step 5: Send confirmation email
            _logger.LogInformation("? Step 5: Sending confirmation email...");
            await Task.Delay(500);
            _logger.LogInformation($"   ? Email sent to {order.CustomerEmail}");

            _logger.LogInformation($"? Order {order.OrderId} processed successfully!");
            // Queue gets dequeued automatically on successful completion
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"? Failed to process order: {orderMessage}");
            throw; // This will move the message to poison queue after max retries. Max reties can be configured in host.json. 
        }
    }

    /// <summary>
    /// Processes notification messages
    /// Triggered when a message is added to "notifications-queue"
    /// </summary>
    [Function("ProcessNotification")]
    public async Task ProcessNotification(
        [QueueTrigger("notifications-queue", Connection = "AzureWebJobsStorage")] string notificationMessage)
    {
        _logger.LogInformation($"?? Processing notification at: {DateTime.UtcNow}");

        try
        {
            var notification = JsonSerializer.Deserialize<NotificationMessage>(notificationMessage);
            
            if (notification == null)
            {
                _logger.LogError("? Failed to deserialize notification");
                return;
            }

            _logger.LogInformation($"?? Notification ID: {notification.Id}");
            _logger.LogInformation($"   Type: {notification.Type}");
            _logger.LogInformation($"   Recipient: {notification.Recipient}");
            _logger.LogInformation($"   Subject: {notification.Subject}");

            // Send the notification
            await Task.Delay(500); // Simulate sending
            
            _logger.LogInformation($"? Notification sent to {notification.Recipient}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "? Failed to send notification");
            throw;
        }
    }

    /// <summary>
    /// Handles poison messages (failed messages after max retries)
    /// Triggered when a message is added to "order-queue-poison"
    /// </summary>
    [Function("HandlePoisonMessage")]
    public async Task HandlePoisonMessage(
        [QueueTrigger("order-queue-poison", Connection = "AzureWebJobsStorage")] string poisonMessage)
    {
        _logger.LogWarning($"??  Processing poison message at: {DateTime.UtcNow}");
        _logger.LogWarning($"Message: {poisonMessage}");

        try
        {
            // In a real app, you would:
            // 1. Log to Application Insights
            // 2. Send alert to operations team
            // 3. Store in dead-letter storage for manual review
            // 4. Attempt manual remediation
            // 5. Retry processing if applicable

            _logger.LogWarning("??  Alerting operations team...");
            await Task.Delay(200);

            _logger.LogWarning("??  Storing in dead-letter storage...");
            await Task.Delay(200);

            _logger.LogInformation("? Poison message handled");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "? Failed to handle poison message");
        }
    }

    /// <summary>
    /// Batch processes multiple messages at once from the batch-orders-queue
    /// Triggered when messages are added to "batch-orders-queue"
    /// Processes up to the configured batch size in a single execution
    /// More efficient for high-volume scenarios as it reduces function invocations
    /// Failed messages in the batch don't affect successful ones
    /// Returns success and failure counts for monitoring
    /// </summary>
    [Function("BatchProcessOrders")]
    public async Task BatchProcess(
        [QueueTrigger("batch-orders-queue", Connection = "AzureWebJobsStorage")] string[] messages)
    {
        _logger.LogInformation($"?? Batch processing {messages.Length} orders at: {DateTime.UtcNow}");

        var successCount = 0;
        var failCount = 0;

        foreach (var message in messages)
        {
            try
            {
                var order = JsonSerializer.Deserialize<OrderMessage>(message);
                
                // Process order
                await Task.Delay(100); // Simulate processing
                
                successCount++;
                _logger.LogInformation($"   ? Processed order {order?.OrderId}");
            }
            catch (Exception ex)
            {
                failCount++;
                _logger.LogError(ex, $"   ? Failed to process order");
            }
        }

        _logger.LogInformation($"?? Batch complete: {successCount} succeeded, {failCount} failed");
    }
}

// Message models
public record OrderMessage(
    string OrderId,
    string ProductName,
    int Quantity,
    decimal UnitPrice,
    decimal TotalAmount,
    string CustomerEmail,
    DateTime OrderDate,
    string Status
);

public record NotificationMessage(
    Guid Id,
    string Type,
    string Recipient,
    string Subject,
    DateTime CreatedAt
);
