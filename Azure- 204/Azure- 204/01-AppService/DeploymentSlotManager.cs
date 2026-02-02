using Azure.Identity;
using Azure.ResourceManager;
using Azure.ResourceManager.AppService;
using Azure.ResourceManager.AppService.Models;
using Microsoft.ApplicationInsights;
using Microsoft.Extensions.Logging;

namespace AZ204.AppService;

/// <summary>
/// Manages Azure App Service deployment slots for blue-green deployments
/// </summary>
public class DeploymentSlotManager
{
    private readonly string _subscriptionId;
    private readonly string _resourceGroupName;
    private readonly string _webAppName;
    private readonly ILogger<DeploymentSlotManager> _logger;
    private readonly TelemetryClient _telemetryClient;
    private readonly ArmClient _armClient;

    public DeploymentSlotManager(
        string subscriptionId,
        string resourceGroupName,
        string webAppName,
        ILogger<DeploymentSlotManager> logger,
        TelemetryClient? telemetryClient = null)
    {
        _subscriptionId = subscriptionId;
        _resourceGroupName = resourceGroupName;
        _webAppName = webAppName;
        _logger = logger;
        _telemetryClient = telemetryClient ?? new TelemetryClient();

        var credential = new DefaultAzureCredential();
        _armClient = new ArmClient(credential, subscriptionId);
    }

    /// <summary>
    /// Perform a blue-green deployment with progressive rollout and automatic rollback
    /// </summary>
    public async Task<DeploymentResult> PerformBlueGreenDeploymentAsync(
        string sourceSlot,
        string targetSlot,
        DeploymentOptions options)
    {
        var result = new DeploymentResult { StartTime = DateTime.UtcNow };
        
        _logger.LogInformation("Starting blue-green deployment from {Source} to {Target}", 
            sourceSlot, targetSlot);

        _telemetryClient.TrackEvent("DeploymentStarted", new Dictionary<string, string>
        {
            { "SourceSlot", sourceSlot },
            { "TargetSlot", targetSlot }
        });

        try
        {
            // Phase 1: Validation
            _logger.LogInformation("Phase 1: Validating slots");
            result.Phases.Add("Validation started");
            
            if (!await ValidateSlotHealthAsync(sourceSlot))
            {
                throw new Exception($"Source slot {sourceSlot} is not healthy");
            }

            if (!await ValidateSlotHealthAsync(targetSlot))
            {
                throw new Exception($"Target slot {targetSlot} is not healthy");
            }

            result.Phases.Add("Validation completed");

            // Phase 2: Progressive rollout (if enabled)
            if (options.UseProgressiveRollout)
            {
                _logger.LogInformation("Phase 2: Progressive rollout");
                result.Phases.Add("Progressive rollout started");

                foreach (var percentage in options.RolloutPercentages)
                {
                    _logger.LogInformation("Routing {Percentage}% traffic to {Target}", 
                        percentage, targetSlot);

                    await UpdateTrafficRoutingAsync(targetSlot, percentage);
                    result.Phases.Add($"Traffic at {percentage}%");

                    // Wait and monitor
                    await Task.Delay(TimeSpan.FromMinutes(options.MonitoringDelayMinutes));

                    // Health check with retries
                    var isHealthy = await HealthCheckWithRetriesAsync(
                        targetSlot, 
                        options.HealthCheckRetries);

                    if (!isHealthy)
                    {
                        _logger.LogError("Health check failed at {Percentage}% rollout", percentage);
                        
                        if (options.AutoRollback)
                        {
                            _logger.LogWarning("Auto-rollback triggered");
                            await RollbackDeploymentAsync(sourceSlot);
                            result.Success = false;
                            result.ErrorMessage = $"Health check failed at {percentage}% rollout";
                            return result;
                        }

                        throw new Exception($"Health check failed at {percentage}% rollout");
                    }

                    _logger.LogInformation("Health check passed at {Percentage}%", percentage);
                }

                result.Phases.Add("Progressive rollout completed");
            }

            // Phase 3: Full swap
            _logger.LogInformation("Phase 3: Performing slot swap");
            result.Phases.Add("Slot swap started");

            await SwapSlotsAsync(sourceSlot, targetSlot, options);
            
            result.Phases.Add("Slot swap completed");

            // Phase 4: Post-swap validation
            _logger.LogInformation("Phase 4: Post-swap validation");
            result.Phases.Add("Post-swap validation started");

            await Task.Delay(TimeSpan.FromSeconds(30)); // Allow for DNS propagation

            if (!await ValidateSlotHealthAsync(sourceSlot))
            {
                _logger.LogError("Post-swap validation failed");
                
                if (options.AutoRollback)
                {
                    _logger.LogWarning("Auto-rollback triggered after swap");
                    await SwapSlotsAsync(targetSlot, sourceSlot, options);
                    result.Success = false;
                    result.ErrorMessage = "Post-swap validation failed";
                    return result;
                }

                throw new Exception("Post-swap validation failed");
            }

            result.Phases.Add("Post-swap validation completed");

            // Success
            result.Success = true;
            result.EndTime = DateTime.UtcNow;
            
            _logger.LogInformation("Blue-green deployment completed successfully in {Duration}s", 
                (result.EndTime - result.StartTime).TotalSeconds);

            _telemetryClient.TrackEvent("DeploymentSucceeded", new Dictionary<string, string>
            {
                { "Duration", (result.EndTime - result.StartTime).TotalSeconds.ToString() },
                { "Phases", result.Phases.Count.ToString() }
            });

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Deployment failed");
            
            result.Success = false;
            result.ErrorMessage = ex.Message;
            result.EndTime = DateTime.UtcNow;

            _telemetryClient.TrackException(ex, new Dictionary<string, string>
            {
                { "SourceSlot", sourceSlot },
                { "TargetSlot", targetSlot }
            });

            return result;
        }
    }

