using System;
using System.Threading.Tasks;

namespace AzureAppServiceExamples
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Replace with your Azure subscription ID
            string subscriptionId = "your-subscription-id";
            string resourceGroupName = "myResourceGroup";
            string location = "eastus";
            string appServicePlanName = "myAppServicePlan";
            string webAppName = $"mywebapp{Guid.NewGuid().ToString().Substring(0, 8)}";

            var manager = new AppServiceManager(subscriptionId);

            try
            {
                Console.WriteLine("Azure App Service Management Demo");
                Console.WriteLine("===================================\n");

                // 1. Create Web App
                Console.WriteLine("1. Creating Web App...");
                await manager.CreateWebAppAsync(
                    resourceGroupName,
                    appServicePlanName,
                    webAppName,
                    location
                );

                // 2. Configure App Settings
                Console.WriteLine("\n2. Configuring App Settings...");
                await manager.ConfigureAppSettingsAsync(
                    resourceGroupName,
                    webAppName
                );

                // 3. Enable Managed Identity
                Console.WriteLine("\n3. Enabling Managed Identity...");
                await manager.EnableManagedIdentityAsync(
                    resourceGroupName,
                    webAppName
                );

                // 4. Scale App Service Plan
                Console.WriteLine("\n4. Scaling App Service Plan...");
                await manager.ScaleAppServicePlanAsync(
                    resourceGroupName,
                    appServicePlanName,
                    "S1",
                    2
                );

                // 5. Create Deployment Slot
                Console.WriteLine("\n5. Creating Deployment Slot...");
                await manager.CreateDeploymentSlotAsync(
                    resourceGroupName,
                    webAppName,
                    "staging"
                );

                // 6. Get Web App Metrics
                Console.WriteLine("\n6. Getting Web App Metrics...");
                await manager.GetWebAppMetricsAsync(
                    resourceGroupName,
                    webAppName
                );

                // 7. Swap Slots (uncomment when ready to swap)
                // Console.WriteLine("\n7. Swapping Deployment Slots...");
                // await manager.SwapDeploymentSlotsAsync(
                //     resourceGroupName,
                //     webAppName,
                //     "staging",
                //     "production"
                // );

                Console.WriteLine("\n? All operations completed successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n? Error: {ex.Message}");
                Console.WriteLine($"Details: {ex}");
            }
        }
    }
}
