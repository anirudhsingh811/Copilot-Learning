namespace YourNamespace.Models;

public class DeploymentSlotOptions
{
    public required string SubscriptionId { get; set; }
    public required string ResourceGroupName { get; set; }
    public required string AppServiceName { get; set; }
    public required string SlotName { get; set; }
    public string? TargetSlotName { get; set; }
}
