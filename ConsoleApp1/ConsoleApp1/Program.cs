using System;
using AzureAppServiceExamples;

Console.WriteLine("╔════════════════════════════════════════════════════════════════╗");
Console.WriteLine("║   Azure App Service - Learning Examples & Code Samples        ║");
Console.WriteLine("╚════════════════════════════════════════════════════════════════╝");
Console.WriteLine();

Console.WriteLine("📚 Available Examples:");
Console.WriteLine();
Console.WriteLine("   1. Run ProgramExample Demo");
Console.WriteLine("   2. View AppServiceManager Class Info");
Console.WriteLine("   3. View ASP.NET Core Startup Info");
Console.WriteLine("   4. Exit");
Console.WriteLine();
Console.Write("Select an option (1-4): ");

var choice = Console.ReadLine();

switch (choice)
{
    case "1":
        Console.WriteLine();
        await ProgramExample.MainRunAsync(args);
        break;
    
    case "2":
        Console.WriteLine();
        Console.WriteLine("╔════════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║   AppServiceManager Class - Azure SDK Examples                ║");
        Console.WriteLine("╚════════════════════════════════════════════════════════════════╝");
        Console.WriteLine();
        Console.WriteLine("The AppServiceManager class (7-WebApp.cs) provides methods for:");
        Console.WriteLine();
        Console.WriteLine("  ✅ Creating Web Apps with App Service Plans");
        Console.WriteLine("  ✅ Configuring App Settings");
        Console.WriteLine("  ✅ Scaling App Service Plans");
        Console.WriteLine("  ✅ Creating Deployment Slots");
        Console.WriteLine("  ✅ Swapping Deployment Slots");
        Console.WriteLine("  ✅ Enabling Managed Identity");
        Console.WriteLine("  ✅ Getting Web App Metrics");
        Console.WriteLine();
        Console.WriteLine("To use it:");
        Console.WriteLine("  var manager = new AppServiceManager(\"your-subscription-id\");");
        Console.WriteLine("  await manager.CreateWebAppAsync(resourceGroup, planName, appName);");
        Console.WriteLine();
        Console.WriteLine("📝 Note: Some operations show Azure CLI commands due to SDK API");
        Console.WriteLine("   version compatibility. The .azcli files are production-ready!");
        Console.WriteLine();
        break;
    
    case "3":
        Console.WriteLine();
        Console.WriteLine("╔════════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║   ASP.NET Core Startup - Web App Example                      ║");
        Console.WriteLine("╚════════════════════════════════════════════════════════════════╝");
        Console.WriteLine();
        Console.WriteLine("The Startup class (9-AspNetCore-Startup.cs) demonstrates:");
        Console.WriteLine();
        Console.WriteLine("  ✅ Azure AD Authentication");
        Console.WriteLine("  ✅ Session Management");
        Console.WriteLine("  ✅ Health Checks");
        Console.WriteLine("  ✅ Application Insights Integration");
        Console.WriteLine("  ✅ Response Compression");
        Console.WriteLine("  ✅ Security Best Practices (HTTPS, HSTS)");
        Console.WriteLine();
        Console.WriteLine("This is a reference example for building production-ready");
        Console.WriteLine("ASP.NET Core web applications on Azure App Service.");
        Console.WriteLine();
        break;
    
    case "4":
        Console.WriteLine();
        Console.WriteLine("Goodbye!");
        return;
    
    default:
        Console.WriteLine();
        Console.WriteLine("Invalid option. Please run again and select 1-4.");
        break;
}

Console.WriteLine();
Console.WriteLine("Press any key to exit...");
Console.ReadKey();

//Console.WriteLine("╔════════════════════════════════════════════════════════════════╗");
//Console.WriteLine("║   Azure App Service - Learning Examples & Code Samples        ║");
//Console.WriteLine("╚════════════════════════════════════════════════════════════════╝");
//Console.WriteLine();

//Console.WriteLine("📚 Available Examples:");
//Console.WriteLine();

