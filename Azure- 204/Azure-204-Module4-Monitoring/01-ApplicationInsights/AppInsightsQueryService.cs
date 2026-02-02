using Azure.Identity;
using Azure.Monitor.Query;
using Azure.Monitor.Query.Models;
using Microsoft.Extensions.Logging;
using System.Text;

namespace AZ204.Monitoring.ApplicationInsights;

/// <summary>
/// Query Application Insights using KQL (Kusto Query Language)
/// Real examples of log analytics queries
/// </summary>
public class AppInsightsQueryService
{
    private readonly LogsQueryClient _logsClient;
    private readonly ILogger<AppInsightsQueryService> _logger;

    public AppInsightsQueryService(ILogger<AppInsightsQueryService> logger)
    {
        _logger = logger;
        _logsClient = new LogsQueryClient(new DefaultAzureCredential());
    }

    /// <summary>
    /// Query Application Insights logs
    /// </summary>
    public async Task<string> QueryLogsAsync(string workspaceId, string kqlQuery, int timeRangeHours = 24)
    {
        try
        {
            _logger.LogInformation("Executing KQL query: {Query}", kqlQuery);
            Console.WriteLine($"\n?? Executing query...");
            Console.WriteLine($"Query: {kqlQuery}");
            Console.WriteLine($"Time range: Last {timeRangeHours} hours\n");

            var timeRange = new QueryTimeRange(TimeSpan.FromHours(timeRangeHours));
            var response = await _logsClient.QueryWorkspaceAsync(
                workspaceId,
                kqlQuery,
                timeRange);

            var result = new StringBuilder();
            result.AppendLine("Query Results:");
            result.AppendLine("???????????????????????????????????????");

            foreach (var table in response.Value.AllTables)
            {
                result.AppendLine($"\nTable: {table.Name}");
                result.AppendLine($"Columns: {string.Join(", ", table.Columns.Select(c => c.Name))}");
                result.AppendLine($"Rows: {table.Rows.Count}");
                result.AppendLine();

                // Display first 10 rows
                var rowsToDisplay = Math.Min(10, table.Rows.Count);
                for (int i = 0; i < rowsToDisplay; i++)
                {
                    var row = table.Rows[i];
                    result.AppendLine($"Row {i + 1}:");
                    for (int j = 0; j < table.Columns.Count; j++)
                    {
                        result.AppendLine($"  {table.Columns[j].Name}: {row[j]}");
                    }
                    result.AppendLine();
                }

                if (table.Rows.Count > 10)
                {
                    result.AppendLine($"... and {table.Rows.Count - 10} more rows");
                }
            }

            var resultStr = result.ToString();
            Console.WriteLine(resultStr);
            return resultStr;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to execute query");
            var errorMsg = $"? Query failed: {ex.Message}";
            Console.WriteLine(errorMsg);
            return errorMsg;
        }
    }

    /// <summary>
    /// Get performance metrics from Application Insights
    /// </summary>
    public async Task<string> GetPerformanceMetricsAsync(string workspaceId)
    {
        var query = @"
            requests
            | where timestamp > ago(24h)
            | summarize 
                RequestCount = count(),
                AvgDuration = avg(duration),
                P95Duration = percentile(duration, 95),
                P99Duration = percentile(duration, 99),
                FailureRate = countif(success == false) * 100.0 / count()
            by bin(timestamp, 1h)
            | order by timestamp desc
            | limit 24";

        return await QueryLogsAsync(workspaceId, query);
    }

    /// <summary>
    /// Get exception analytics
    /// </summary>
    public async Task<string> GetExceptionAnalyticsAsync(string workspaceId)
    {
        var query = @"
            exceptions
            | where timestamp > ago(24h)
            | summarize 
                ExceptionCount = count(),
                UniqueExceptions = dcount(type)
            by type, outerMessage
            | order by ExceptionCount desc
            | limit 20";

        return await QueryLogsAsync(workspaceId, query);
    }

