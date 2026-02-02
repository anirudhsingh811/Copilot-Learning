using Azure.Identity;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace AZ204.Monitoring.ApplicationInsights;

/// <summary>
/// Real Application Insights implementation for telemetry collection
/// Demonstrates custom events, metrics, dependencies, and distributed tracing
/// </summary>
public class AppInsightsTelemetryService
{
    private readonly TelemetryClient _telemetryClient;
    private readonly ILogger<AppInsightsTelemetryService> _logger;

    public AppInsightsTelemetryService(
        string instrumentationKey,
        ILogger<AppInsightsTelemetryService> logger)
    {
        _logger = logger;
        
        var configuration = TelemetryConfiguration.CreateDefault();
        configuration.ConnectionString = $"InstrumentationKey={instrumentationKey}";
        
        _telemetryClient = new TelemetryClient(configuration);
        _telemetryClient.Context.Cloud.RoleName = "AZ204-Monitoring-Demo";
        _telemetryClient.Context.Component.Version = "1.0.0";
    }

    /// <summary>
    /// Track custom event with properties
    /// </summary>
    public void TrackCustomEvent(string eventName, Dictionary<string, string>? properties = null)
    {
        _logger.LogInformation("Tracking event: {EventName}", eventName);
        
        var eventTelemetry = new EventTelemetry(eventName);
        
        if (properties != null)
        {
            foreach (var prop in properties)
            {
                eventTelemetry.Properties[prop.Key] = prop.Value;
            }
        }
        
        _telemetryClient.TrackEvent(eventTelemetry);
        Console.WriteLine($"? Event tracked: {eventName}");
    }

    /// <summary>
    /// Track custom metric
    /// </summary>
    public void TrackMetric(string metricName, double value, Dictionary<string, string>? properties = null)
    {
        _logger.LogInformation("Tracking metric: {MetricName} = {Value}", metricName, value);
        
        var metric = new MetricTelemetry(metricName, value);
        
        if (properties != null)
        {
            foreach (var prop in properties)
            {
                metric.Properties[prop.Key] = prop.Value;
            }
        }
        
        _telemetryClient.TrackMetric(metric);
        Console.WriteLine($"?? Metric tracked: {metricName} = {value}");
    }

    /// <summary>
    /// Track HTTP dependency (external API call)
    /// </summary>
    public async Task<string> TrackHttpDependencyAsync(string url)
    {
        var stopwatch = Stopwatch.StartNew();
        var success = true;
        var resultCode = "200";
        
        try
        {
            Console.WriteLine($"?? Calling external API: {url}");
            
            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(url);
            resultCode = ((int)response.StatusCode).ToString();
            success = response.IsSuccessStatusCode;
            
            return await response.Content.ReadAsStringAsync();
        }
        catch (Exception ex)
        {
            success = false;
            resultCode = "500";
            _logger.LogError(ex, "Dependency call failed");
            throw;
        }
        finally
        {
            stopwatch.Stop();
            
            var dependency = new DependencyTelemetry
            {
                Name = "External API Call",
                Target = new Uri(url).Host,
                Data = url,
                Duration = stopwatch.Elapsed,
                Success = success,
                ResultCode = resultCode,
                Type = "HTTP"
            };
            
            _telemetryClient.TrackDependency(dependency);
            
            var icon = success ? "?" : "?";
            Console.WriteLine($"{icon} Dependency tracked: {url} ({stopwatch.ElapsedMilliseconds}ms)");
        }
    }

    /// <summary>
    /// Track exception with custom properties
    /// </summary>
    public void TrackException(Exception exception, Dictionary<string, string>? properties = null)
    {
        _logger.LogError(exception, "Exception tracked");
        
        var exceptionTelemetry = new ExceptionTelemetry(exception);
        exceptionTelemetry.SeverityLevel = SeverityLevel.Error;
        
        if (properties != null)
        {
            foreach (var prop in properties)
            {
                exceptionTelemetry.Properties[prop.Key] = prop.Value;
            }
        }
        
        _telemetryClient.TrackException(exceptionTelemetry);
        Console.WriteLine($"??  Exception tracked: {exception.Message}");
    }

    /// <summary>
    /// Track page view (for web applications)
    /// </summary>
    public void TrackPageView(string pageName, TimeSpan duration)
    {
        var pageView = new PageViewTelemetry(pageName)
        {
            Duration = duration
        };
        
        _telemetryClient.TrackPageView(pageView);
        Console.WriteLine($"?? Page view tracked: {pageName} ({duration.TotalMilliseconds}ms)");
    }