//Console.WriteLine("   Azure CLI Scripts (.azcli files):");
//Console.WriteLine("   ─────────────────────────────────────────────────────────────");
//Console.WriteLine("   1. 1-CreateAppService.azcli     - Create and manage App Service");
//Console.WriteLine("   2. 2-AppSettings.azcli          - Configure app settings");
//Console.WriteLine("   3. 3-Authentication.azcli       - Setup authentication");
//Console.WriteLine("   4. 4-Scaling.azcli              - Manual and auto-scaling");
//Console.WriteLine("   5. 5-DeploymentSlots.azcli      - Deployment slots & swapping");
//Console.WriteLine("   6. 6-DeploymentMethods.azcli    - Various deployment methods");
//Console.WriteLine();

//Console.WriteLine("   C# SDK Examples (.cs files):");
//Console.WriteLine("   ─────────────────────────────────────────────────────────────");
//Console.WriteLine("   7. 7-WebApp.cs                  - App Service management class");
//Console.WriteLine("   8. 8-WebApp-Program.cs          - Demo program with all operations");
//Console.WriteLine("   9. 9-AspNetCore-Startup.cs      - ASP.NET Core web app example");
//Console.WriteLine();

//Console.WriteLine("   Infrastructure as Code:");
//Console.WriteLine("   ─────────────────────────────────────────────────────────────");
//Console.WriteLine("   10. 11-Bicep-AppService.bicep   - Complete Bicep template");
//Console.WriteLine();

//Console.WriteLine("   Configuration:");
//Console.WriteLine("   ─────────────────────────────────────────────────────────────");
//Console.WriteLine("   11. 10-appsettings.json         - App configuration example");
//Console.WriteLine();

//Console.WriteLine("════════════════════════════════════════════════════════════════");
//Console.WriteLine();

//Console.WriteLine("🎯 Quick Start Guide:");
//Console.WriteLine();
//Console.WriteLine("   For Azure CLI examples:");
//Console.WriteLine("   ----------------------");
//Console.WriteLine("   1. Login to Azure:       az login");
//Console.WriteLine("   2. Set subscription:     az account set --subscription \"your-id\"");
//Console.WriteLine("   3. Run any .azcli file:  Execute commands line by line");
//Console.WriteLine();

//Console.WriteLine("   For C# SDK examples:");
//Console.WriteLine("   -------------------");
//Console.WriteLine("   1. Update subscription ID in the code");
//Console.WriteLine("   2. Ensure you're logged in to Azure");
//Console.WriteLine("   3. Run: dotnet run");
//Console.WriteLine();

//Console.WriteLine("   For Bicep deployment:");
//Console.WriteLine("   --------------------");
//Console.WriteLine("   az deployment group create \\");
//Console.WriteLine("     --resource-group myResourceGroup \\");
//Console.WriteLine("     --template-file 11-Bicep-AppService.bicep");
//Console.WriteLine();

//Console.WriteLine("════════════════════════════════════════════════════════════════");
//Console.WriteLine();

//Console.WriteLine("📖 Key Topics Covered:");
//Console.WriteLine();
//Console.WriteLine("   ✅ Creating and updating App Services");
//Console.WriteLine("   ✅ Authentication & Authorization (Azure AD, Easy Auth)");
//Console.WriteLine("   ✅ Configuring app settings and connection strings");
//Console.WriteLine("   ✅ Manual and automatic scaling");
//Console.WriteLine("   ✅ Deployment slots for zero-downtime deployments");
//Console.WriteLine("   ✅ Multiple deployment methods (ZIP, Git, GitHub Actions, etc.)");
//Console.WriteLine("   ✅ Managed Identity for secure access");
//Console.WriteLine("   ✅ Application Insights integration");
//Console.WriteLine();

//Console.WriteLine("════════════════════════════════════════════════════════════════");
//Console.WriteLine();

//Console.WriteLine("💡 Would you like to view a code walkthrough? (y/n): ");
//var input = Console.ReadLine();

//if (input?.ToLower() == "y" || input?.ToLower() == "yes")
//{
//    Console.WriteLine();
//    Console.WriteLine("🚀 Azure App Service Code Walkthrough");
//    Console.WriteLine();
//    Console.WriteLine("Note: To actually create Azure resources, you need to:");
//    Console.WriteLine("   1. Update the subscription ID in 8-WebApp-Program.cs");
//    Console.WriteLine("   2. Be authenticated with Azure (az login or Azure credentials)");
//    Console.WriteLine("   3. Have appropriate permissions in your Azure subscription");
//    Console.WriteLine();
//    Console.WriteLine("Press any key to view a sample code walkthrough...");
//    Console.ReadKey();
//    Console.WriteLine();
//    Console.WriteLine();

