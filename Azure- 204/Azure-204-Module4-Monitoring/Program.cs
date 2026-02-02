using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using AZ204.Monitoring.ApplicationInsights;
using AZ204.Monitoring.Cache;
using AZ204.Monitoring.CDN;

namespace AZ204.Monitoring;

class Program
{
    static async Task Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((context, config) =>
            {
                config.AddJsonFile("appsettings.json", optional: true);
                config.AddEnvironmentVariables();
                config.AddUserSecrets<Program>(optional: true);
            })
            .ConfigureServices((context, services) =>
            {
                services.AddLogging(builder =>
                {
                    builder.AddConsole();
                    builder.AddDebug();
                });
            })
            .Build();

        var logger = host.Services.GetRequiredService<ILogger<Program>>();
        var configuration = host.Services.GetRequiredService<IConfiguration>();

        Console.WriteLine("???????????????????????????????????????????????????????????????");
        Console.WriteLine("  AZ-204 Module 4: Monitor, Troubleshoot, and Optimize");
        Console.WriteLine("  Real Azure Examples - Application Insights, Cache, CDN");
        Console.WriteLine("???????????????????????????????????????????????????????????????");
        Console.WriteLine();

        while (true)
        {
            DisplayMenu();
            var choice = Console.ReadLine()?.Trim();

            try
            {
                switch (choice)
                {
                    case "1":
                        await ApplicationInsightsMenu(logger, configuration);
                        break;
                    case "2":
                        await RedisCacheMenu(logger, configuration);
                        break;
                    case "3":
                        await CdnMenu(logger, configuration);
                        break;
                    case "4":
                        DisplayBestPractices();
                        break;
                    case "5":
                        DisplayExamTips();
                        break;
                    case "0":
                        Console.WriteLine("\n?? Goodbye!");
                        return;
                    default:
                        Console.WriteLine("\n? Invalid choice. Please try again.\n");
                        break;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred");
                Console.WriteLine($"\n? Error: {ex.Message}\n");
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
            Console.Clear();
        }
    }

    static void DisplayMenu()
    {
        Console.WriteLine("\n???????????????????????????????????????????????????????????????");
        Console.WriteLine("  MAIN MENU - Select a topic:");
        Console.WriteLine("???????????????????????????????????????????????????????????????");
        Console.WriteLine("  1. Application Insights (Telemetry & Monitoring)");
        Console.WriteLine("  2. Azure Cache for Redis (Performance)");
        Console.WriteLine("  3. Azure CDN (Content Delivery)");
        Console.WriteLine("???????????????????????????????????????????????????????????????");
        Console.WriteLine("  4. Best Practices & Patterns");
        Console.WriteLine("  5. AZ-204 Exam Tips");
        Console.WriteLine("???????????????????????????????????????????????????????????????");
        Console.WriteLine("  0. Exit");
        Console.WriteLine("???????????????????????????????????????????????????????????????");
        Console.Write("\nEnter your choice: ");
    }

    static async Task ApplicationInsightsMenu(ILogger<Program> logger, IConfiguration configuration)
    {
        Console.Clear();
        Console.WriteLine("???????????????????????????????????????????????????????????????");
        Console.WriteLine("  APPLICATION INSIGHTS - TELEMETRY & MONITORING");
        Console.WriteLine("???????????????????????????????????????????????????????????????\n");

        while (true)
        {
            Console.WriteLine("\n???????????????????????????????????????????????????????????????");
            Console.WriteLine("  APPLICATION INSIGHTS OPTIONS:");
            Console.WriteLine("???????????????????????????????????????????????????????????????");
            Console.WriteLine("  1. Track Custom Events");
            Console.WriteLine("  2. Track Custom Metrics");
            Console.WriteLine("  3. Track HTTP Dependencies");
            Console.WriteLine("  4. Track Exceptions");
            Console.WriteLine("  5. Simulate Business Transaction (Complete Example)");
            Console.WriteLine("  6. Show KQL Query Examples");
            Console.WriteLine("  7. Query Application Insights (Requires Config)");
            Console.WriteLine("  0. Back to Main Menu");
            Console.WriteLine("???????????????????????????????????????????????????????????????");
            Console.Write("\nEnter your choice: ");

            var choice = Console.ReadLine()?.Trim();

            if (choice == "0") return;

            var instrumentationKey = configuration["ApplicationInsights:InstrumentationKey"];
            
            if (string.IsNullOrEmpty(instrumentationKey) && choice != "6")
            {
                Console.WriteLine("\n??  Application Insights not configured!");
                Console.WriteLine("\nTo use Application Insights, add to appsettings.json:");
                Console.WriteLine("{\n  \"ApplicationInsights\": {");
                Console.WriteLine("    \"InstrumentationKey\": \"your-instrumentation-key\"");
                Console.WriteLine("  }\n}");
                Console.WriteLine("\nNote: Telemetry will still be tracked locally for demonstration.");
                instrumentationKey = "demo-key-" + Guid.NewGuid().ToString();
            }

            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            var appInsightsLogger = loggerFactory.CreateLogger<AppInsightsTelemetryService>();
            var telemetryService = new AppInsightsTelemetryService(instrumentationKey!, appInsightsLogger);

            try
            {
                switch (choice)
                {
                    case "1":
                        Console.Clear();
                        Console.WriteLine("?? Tracking Custom Events\n");
                        telemetryService.TrackCustomEvent("UserLogin", new Dictionary<string, string>
                        {
                            { "UserId", "user123" },
                            { "LoginMethod", "OAuth" },
                            { "Location", "US-East" }
                        });
                        telemetryService.TrackCustomEvent("PageView", new Dictionary<string, string>
                        {
                            { "Page", "ProductCatalog" },
                            { "Category", "Electronics" }
                        });
                        telemetryService.Flush();
                        break;

                    case "2":
                        Console.Clear();
                        Console.WriteLine("?? Tracking Custom Metrics\n");
                        telemetryService.TrackMetric("ResponseTime", 245.5, new Dictionary<string, string>
                        {
                            { "Endpoint", "/api/products" }
                        });
                        telemetryService.TrackMetric("QueueLength", 42);
                        telemetryService.TrackMetric("CacheHitRate", 85.5);
                        telemetryService.Flush();
                        break;

                    case "3":
                        Console.Clear();
                        Console.WriteLine("?? Tracking HTTP Dependencies\n");
                        await telemetryService.TrackHttpDependencyAsync("https://jsonplaceholder.typicode.com/posts/1");
                        await telemetryService.TrackHttpDependencyAsync("https://api.github.com");
                        telemetryService.Flush();
                        break;

                    case "4":
                        Console.Clear();
                        Console.WriteLine("??  Tracking Exceptions\n");
                        try
                        {
                            throw new InvalidOperationException("Simulated error for demonstration");
                        }
                        catch (Exception ex)
                        {
                            telemetryService.TrackException(ex, new Dictionary<string, string>
                            {
                                { "Operation", "ProcessPayment" },
                                { "OrderId", "ORD-123" }
                            });
                        }
                        telemetryService.Flush();
                        break;

                    case "5":
                        Console.Clear();
                        Console.WriteLine("?? Simulating Complete Business Transaction\n");
                        var orderId = $"ORD-{Random.Shared.Next(1000, 9999)}";
                        var result = await telemetryService.SimulateBusinessTransactionAsync(orderId);
                        Console.WriteLine($"\n? {result}");
                        telemetryService.Flush();
                        break;

                    case "6":
                        Console.Clear();
                        var queryLogger = loggerFactory.CreateLogger<AppInsightsQueryService>();
                        var queryService = new AppInsightsQueryService(queryLogger);
                        queryService.ShowKqlExamples();
                        break;

                    case "7":
                        Console.Clear();
                        var workspaceId = configuration["ApplicationInsights:WorkspaceId"];
                        if (string.IsNullOrEmpty(workspaceId))
                        {
                            Console.WriteLine("? Workspace ID not configured in appsettings.json");
                            break;
                        }
                        var queryLogger2 = loggerFactory.CreateLogger<AppInsightsQueryService>();
                        var queryService2 = new AppInsightsQueryService(queryLogger2);
                        await queryService2.GetPerformanceMetricsAsync(workspaceId);
                        break;

                    default:
                        Console.WriteLine("\n? Invalid choice.");
                        break;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Application Insights operation failed");
                Console.WriteLine($"\n? Error: {ex.Message}");
            }
        }
    }

    static async Task RedisCacheMenu(ILogger<Program> logger, IConfiguration configuration)
    {
        Console.Clear();
        Console.WriteLine("???????????????????????????????????????????????????????????????");
        Console.WriteLine("  AZURE CACHE FOR REDIS - PERFORMANCE OPTIMIZATION");
        Console.WriteLine("???????????????????????????????????????????????????????????????\n");

        var connectionString = configuration["Redis:ConnectionString"];
        
        if (string.IsNullOrEmpty(connectionString))
        {
            Console.WriteLine("??  Redis Cache not configured!");
            Console.WriteLine("\nTo use Redis Cache, add to appsettings.json:");
            Console.WriteLine("{\n  \"Redis\": {");
            Console.WriteLine("    \"ConnectionString\": \"your-cache-name.redis.cache.windows.net:6380,password=your-password,ssl=True\"");
            Console.WriteLine("  }\n}");
            Console.WriteLine("\nPress any key to return...");
            Console.ReadKey();
            return;
        }

        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        var redisLogger = loggerFactory.CreateLogger<RedisCacheService>();
        
        RedisCacheService? cacheService = null;
        
        try
        {
            cacheService = new RedisCacheService(connectionString, redisLogger);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"? Failed to connect to Redis: {ex.Message}");
            Console.WriteLine("\nPress any key to return...");
            Console.ReadKey();
            return;
        }

        while (true)
        {
            Console.WriteLine("\n???????????????????????????????????????????????????????????????");
            Console.WriteLine("  REDIS CACHE OPTIONS:");
            Console.WriteLine("???????????????????????????????????????????????????????????????");
            Console.WriteLine("  1. Cache-Aside Pattern (Get Product)");
            Console.WriteLine("  2. Set Cache Value");
            Console.WriteLine("  3. Get Cache Value");
            Console.WriteLine("  4. Delete Cache Value");
            Console.WriteLine("  5. Rate Limiting Demo");
            Console.WriteLine("  6. Session Management Demo");
            Console.WriteLine("  7. View Cache Statistics");
            Console.WriteLine("  0. Back to Main Menu");
            Console.WriteLine("???????????????????????????????????????????????????????????????");
            Console.Write("\nEnter your choice: ");

            var choice = Console.ReadLine()?.Trim();

            if (choice == "0")
            {
                cacheService?.Dispose();
                return;
            }

            try
            {
                switch (choice)
                {
                    case "1":
                        Console.Clear();
                        Console.WriteLine("?? Cache-Aside Pattern: Getting Product\n");
                        
                        // First call - cache miss
                        Console.WriteLine("First call (should be cache MISS):");
                        var product1 = await cacheService!.GetProductAsync(101);
                        Console.WriteLine($"\nProduct: {product1.Name}, Price: ${product1.Price}");
                        
                        Console.WriteLine("\n" + new string('?', 50));
                        
                        // Second call - cache hit
                        Console.WriteLine("\nSecond call (should be cache HIT):");
                        var product2 = await cacheService.GetProductAsync(101);
                        Console.WriteLine($"\nProduct: {product2.Name}, Price: ${product2.Price}");
                        break;

                    case "2":
                        Console.Clear();
                        Console.WriteLine("?? Setting Cache Value\n");
                        await cacheService!.SetAsync("test-key", new { Message = "Hello from Redis!", Timestamp = DateTime.UtcNow }, TimeSpan.FromMinutes(5));
                        break;

                    case "3":
                        Console.Clear();
                        Console.WriteLine("?? Getting Cache Value\n");
                        Console.Write("Enter key to retrieve: ");
                        var key = Console.ReadLine();
                        var value = await cacheService!.GetAsync<dynamic>(key!);
                        if (value != null)
                        {
                            Console.WriteLine($"\nValue: {value}");
                        }
                        break;

                    case "4":
                        Console.Clear();
                        Console.WriteLine("???  Deleting Cache Value\n");
                        Console.Write("Enter key to delete: ");
                        var deleteKey = Console.ReadLine();
                        await cacheService!.DeleteAsync(deleteKey!);
                        break;

                    case "5":
                        Console.Clear();
                        Console.WriteLine("?? Rate Limiting Demo\n");
                        var userId = "user-" + Random.Shared.Next(1, 5);
                        Console.WriteLine($"Simulating 10 requests for {userId} (limit: 5/minute)\n");
                        
                        for (int i = 1; i <= 10; i++)
                        {
                            var allowed = await cacheService!.CheckRateLimitAsync(userId, maxRequests: 5);
                            await Task.Delay(200);
                        }
                        break;

                    case "6":
                        Console.Clear();
                        Console.WriteLine("?? Session Management Demo\n");
                        var sessionId = Guid.NewGuid().ToString();
                        var session = new UserSession
                        {
                            UserId = "user123",
                            UserName = "John Doe",
                            LoginTime = DateTime.UtcNow,
                            Properties = new Dictionary<string, string>
                            {
                                { "Role", "Admin" },
                                { "Department", "IT" }
                            }
                        };
                        await cacheService!.ManageSessionAsync(sessionId, session);
                        Console.WriteLine($"Session ID: {sessionId}");
                        break;

                    case "7":
                        Console.Clear();
                        await cacheService!.GetCacheStatsAsync();
                        break;

                    default:
                        Console.WriteLine("\n? Invalid choice.");
                        break;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Redis operation failed");
                Console.WriteLine($"\n? Error: {ex.Message}");
            }
        }
    }

    static async Task CdnMenu(ILogger<Program> logger, IConfiguration configuration)
    {
        Console.Clear();
        Console.WriteLine("???????????????????????????????????????????????????????????????");
        Console.WriteLine("  AZURE CDN - CONTENT DELIVERY NETWORK");
        Console.WriteLine("???????????????????????????????????????????????????????????????\n");

        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        var cdnLogger = loggerFactory.CreateLogger<CdnManagementService>();
        var cdnService = new CdnManagementService(cdnLogger);

        while (true)
        {
            Console.WriteLine("\n???????????????????????????????????????????????????????????????");
            Console.WriteLine("  CDN OPTIONS:");
            Console.WriteLine("???????????????????????????????????????????????????????????????");
            Console.WriteLine("  1. Create CDN Profile");
            Console.WriteLine("  2. Create CDN Endpoint");
            Console.WriteLine("  3. Purge CDN Content");
            Console.WriteLine("  4. Get CDN Statistics");
            Console.WriteLine("  5. List CDN Endpoints");
            Console.WriteLine("  6. Show CDN Best Practices");
            Console.WriteLine("  7. Show CDN Configuration Scenarios");
            Console.WriteLine("  0. Back to Main Menu");
            Console.WriteLine("???????????????????????????????????????????????????????????????");
            Console.Write("\nEnter your choice: ");

            var choice = Console.ReadLine()?.Trim();

            if (choice == "0") return;

            try
            {
                switch (choice)
                {
                    case "1":
                    case "2":
                    case "3":
                    case "4":
                    case "5":
                        Console.Clear();
                        Console.WriteLine("??  This operation requires Azure credentials and configuration.");
                        Console.WriteLine("\nRequired in appsettings.json:");
                        Console.WriteLine("{\n  \"Azure\": {");
                        Console.WriteLine("    \"SubscriptionId\": \"your-subscription-id\",");
                        Console.WriteLine("    \"ResourceGroup\": \"your-resource-group\",");
                        Console.WriteLine("    \"CdnProfileName\": \"your-cdn-profile\"");
                        Console.WriteLine("  }\n}");
                        break;

                    case "6":
                        Console.Clear();
                        cdnService.ShowCdnBestPractices();
                        break;

                    case "7":
                        Console.Clear();
                        cdnService.ShowCdnConfigurations();
                        break;

                    default:
                        Console.WriteLine("\n? Invalid choice.");
                        break;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "CDN operation failed");
                Console.WriteLine($"\n? Error: {ex.Message}");
            }

            await Task.CompletedTask;
        }
    }

    static void DisplayBestPractices()
    {
        Console.Clear();
        Console.WriteLine("???????????????????????????????????????????????????????????????");
        Console.WriteLine("  MONITORING BEST PRACTICES");
        Console.WriteLine("???????????????????????????????????????????????????????????????\n");

        Console.WriteLine("?? APPLICATION INSIGHTS:");
        Console.WriteLine("  ? Track custom events for business metrics");
        Console.WriteLine("  ? Use dependency tracking for external calls");
        Console.WriteLine("  ? Implement distributed tracing with operation IDs");
        Console.WriteLine("  ? Set appropriate sampling rates in production");
        Console.WriteLine("  ? Create custom dashboards in Azure Portal");
        Console.WriteLine("  ? Configure alerts for critical metrics\n");

        Console.WriteLine("?? REDIS CACHE:");
        Console.WriteLine("  ? Use cache-aside pattern for read-heavy workloads");
        Console.WriteLine("  ? Set appropriate TTL based on data freshness");
        Console.WriteLine("  ? Implement connection pooling (handled by StackExchange.Redis)");
        Console.WriteLine("  ? Use async operations for better performance");
        Console.WriteLine("  ? Monitor cache hit rate and adjust strategy");
        Console.WriteLine("  ? Handle Redis connection failures gracefully\n");

        Console.WriteLine("?? CDN:");
        Console.WriteLine("  ? Enable compression for text-based content");
        Console.WriteLine("  ? Use HTTPS everywhere");
        Console.WriteLine("  ? Set long TTL for static assets");
        Console.WriteLine("  ? Use query strings for cache versioning");
        Console.WriteLine("  ? Implement proper cache headers");
        Console.WriteLine("  ? Purge cache after deployments\n");

        Console.WriteLine("?? GENERAL:");
        Console.WriteLine("  ? Monitor all three: logs, metrics, and traces");
        Console.WriteLine("  ? Use structured logging");
        Console.WriteLine("  ? Implement health check endpoints");
        Console.WriteLine("  ? Set up automated alerts");
        Console.WriteLine("  ? Create runbooks for common issues");
        Console.WriteLine("  ? Review monitoring costs regularly");
    }

    static void DisplayExamTips()
    {
        Console.Clear();
        Console.WriteLine("???????????????????????????????????????????????????????????????");
        Console.WriteLine("  AZ-204 EXAM TIPS - MODULE 4");
        Console.WriteLine("???????????????????????????????????????????????????????????????\n");

        Console.WriteLine("?? KEY EXAM TOPICS:\n");

        Console.WriteLine("1. Application Insights:");
        Console.WriteLine("   • Know how to instrument apps (SDK vs auto-instrumentation)");
        Console.WriteLine("   • Understand KQL queries for log analytics");
        Console.WriteLine("   • Custom telemetry: events, metrics, dependencies");
        Console.WriteLine("   • Distributed tracing concepts\n");

        Console.WriteLine("2. Azure Cache for Redis:");
        Console.WriteLine("   • Cache-aside pattern (lazy loading)");
        Console.WriteLine("   • Write-through vs write-behind");
        Console.WriteLine("   • Redis data types (strings, lists, sets, hashes)");
        Console.WriteLine("   • Clustering and persistence options\n");

        Console.WriteLine("3. Azure CDN:");
        Console.WriteLine("   • CDN profiles and endpoints");
        Console.WriteLine("   • Caching behavior and rules");
        Console.WriteLine("   • Purging content");
        Console.WriteLine("   • Geo-filtering and HTTPS\n");

        Console.WriteLine("4. Performance Optimization:");
        Console.WriteLine("   • Retry policies (exponential backoff)");
        Console.WriteLine("   • Circuit breaker pattern");
        Console.WriteLine("   • Async/await best practices");
        Console.WriteLine("   • Connection pooling\n");

        Console.WriteLine("?? EXAM SCENARIOS:\n");
        Console.WriteLine("Q: App has slow database queries. How to improve?");
        Console.WriteLine("A: Implement cache-aside pattern with Redis Cache\n");

        Console.WriteLine("Q: Need to track custom business events.");
        Console.WriteLine("A: Use Application Insights TrackEvent() method\n");

        Console.WriteLine("Q: Static website needs global distribution.");
        Console.WriteLine("A: Use Azure CDN with blob storage origin\n");

        Console.WriteLine("Q: How to query Application Insights logs?");
        Console.WriteLine("A: Use KQL (Kusto Query Language) in Log Analytics\n");
    }
}