    /// <summary>
    /// Validate slot health using health check endpoint
    /// </summary>
    private async Task<bool> ValidateSlotHealthAsync(string slotName)
    {
        try
        {
            using var httpClient = new HttpClient();
            httpClient.Timeout = TimeSpan.FromSeconds(30);

            var healthUrl = slotName == "production" 
                ? $"https://{_webAppName}.azurewebsites.net/health"
                : $"https://{_webAppName}-{slotName}.azurewebsites.net/health";

            _logger.LogDebug("Checking health at {Url}", healthUrl);

            var response = await httpClient.GetAsync(healthUrl);
            var isHealthy = response.IsSuccessStatusCode;

            _logger.LogInformation("Slot {Slot} health check: {Status}", 
                slotName, isHealthy ? "Healthy" : "Unhealthy");

            return isHealthy;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Health check failed for slot {Slot}", slotName);
            return false;
        }
    }

    /// <summary>
    /// Health check with retry logic
    /// </summary>
    private async Task<bool> HealthCheckWithRetriesAsync(string slotName, int maxRetries)
    {
        for (int i = 0; i < maxRetries; i++)
        {
            if (await ValidateSlotHealthAsync(slotName))
            {
                return true;
            }

            if (i < maxRetries - 1)
            {
                _logger.LogWarning("Health check failed, retrying {Attempt}/{Max}", 
                    i + 1, maxRetries);
                await Task.Delay(TimeSpan.FromSeconds(10));
            }
        }

        return false;
    }

    /// <summary>
    /// Update traffic routing between slots
    /// </summary>
    private async Task UpdateTrafficRoutingAsync(string targetSlot, int percentage)
    {
        try
        {
            _logger.LogInformation("Updating traffic routing: {Percentage}% to {Slot}", 
                percentage, targetSlot);

            // In production, use Azure SDK to update routing rules
            // For now, simulate the operation
            await Task.Delay(1000);

            _logger.LogInformation("Traffic routing updated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update traffic routing");
            throw;
        }
    }

