using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;

// Learning Summary : This code defines real HTTP-triggered Azure Functions using C# 12.0 features.
// It includes three functions: a simple GET function that responds with a greeting message, a POST function that processes orders and queues them for further processing, and a GET function that retrieves the status of an order using route parameters.
// The code demonstrates dependency injection for logging, HTTP request handling, JSON serialization, and output bindings to Azure Storage Queues.
namespace AZ204.RealFunctions;

/// <summary>
/// Real HTTP Triggered Azure Functions
/// These are actual functions that can be deployed to Azure
/// </summary>
public class HttpTriggerFunctions
{
    private readonly ILogger<HttpTriggerFunctions> _logger; // Logger instance

    public HttpTriggerFunctions(ILogger<HttpTriggerFunctions> logger) // Constructor with DI and resolved at runtime in Azure
    {
        _logger = logger;
    }

    /// <summary>
    /// Simple HTTP GET function
    /// Endpoint: GET http://localhost:7071/api/hello?name=YourName
    /// </summary>
    [Function("HttpGetExample")]
    public async Task<HttpResponseData> HttpGet(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "hello")] HttpRequestData req) // HTTP trigger binding include request data, method, and route
    {
        _logger.LogInformation("HTTP GET request received");

        var query = System.Web.HttpUtility.ParseQueryString(req.Url.Query);
        var name = query["name"] ?? "World";

        var response = req.CreateResponse(HttpStatusCode.OK);// Create HTTP response

        var result = new
        {
            Message = $"Hello, {name}!",
            Timestamp = DateTime.UtcNow,
            Method = "GET"
        };

        await response.WriteAsJsonAsync(result);// Async write JSON to response
        return response;
    }

    /// <summary>
    /// HTTP POST function for order processing
    /// Endpoint: POST http://localhost:7071/api/orders
    /// Body: { "ProductName": "Widget", "Quantity": 5, "UnitPrice": 29.99, "CustomerEmail": "test@example.com" }
    /// </summary>
    [Function("ProcessOrder")]// Function name
    [QueueOutput("order-queue", Connection = "AzureWebJobsStorage")] // Output binding to Azure Storage Queue, Parameter- level binding
    public async Task<OrderProcessResponse> ProcessOrder(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "orders")] HttpRequestData req)// HTTP trigger binding
    {
        _logger.LogInformation("Processing order request");

        // Read request body
        var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        var order = JsonSerializer.Deserialize<OrderRequest>(requestBody);

        if (order == null || string.IsNullOrEmpty(order.ProductName))
        {
            throw new ArgumentException("Invalid order data");
        }

        // Generate order ID
        var orderId = Guid.NewGuid().ToString();
        var totalAmount = order.Quantity * order.UnitPrice;

        // Create order message
        var orderMessage = new
        {
            OrderId = orderId,
            ProductName = order.ProductName,
            Quantity = order.Quantity,
            UnitPrice = order.UnitPrice,
            TotalAmount = totalAmount,
            CustomerEmail = order.CustomerEmail,
            OrderDate = DateTime.UtcNow,
            Status = "Pending"
        };

        _logger.LogInformation($"Order {orderId} queued for processing");

        // Return both HTTP response and queue message
        return new OrderProcessResponse
        {
            HttpResponse = new OrderResponse(
                OrderId: orderId,
                Message: "Order received and queued for processing",
                TotalAmount: totalAmount,
                EstimatedProcessingTime: "2-5 minutes"
            ),
            QueueMessage = JsonSerializer.Serialize(orderMessage)//     Serialize order message to JSON for queue
        };
    }

    /// <summary>
    /// HTTP GET function with route parameters
    /// Endpoint: GET http://localhost:7071/api/orders/{orderId}
    /// </summary>
    [Function("GetOrderStatus")]// Function name
    public async Task<HttpResponseData> GetOrderStatus(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "orders/{orderId}")] HttpRequestData req,
        string orderId) // Route parameter binding
    {
        _logger.LogInformation($"Getting status for order {orderId}");

        // In a real app, you'd query a database
        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(new
        {
            OrderId = orderId,
            Status = "Processing",
            EstimatedCompletion = DateTime.UtcNow.AddMinutes(3),
            LastUpdated = DateTime.UtcNow
        });

        return response;
    }
}

public record OrderRequest(
    string ProductName,
    int Quantity,
    decimal UnitPrice,
    string CustomerEmail
);

public record OrderResponse(
    string OrderId,
    string Message,
    decimal TotalAmount,
    string EstimatedProcessingTime
);

public class OrderProcessResponse
{
    public OrderResponse HttpResponse { get; set; } = null!;
    public string QueueMessage { get; set; } = null!;
}
