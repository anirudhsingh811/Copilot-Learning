# Azure Monitoring Module - Real Examples for AZ-204

## ?? Overview

This module contains **real, working implementations** of Azure monitoring, caching, and content delivery services for AZ-204 exam preparation.

**Build Status:** ? **SUCCESS**

---

## ?? What's Included

### 1. **Application Insights** (`01-ApplicationInsights/`)

Real telemetry collection and monitoring implementation using the official Azure Application Insights SDK.

#### Features:
- ? **Custom Event Tracking** - Track business events
- ? **Custom Metrics** - Performance counters and KPIs
- ? **HTTP Dependency Tracking** - Monitor external API calls
- ? **Exception Tracking** - Capture and analyze errors
- ? **Distributed Tracing** - Operation correlation across services
- ? **KQL Queries** - Log Analytics query examples
- ? **Complete Business Transaction** - End-to-end example

#### Classes:
- `AppInsightsTelemetryService` - Main telemetry service
- `AppInsightsQueryService` - KQL query execution

---

### 2. **Azure Cache for Redis** (`02-Cache/`)

Real Redis Cache implementation using StackExchange.Redis client for performance optimization.

#### Features:
- ? **Cache-Aside Pattern** - Lazy loading with fallback
- ? **Set/Get/Delete** operations
- ? **TTL Management** - Expiration policies
- ? **Rate Limiting** - Request throttling
- ? **Session Management** - User session state
- ? **Cache Statistics** - Hit rate and performance metrics
- ? **Increment/Decrement** - Atomic counters

#### Class:
- `RedisCacheService` - Complete Redis operations

---

### 3. **Azure CDN** (`03-CDN/`)

Real Azure CDN management using Azure Resource Manager SDK.

#### Features:
- ? **CDN Profile Management** - Create and configure profiles
- ? **Endpoint Configuration** - Origins and caching rules
- ? **Content Purging** - Cache invalidation
- ? **Statistics** - Endpoint monitoring
- ? **Best Practices** - Configuration scenarios
- ? **Compression** - Performance optimization

#### Class:
- `CdnManagementService` - CDN operations

---

## ?? Quick Start

### 1. Run the Project

```bash
cd Azure-204-Module4-Monitoring
dotnet run
```

### 2. Interactive Menu

```
???????????????????????????????????????????????????????????
  MAIN MENU:
???????????????????????????????????????????????????????????
  1. Application Insights (Telemetry & Monitoring)
  2. Azure Cache for Redis (Performance)
  3. Azure CDN (Content Delivery)
  4. Best Practices & Patterns
  5. AZ-204 Exam Tips
  0. Exit
```

---

## ?? Configuration

### Option 1: Demo Mode (No Azure Resources)

Run without configuration - examples will work in demo mode for learning:

```bash
dotnet run
# Choose option 1 (Application Insights)
# Choose option 1-6 (various demos)
```

### Option 2: Real Azure Integration

Create `appsettings.json` (copy from `appsettings.template.json`):

```json
{
  "ApplicationInsights": {
    "InstrumentationKey": "your-key-here",
    "WorkspaceId": "your-workspace-id"
  },
  "Redis": {
    "ConnectionString": "your-cache.redis.cache.windows.net:6380,password=xxx,ssl=True"
  },
  "Azure": {
    "SubscriptionId": "your-subscription-id",
    "ResourceGroup": "your-rg",
    "CdnProfileName": "your-cdn-profile"
  }
}
```

---

## ?? Usage Examples

### Application Insights

#### Track Custom Events
```csharp
var telemetryService = new AppInsightsTelemetryService(instrumentationKey, logger);

telemetryService.TrackCustomEvent("UserLogin", new Dictionary<string, string>
{
    { "UserId", "user123" },
    { "LoginMethod", "OAuth" }
});
```

#### Track Metrics
```csharp
telemetryService.TrackMetric("ResponseTime", 245.5, new Dictionary<string, string>
{
    { "Endpoint", "/api/products" }
});
```

#### Track Dependencies
```csharp
await telemetryService.TrackHttpDependencyAsync("https://api.example.com/data");
```

#### Complete Business Transaction
```csharp
var result = await telemetryService.SimulateBusinessTransactionAsync("ORD-123");
telemetryService.Flush(); // Important for console apps!
```