    /// <summary>
    /// Swap deployment slots
    /// </summary>
    private async Task SwapSlotsAsync(
        string sourceSlot, 
        string targetSlot, 
        DeploymentOptions options)
    {
        try
        {
            _logger.LogInformation("Swapping slots: {Source} -> {Target}", sourceSlot, targetSlot);

            // Warm up the target slot before swap
            if (options.WarmUpUrls.Any())
            {
                _logger.LogInformation("Warming up target slot");
                await WarmUpSlotAsync(targetSlot, options.WarmUpUrls);
            }

            // Perform the swap using Azure SDK
            // For production: Use WebSiteManagementClient.WebApps.SwapSlotWithProduction
            await Task.Delay(5000); // Simulate swap operation

            _logger.LogInformation("Slot swap completed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Slot swap failed");
            throw;
        }
    }

    /// <summary>
    /// Warm up slot by hitting specified URLs
    /// </summary>
    private async Task WarmUpSlotAsync(string slotName, List<string> urls)
    {
        using var httpClient = new HttpClient();
        httpClient.Timeout = TimeSpan.FromSeconds(30);

        var baseUrl = slotName == "production"
            ? $"https://{_webAppName}.azurewebsites.net"
            : $"https://{_webAppName}-{slotName}.azurewebsites.net";

        foreach (var url in urls)
        {
            try
            {
                var fullUrl = $"{baseUrl}{url}";
                _logger.LogDebug("Warming up: {Url}", fullUrl);
                
                await httpClient.GetAsync(fullUrl);
                await Task.Delay(1000);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Warm-up request failed for {Url}", url);
            }
        }

        _logger.LogInformation("Warm-up completed for {Count} URLs", urls.Count);
    }

    /// <summary>
    /// Rollback to previous slot
    /// </summary>
    private async Task RollbackDeploymentAsync(string targetSlot)
    {
        _logger.LogWarning("Rolling back to {Slot}", targetSlot);

        try
        {
            // Route 100% traffic back to target slot
            await UpdateTrafficRoutingAsync(targetSlot, 100);

            _logger.LogInformation("Rollback completed");

            _telemetryClient.TrackEvent("DeploymentRolledBack", new Dictionary<string, string>
            {
                { "TargetSlot", targetSlot }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Rollback failed");
            throw;
        }
    }

    /// <summary>
    /// Create a new deployment slot
    /// </summary>
    public async Task<bool> CreateDeploymentSlotAsync(
        string slotName,
        string cloneFromSlot = "production")
    {
        try
        {
            _logger.LogInformation("Creating deployment slot {Slot} cloned from {Source}", 
                slotName, cloneFromSlot);

            // In production, use Azure SDK to create slot
            // var slot = await webSiteManagementClient.WebApps.CreateOrUpdateSlotAsync(...)
            
            await Task.Delay(3000); // Simulate creation

            _logger.LogInformation("Deployment slot {Slot} created successfully", slotName);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create deployment slot {Slot}", slotName);
            return false;
        }
    }

    /// <summary>
    /// Delete a deployment slot
    /// </summary>
    public async Task<bool> DeleteDeploymentSlotAsync(string slotName)
    {
        try
        {
            _logger.LogInformation("Deleting deployment slot {Slot}", slotName);

            // In production, use Azure SDK to delete slot
            await Task.Delay(2000); // Simulate deletion

            _logger.LogInformation("Deployment slot {Slot} deleted successfully", slotName);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete deployment slot {Slot}", slotName);
            return false;
        }
    }
}

/// <summary>
/// Deployment options for blue-green deployments
/// </summary>
public class DeploymentOptions
{
    public bool UseProgressiveRollout { get; set; } = false;
    public int[] RolloutPercentages { get; set; } = new[] { 10, 25, 50, 100 };
    public int MonitoringDelayMinutes { get; set; } = 5;
    public bool AutoRollback { get; set; } = true;
    public int HealthCheckRetries { get; set; } = 3;
    public List<string> WarmUpUrls { get; set; } = new() { "/", "/health", "/api/status" };
    public bool PreserveSlotSettings { get; set; } = true;
}

/// <summary>
/// Result of a deployment operation
/// </summary>
public class DeploymentResult
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public List<string> Phases { get; set; } = new();
    
    public TimeSpan Duration => EndTime - StartTime;
}