//    ShowCodeWalkthrough();
//}
//else
//{
//    Console.WriteLine();
//    Console.WriteLine("✨ All example files are available in the AzureAppServiceExamples folder!");
//    Console.WriteLine("   Check out README.md for detailed documentation.");
//}

//Console.WriteLine();
//Console.WriteLine("Press any key to exit...");
//Console.ReadKey();

//static void ShowCodeWalkthrough()
//{
//    Console.WriteLine("════════════════════════════════════════════════════════════════");
//    Console.WriteLine("   Code Walkthrough: Creating an App Service with C#");
//    Console.WriteLine("════════════════════════════════════════════════════════════════");
//    Console.WriteLine();

//    Console.WriteLine("1️⃣ Initialize Azure Resource Manager Client:");
//    Console.WriteLine("   ─────────────────────────────────────────");
//    Console.WriteLine("   var armClient = new ArmClient(new DefaultAzureCredential());");
//    Console.WriteLine();

//    Console.WriteLine("2️⃣ Get Resource Group:");
//    Console.WriteLine("   ────────────────────");
//    Console.WriteLine("   var subscription = await armClient.GetSubscriptionResource(");
//    Console.WriteLine("       new ResourceIdentifier($\"/subscriptions/{subscriptionId}\")");
//    Console.WriteLine("   ).GetAsync();");
//    Console.WriteLine("   var resourceGroup = await subscription.Value");
//    Console.WriteLine("       .GetResourceGroups().GetAsync(resourceGroupName);");
//    Console.WriteLine();

//    Console.WriteLine("3️⃣ Create App Service Plan:");
//    Console.WriteLine("   ─────────────────────────");
//    Console.WriteLine("   var appServicePlanData = new AppServicePlanData(location) {");
//    Console.WriteLine("       Sku = new AppServiceSkuDescription {");
//    Console.WriteLine("           Name = \"B1\", Tier = \"Basic\", Capacity = 1");
//    Console.WriteLine("       },");
//    Console.WriteLine("       Kind = \"linux\",");
//    Console.WriteLine("       IsReserved = true");
//    Console.WriteLine("   };");
//    Console.WriteLine();

//    Console.WriteLine("4️⃣ Create Web App:");
//    Console.WriteLine("   ───────────────");
//    Console.WriteLine("   var webSiteData = new WebSiteData(location) {");
//    Console.WriteLine("       AppServicePlanId = appServicePlan.Id,");
//    Console.WriteLine("       SiteConfig = new SiteConfigProperties {");
//    Console.WriteLine("           LinuxFxVersion = \"DOTNETCORE|8.0\",");
//    Console.WriteLine("           HttpsOnly = true");
//    Console.WriteLine("       }");
//    Console.WriteLine("   };");
//    Console.WriteLine();

//    Console.WriteLine("5️⃣ Deploy Your App:");
//    Console.WriteLine("   ────────────────");
//    Console.WriteLine("   az webapp deployment source config-zip \\");
//    Console.WriteLine("       --name myWebApp \\");
//    Console.WriteLine("       --resource-group myResourceGroup \\");
//    Console.WriteLine("       --src ./publish.zip");
//    Console.WriteLine();

//    Console.WriteLine("════════════════════════════════════════════════════════════════");
//    Console.WriteLine();
//    Console.WriteLine("✨ For complete working examples, see:");
//    Console.WriteLine("   • 7-WebApp.cs - Full implementation reference");
//    Console.WriteLine("   • 8-WebApp-Program.cs - Complete demo reference");
//    Console.WriteLine("   • AzureAppServiceExamples folder - All CLI examples (READY TO USE!)");
//    Console.WriteLine();
//    Console.WriteLine("💡 TIP: The .azcli files are production-ready and can be");
//    Console.WriteLine("   executed immediately after logging in with 'az login'");
//    Console.WriteLine();
//}
