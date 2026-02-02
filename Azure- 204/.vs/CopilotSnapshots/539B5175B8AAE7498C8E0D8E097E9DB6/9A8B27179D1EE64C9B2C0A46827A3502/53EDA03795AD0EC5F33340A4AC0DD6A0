using Azure.Core;
using Azure.Identity;
using Azure.ResourceManager;
using Azure.ResourceManager.AppService;
using Azure.ResourceManager.AppService.Models;
using Azure.ResourceManager.Resources;
using Microsoft.Extensions.Logging;

namespace AZ204.AppService;

/// <summary>
/// Service for managing Azure App Service deployment slots
/// </summary>
public class DeploymentSlotService
{
    private readonly ArmClient _armClient;
    private readonly ILogger<DeploymentSlotService> _logger;

    public DeploymentSlotService(ILogger<DeploymentSlotService> logger, TokenCredential? credential = null)
    {
        _logger = logger;
        _armClient = new ArmClient(credential ?? new DefaultAzureCredential());
    }

    public DeploymentSlotService(TokenCredential credential)
    {
        _logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<DeploymentSlotService>();
        _armClient = new ArmClient(credential);
    }

    /// <summary>
    /// Creates a deployment slot for an App Service
    /// </summary>
    public async Task<WebSiteSlotResource> CreateDeploymentSlotAsync(
        string subscriptionId,
        string resourceGroupName,
        string appServiceName,
        string slotName,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating deployment slot '{SlotName}' for app '{AppServiceName}'", 
            slotName, appServiceName);

        var subscription = _armClient.GetSubscriptionResource(
            new ResourceIdentifier($"/subscriptions/{subscriptionId}"));
        
        var resourceGroup = await subscription.GetResourceGroupAsync(resourceGroupName, cancellationToken);
        var webSite = await resourceGroup.Value.GetWebSiteAsync(appServiceName, cancellationToken);
        
        // Create slot data based on production site
        var slotData = new WebSiteData(webSite.Value.Data.Location)
        {
            AppServicePlanId = webSite.Value.Data.AppServicePlanId,
            SiteConfig = new SiteConfigProperties()
        };

        // Copy app settings if they exist
        if (webSite.Value.Data.SiteConfig?.AppSettings != null)
        {
            foreach (var setting in webSite.Value.Data.SiteConfig.AppSettings)
            {
                slotData.SiteConfig.AppSettings.Add(setting);
            }
        }

        var slotCollection = webSite.Value.GetWebSiteSlots();
        var slot = await slotCollection.CreateOrUpdateAsync(
            Azure.WaitUntil.Completed,
            slotName,
            slotData,
            cancellationToken);

        _logger.LogInformation("Successfully created slot '{SlotName}'", slotName);
        return slot.Value;
    }

    /// <summary>
    /// Swaps a deployment slot with production or another slot
    /// </summary>
    public async Task SwapSlotAsync(
        string subscriptionId,
        string resourceGroupName,
        string appServiceName,
        string sourceSlotName,
        string? targetSlotName = null,
        CancellationToken cancellationToken = default)
    {
        var target = targetSlotName ?? "production";
        _logger.LogInformation("Swapping slot '{SourceSlot}' with '{TargetSlot}'", 
            sourceSlotName, target);

        var subscription = _armClient.GetSubscriptionResource(
            new ResourceIdentifier($"/subscriptions/{subscriptionId}"));
        
        var resourceGroup = await subscription.GetResourceGroupAsync(resourceGroupName, cancellationToken);
        var webSite = await resourceGroup.Value.GetWebSiteAsync(appServiceName, cancellationToken);
        
        // Create swap parameters with the correct constructor
        var swapParameters = new CsmSlotEntity(target, preserveVnet: true);

        var slot = await webSite.Value.GetWebSiteSlotAsync(sourceSlotName, cancellationToken);
        
        await slot.Value.SwapSlotAsync(
            Azure.WaitUntil.Completed,
            swapParameters,
            cancellationToken);

        _logger.LogInformation("Successfully swapped slots");
    }

    /// <summary>
    /// Gets information about a deployment slot
    /// </summary>
    public async Task<WebSiteSlotResource> GetDeploymentSlotAsync(
        string subscriptionId,
        string resourceGroupName,
        string appServiceName,
        string slotName,
        CancellationToken cancellationToken = default)
    {
        var subscription = _armClient.GetSubscriptionResource(
            new ResourceIdentifier($"/subscriptions/{subscriptionId}"));
        
        var resourceGroup = await subscription.GetResourceGroupAsync(resourceGroupName, cancellationToken);
        var webSite = await resourceGroup.Value.GetWebSiteAsync(appServiceName, cancellationToken);
        var slot = await webSite.Value.GetWebSiteSlotAsync(slotName, cancellationToken);

        return slot.Value;
    }

    /// <summary>
    /// Lists all deployment slots for an App Service
    /// </summary>
    public async Task<List<WebSiteSlotResource>> ListDeploymentSlotsAsync(
        string subscriptionId,
        string resourceGroupName,
        string appServiceName,
        CancellationToken cancellationToken = default)
    {
        var subscription = _armClient.GetSubscriptionResource(
            new ResourceIdentifier($"/subscriptions/{subscriptionId}"));
        
        var resourceGroup = await subscription.GetResourceGroupAsync(resourceGroupName, cancellationToken);
        var webSite = await resourceGroup.Value.GetWebSiteAsync(appServiceName, cancellationToken);
        var slots = new List<WebSiteSlotResource>();

        await foreach (var slot in webSite.Value.GetWebSiteSlots().GetAllAsync(cancellationToken))
        {
            slots.Add(slot);
        }

        return slots;
    }

    /// <summary>
    /// Deletes a deployment slot
    /// </summary>
    public async Task DeleteDeploymentSlotAsync(
        string subscriptionId,
        string resourceGroupName,
        string appServiceName,
        string slotName,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting deployment slot '{SlotName}'", slotName);

        var subscription = _armClient.GetSubscriptionResource(
            new ResourceIdentifier($"/subscriptions/{subscriptionId}"));
        
        var resourceGroup = await subscription.GetResourceGroupAsync(resourceGroupName, cancellationToken);
        var webSite = await resourceGroup.Value.GetWebSiteAsync(appServiceName, cancellationToken);
        var slot = await webSite.Value.GetWebSiteSlotAsync(slotName, cancellationToken);

        await slot.Value.DeleteAsync(Azure.WaitUntil.Completed, cancellationToken: cancellationToken);
        
        _logger.LogInformation("Successfully deleted slot '{SlotName}'", slotName);
    }
}