---

### Redis Cache

#### Cache-Aside Pattern
```csharp
var cacheService = new RedisCacheService(connectionString, logger);

var product = await cacheService.GetOrCreateAsync(
    "product:123",
    async () => await FetchFromDatabase(123),
    expiration: TimeSpan.FromMinutes(10)
);
```

#### Rate Limiting
```csharp
var allowed = await cacheService.CheckRateLimitAsync(
    userId: "user123",
    maxRequests: 100,
    window: TimeSpan.FromMinutes(1)
);
```

#### Session Management
```csharp
var session = new UserSession
{
    UserId = "user123",
    UserName = "John Doe",
    LoginTime = DateTime.UtcNow
};

await cacheService.ManageSessionAsync(sessionId, session);
```

---

### CDN Management

#### Create CDN Profile
```csharp
var cdnService = new CdnManagementService(logger);

var profile = await cdnService.CreateCdnProfileAsync(
    subscriptionId,
    resourceGroupName,
    "my-cdn-profile",
    location: "global",
    skuName: "Standard_Microsoft"
);
```

#### Purge Content
```csharp
await cdnService.PurgeCdnContentAsync(
    subscriptionId,
    resourceGroupName,
    profileName,
    endpointName,
    new List<string> { "/css/*", "/js/*", "/images/logo.png" }
);
```

---

## ?? AZ-204 Exam Coverage

### ? Topics Covered

#### Application Insights (15-20% of exam)
- [x] Instrumenting applications
- [x] Custom telemetry (events, metrics, dependencies)
- [x] KQL (Kusto Query Language)
- [x] Distributed tracing
- [x] Log Analytics workspaces
- [x] Alerts and dashboards

#### Azure Cache for Redis (10-15% of exam)
- [x] Cache-aside pattern (lazy loading)
- [x] Write-through and write-behind strategies
- [x] TTL and expiration policies
- [x] Connection management
- [x] High availability and clustering
- [x] Performance optimization

#### Azure CDN (5-10% of exam)
- [x] CDN profiles and endpoints
- [x] Origin configuration
- [x] Caching rules and behaviors
- [x] Content purging
- [x] Compression
- [x] HTTPS and custom domains

---

## ?? Key Patterns Demonstrated

### 1. **Cache-Aside Pattern**
```
Application ? Check Cache ? Cache Hit? ? Return
                ? (Miss)
           Load from DB ? Store in Cache ? Return
```

### 2. **Telemetry Correlation**
```
Operation Start ? Track Events ? Track Dependencies ? Track Metrics ? Operation End
         ?
    Correlation ID (distributed tracing)
```

### 3. **Rate Limiting**
```
Request ? Check Counter in Redis ? Allow/Deny ? Increment Counter
                                        ?
                                  Auto-expire (TTL)
```

---

## ?? Real-World Scenarios

### Scenario 1: E-Commerce Application

**Problem:** Slow product catalog page load times

**Solution:**
```csharp
// Use cache-aside pattern
var products = await cacheService.GetOrCreateAsync(
    "products:category:electronics",
    async () => await database.GetProductsAsync("Electronics"),
    TimeSpan.FromMinutes(10)
);
```

**Result:** 90% reduction in database load, 5x faster response times

---

### Scenario 2: API Rate Limiting

**Problem:** Prevent API abuse and ensure fair usage

**Solution:**
```csharp
var allowed = await cacheService.CheckRateLimitAsync(
    userId,
    maxRequests: 1000,
    window: TimeSpan.FromHours(1)
);

if (!allowed)
{
    return StatusCode(429, "Too many requests");
}
```

---

### Scenario 3: Static Content Distribution

**Problem:** Global users experiencing slow image loads

**Solution:**
1. Upload images to Azure Blob Storage
2. Configure CDN endpoint with storage origin
3. Enable compression for supported file types
4. Set long cache TTL (1 year)

**Result:** 70% bandwidth reduction, faster global delivery

---

## ?? Performance Metrics

### Application Insights
- ? Tracks performance counters
- ? Monitors response times (P50, P95, P99)
- ? Detects performance anomalies
- ? Correlates across distributed systems

### Redis Cache
- ? Sub-millisecond response times
- ? 10-100x faster than database
- ? Supports millions of requests/second
- ? Hit rate typically 80-95%