    /// <summary>
    /// Track request (HTTP request telemetry)
    /// </summary>
    public void TrackRequest(string name, DateTimeOffset startTime, TimeSpan duration, string responseCode, bool success)
    {
        var request = new RequestTelemetry
        {
            Name = name,
            Timestamp = startTime,
            Duration = duration,
            ResponseCode = responseCode,
            Success = success
        };
        
        _telemetryClient.TrackRequest(request);
        
        var icon = success ? "?" : "?";
        Console.WriteLine($"{icon} Request tracked: {name} ({responseCode}) - {duration.TotalMilliseconds}ms");
    }

    /// <summary>
    /// Create a custom operation for distributed tracing
    /// </summary>
    public IOperationHolder<RequestTelemetry> StartOperation(string operationName)
    {
        _logger.LogInformation("Starting operation: {OperationName}", operationName);
        Console.WriteLine($"?? Starting operation: {operationName}");
        
        return _telemetryClient.StartOperation<RequestTelemetry>(operationName);
    }

    /// <summary>
    /// Stop an operation
    /// </summary>
    public void StopOperation(IOperationHolder<RequestTelemetry> operation, bool success = true)
    {
        operation.Telemetry.Success = success;
        _telemetryClient.StopOperation(operation);
        
        var icon = success ? "?" : "?";
        Console.WriteLine($"{icon} Operation completed: {operation.Telemetry.Name}");
    }

    /// <summary>
    /// Flush telemetry (important for console apps)
    /// </summary>
    public void Flush()
    {
        _telemetryClient.Flush();
        // Allow time for flushing
        Task.Delay(5000).Wait();
        Console.WriteLine("?? Telemetry flushed to Application Insights");
    }

    /// <summary>
    /// Demonstrate a complete business transaction with nested operations
    /// </summary>
    public async Task<string> SimulateBusinessTransactionAsync(string orderId)
    {
        using var operation = StartOperation("ProcessOrder");
        
        try
        {
            // Track order received event
            TrackCustomEvent("OrderReceived", new Dictionary<string, string>
            {
                { "OrderId", orderId },
                { "Channel", "WebApp" }
            });

            // Simulate database dependency
            await SimulateDatabaseCallAsync();

            // Simulate payment processing
            await SimulatePaymentProcessingAsync(orderId);

            // Simulate external API call
            await TrackHttpDependencyAsync("https://jsonplaceholder.typicode.com/posts/1");

            // Track success metric
            TrackMetric("OrderProcessingTime", 2500, new Dictionary<string, string>
            {
                { "OrderId", orderId }
            });

            StopOperation(operation, success: true);

            return $"Order {orderId} processed successfully";
        }
        catch (Exception ex)
        {
            TrackException(ex, new Dictionary<string, string>
            {
                { "OrderId", orderId },
                { "Stage", "Processing" }
            });
            
            StopOperation(operation, success: false);
            throw;
        }
    }

    private async Task SimulateDatabaseCallAsync()
    {
        var stopwatch = Stopwatch.StartNew();
        Console.WriteLine("???  Calling database...");
        
        await Task.Delay(Random.Shared.Next(100, 500));
        
        stopwatch.Stop();
        
        var dependency = new DependencyTelemetry
        {
            Name = "SQL Query",
            Target = "sql.database.azure.net",
            Data = "SELECT * FROM Orders WHERE OrderId = @orderId",
            Duration = stopwatch.Elapsed,
            Success = true,
            Type = "SQL"
        };
        
        _telemetryClient.TrackDependency(dependency);
        Console.WriteLine($"? Database call completed ({stopwatch.ElapsedMilliseconds}ms)");
    }

    private async Task SimulatePaymentProcessingAsync(string orderId)
    {
        var stopwatch = Stopwatch.StartNew();
        Console.WriteLine("?? Processing payment...");
        
        await Task.Delay(Random.Shared.Next(500, 1000));
        
        stopwatch.Stop();
        
        var dependency = new DependencyTelemetry
        {
            Name = "Payment Gateway",
            Target = "payment.stripe.com",
            Data = $"ProcessPayment for order {orderId}",
            Duration = stopwatch.Elapsed,
            Success = true,
            Type = "HTTP"
        };
        
        _telemetryClient.TrackDependency(dependency);
        Console.WriteLine($"? Payment processed ({stopwatch.ElapsedMilliseconds}ms)");
    }
}