    /// <summary>
    /// Get dependency analytics (database, external APIs, etc.)
    /// </summary>
    public async Task<string> GetDependencyAnalyticsAsync(string workspaceId)
    {
        var query = @"
            dependencies
            | where timestamp > ago(24h)
            | summarize 
                CallCount = count(),
                AvgDuration = avg(duration),
                FailureCount = countif(success == false),
                FailureRate = countif(success == false) * 100.0 / count()
            by type, target, name
            | order by CallCount desc
            | limit 20";

        return await QueryLogsAsync(workspaceId, query);
    }

    /// <summary>
    /// Get custom events analytics
    /// </summary>
    public async Task<string> GetCustomEventsAsync(string workspaceId)
    {
        var query = @"
            customEvents
            | where timestamp > ago(24h)
            | summarize EventCount = count() by name
            | order by EventCount desc
            | limit 20";

        return await QueryLogsAsync(workspaceId, query);
    }

    /// <summary>
    /// Get availability results (health checks)
    /// </summary>
    public async Task<string> GetAvailabilityResultsAsync(string workspaceId)
    {
        var query = @"
            availabilityResults
            | where timestamp > ago(24h)
            | summarize 
                TestCount = count(),
                SuccessRate = countif(success == true) * 100.0 / count(),
                AvgDuration = avg(duration)
            by name, location
            | order by SuccessRate asc";

        return await QueryLogsAsync(workspaceId, query);
    }

    /// <summary>
    /// Get page views and user analytics
    /// </summary>
    public async Task<string> GetUserAnalyticsAsync(string workspaceId)
    {
        var query = @"
            pageViews
            | where timestamp > ago(24h)
            | summarize 
                PageViews = count(),
                UniqueUsers = dcount(user_Id),
                AvgDuration = avg(duration)
            by name
            | order by PageViews desc
            | limit 20";

        return await QueryLogsAsync(workspaceId, query);
    }

    /// <summary>
    /// Demonstrate common KQL query patterns
    /// </summary>
    public void ShowKqlExamples()
    {
        Console.WriteLine("\n?? KQL Query Examples for Application Insights");
        Console.WriteLine("?????????????????????????????????????????????????????????????\n");

        var examples = new[]
        {
            ("Failed Requests", @"
requests
| where timestamp > ago(1h)
| where success == false
| project timestamp, name, resultCode, duration
| order by timestamp desc"),

            ("Slow Requests (>2 seconds)", @"
requests
| where timestamp > ago(24h)
| where duration > 2000
| summarize count() by name, bin(timestamp, 1h)
| order by timestamp desc"),

            ("Top 10 Slowest Dependencies", @"
dependencies
| where timestamp > ago(24h)
| summarize AvgDuration = avg(duration) by name, type
| top 10 by AvgDuration desc"),

            ("Exception Trends", @"
exceptions
| where timestamp > ago(7d)
| summarize ExceptionCount = count() by bin(timestamp, 1d), type
| render timechart"),

            ("User Journey", @"
union pageViews, customEvents
| where timestamp > ago(1h)
| where user_Id == 'specific-user-id'
| order by timestamp asc
| project timestamp, itemType, name"),

            ("Performance by Operation", @"
requests
| where timestamp > ago(24h)
| summarize 
    count(),
    avg(duration),
    percentile(duration, 50),
    percentile(duration, 95),
    percentile(duration, 99)
by operation_Name"),

            ("Dependency Failure Analysis", @"
dependencies
| where timestamp > ago(24h)
| where success == false
| summarize FailureCount = count() by type, target, resultCode
| order by FailureCount desc"),

            ("Custom Metrics Trending", @"
customMetrics
| where timestamp > ago(24h)
| where name == 'OrderProcessingTime'
| summarize avg(value), max(value), min(value) by bin(timestamp, 1h)
| render timechart")
        };

        foreach (var (title, query) in examples)
        {
            Console.WriteLine($"?? {title}");
            Console.WriteLine($"   Query:{query}");
            Console.WriteLine();
        }
    }
}