### Azure CDN
- ? 99.9% uptime SLA
- ? Reduces origin load by 80-90%
- ? Global POP (Point of Presence) network
- ? HTTPS termination at edge

---

## ??? Troubleshooting

### Application Insights Not Showing Data

**Issue:** Telemetry not appearing in portal

**Solution:**
1. Check instrumentation key is correct
2. Call `Flush()` for console apps
3. Wait 2-5 minutes for ingestion
4. Verify firewall allows outbound HTTPS

```csharp
telemetryService.Flush();
await Task.Delay(5000); // Allow time for flushing
```

---

### Redis Connection Fails

**Issue:** `RedisConnectionException`

**Solution:**
1. Verify connection string format
2. Check firewall rules in Azure Portal
3. Ensure SSL port (6380) is used
4. Add `abortConnect=False` to connection string

```json
"Redis": {
  "ConnectionString": "cache.redis.cache.windows.net:6380,password=xxx,ssl=True,abortConnect=False"
}
```

---

### CDN Not Serving Updated Content

**Issue:** Old content still being served

**Solution:**
1. Purge CDN endpoint content
2. Check cache headers on origin
3. Verify TTL settings
4. Use query strings for versioning

```csharp
await cdnService.PurgeCdnContentAsync(
    subscriptionId, resourceGroup, profile, endpoint,
    new List<string> { "/*" } // Purge all
);
```

---

## ?? KQL Query Examples

### Failed Requests (Last Hour)
```kql
requests
| where timestamp > ago(1h)
| where success == false
| project timestamp, name, resultCode, duration
| order by timestamp desc
```

### Slow Dependencies
```kql
dependencies
| where timestamp > ago(24h)
| where duration > 2000
| summarize AvgDuration = avg(duration) by name, type
| order by AvgDuration desc
```

### Exception Trends
```kql
exceptions
| where timestamp > ago(7d)
| summarize ExceptionCount = count() by bin(timestamp, 1d), type
| render timechart
```

---

## ?? Best Practices

### Application Insights
- ? Use sampling in production (90-95%)
- ? Set meaningful operation names
- ? Include custom properties for context
- ? Implement correlation IDs
- ? Flush telemetry in console apps

### Redis Cache
- ? Handle cache failures gracefully
- ? Use connection multiplexing (StackExchange.Redis does this)
- ? Set appropriate TTL based on data freshness
- ? Monitor cache hit rate (target >80%)
- ? Use async operations

### CDN
- ? Enable compression for text files
- ? Set long TTL for static assets
- ? Use HTTPS everywhere
- ? Implement query string versioning
- ? Purge after deployments

---

## ?? Exam Tips

### Common Exam Questions

**Q: How to track custom business events?**  
A: Use `TelemetryClient.TrackEvent()` with custom properties

**Q: What pattern for caching database queries?**  
A: Cache-aside pattern (lazy loading with fallback)

**Q: How to invalidate CDN content?**  
A: Use PurgeContent API or Azure Portal

**Q: What's the difference between metrics and events?**  
A: Metrics are numeric values (response time), Events are discrete occurrences (user login)

**Q: How to query Application Insights logs?**  
A: Use KQL in Log Analytics workspace

---

## ?? Related Resources

### Official Documentation
- [Application Insights Overview](https://docs.microsoft.com/azure/azure-monitor/app/app-insights-overview)
- [Azure Cache for Redis](https://docs.microsoft.com/azure/azure-cache-for-redis/)
- [Azure CDN](https://docs.microsoft.com/azure/cdn/)

### Learning Paths
- [Monitor and troubleshoot Azure solutions](https://docs.microsoft.com/learn/paths/az-204-monitor-troubleshoot-solutions/)

---

## ?? Summary

**Module Status:** ? Complete with real working examples

**You now have:**
- ? Production-ready Application Insights implementation
- ? Real Redis Cache with cache-aside pattern
- ? Azure CDN management examples
- ? Interactive demos for hands-on learning
- ? KQL query library
- ? Best practices and exam tips

**Next Steps:**
1. Run the interactive demos
2. Review the code implementations
3. Practice KQL queries
4. Configure with your Azure resources
5. Complete the AZ-204 exam! ??

---

*For main project documentation, see root `QUICK_START_GUIDE.md`*
