using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using AZ204.Security.Services;

namespace AZ204.Security;

class Program
{
    static async Task Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args) // Creates a default host builder
            .ConfigureAppConfiguration((context, config) =>
            {
                config.AddJsonFile("appsettings.json", optional: true); // Load settings from appsettings.json
                config.AddEnvironmentVariables();// Load settings from environment variables
                config.AddUserSecrets<Program>(optional: true); // Load user secrets for sensitive data
            })
            .ConfigureServices((context, services) => // Configure DI services
            {
                services.AddLogging(builder =>
                {
                    builder.AddConsole(); // Log to console and Di debug
                    builder.AddDebug();
                    builder.SetMinimumLevel(LogLevel.Information);
                });

                // Register services
                services.AddTransient<KeyVaultService>();
                services.AddTransient<MsalAuthenticationService>();
                services.AddTransient<GraphApiService>();
                services.AddTransient<ManagedIdentityService>();
                services.AddTransient<SharedAccessSignatureService>();
            })
            .Build();

        var logger = host.Services.GetRequiredService<ILogger<Program>>();

        Console.Clear();
        DisplayHeader();

        bool exit = false;
        while (!exit)
        {
            DisplayMenu();
            var choice = Console.ReadKey(true).Key;
            Console.WriteLine();

            try
            {
                switch (choice)
                {
                    case ConsoleKey.D1:
                    case ConsoleKey.NumPad1:
                        var kvService = host.Services.GetRequiredService<KeyVaultService>();
                        await kvService.RunKeyVaultDemoAsync();
                        break;

                    case ConsoleKey.D2:
                    case ConsoleKey.NumPad2:
                        var msalService = host.Services.GetRequiredService<MsalAuthenticationService>();
                        await msalService.RunMsalDemoAsync();
                        break;

                    case ConsoleKey.D3:
                    case ConsoleKey.NumPad3:
                        var graphService = host.Services.GetRequiredService<GraphApiService>();
                        await graphService.RunGraphApiDemoAsync();
                        break;

                    case ConsoleKey.D4:
                    case ConsoleKey.NumPad4:
                        var miService = host.Services.GetRequiredService<ManagedIdentityService>();
                        await miService.RunManagedIdentityDemoAsync();
                        break;

                    case ConsoleKey.D5:
                    case ConsoleKey.NumPad5:
                        var sasService = host.Services.GetRequiredService<SharedAccessSignatureService>();
                        await sasService.RunSasDemoAsync();
                        break;

                    case ConsoleKey.D6:
                    case ConsoleKey.NumPad6:
                        await RunAllDemosAsync(host);
                        break;

                    case ConsoleKey.D7:
                    case ConsoleKey.NumPad7:
                        DisplaySetupGuide();
                        break;

                    case ConsoleKey.D8:
                    case ConsoleKey.NumPad8:
                        DisplayExamTips();
                        break;

                    case ConsoleKey.Q:
                    case ConsoleKey.Escape:
                        exit = true;
                        Console.WriteLine("\n?? Goodbye! Good luck with your AZ-204 exam!\n");
                        continue;

                    default:
                        Console.WriteLine("? Invalid option. Please try again.\n");
                        break;
                }

                if (!exit && choice != ConsoleKey.D7 && choice != ConsoleKey.NumPad7 && 
                    choice != ConsoleKey.D8 && choice != ConsoleKey.NumPad8)
                {
                    Console.WriteLine("\nPress any key to continue...");
                    Console.ReadKey(true);
                    Console.Clear();
                    DisplayHeader();
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error executing demo");
                Console.WriteLine($"\n? Unexpected error: {ex.Message}");
                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey(true);
                Console.Clear();
                DisplayHeader();
            }
        }
    }

    static void DisplayHeader()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("?????????????????????????????????????????????????????????????????????");
        Console.WriteLine("?         AZ-204: Implement Azure Security                         ?");
        Console.WriteLine("?         Interactive Learning & Hands-On Demos                    ?");
        Console.WriteLine("?????????????????????????????????????????????????????????????????????");
        Console.ResetColor();
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("??  Exam Weight: 20-25% (HIGHEST PRIORITY!)");
        Console.ResetColor();
        Console.WriteLine();
    }

    static void DisplayMenu()
    {
        Console.WriteLine("?? Choose a demo to run:\n");
        Console.WriteLine("  [1] ?? Azure Key Vault");
        Console.WriteLine("      ?? Secrets, Keys, and Certificates management");
        Console.WriteLine();
        Console.WriteLine("  [2] ?? MSAL Authentication");
        Console.WriteLine("      ?? Public Client, Confidential Client, Device Code Flow");
        Console.WriteLine();
        Console.WriteLine("  [3] ?? Microsoft Graph API");
        Console.WriteLine("      ?? User Profile, Mail, Calendar, Organization");
        Console.WriteLine();
        Console.WriteLine("  [4] ?? Managed Identity");
        Console.WriteLine("      ?? System-Assigned, User-Assigned, DefaultAzureCredential");
        Console.WriteLine();
        Console.WriteLine("  [5] ?? Shared Access Signatures (SAS)");
        Console.WriteLine("      ?? Blob SAS, Account SAS, Stored Access Policies");
        Console.WriteLine();
        Console.WriteLine("  [6] ?? Run All Demos");
        Console.WriteLine("      ?? Execute all demonstrations sequentially");
        Console.WriteLine();
        Console.WriteLine("  [7] ?? Setup Guide");
        Console.WriteLine("      ?? Azure resources and configuration required");
        Console.WriteLine();
        Console.WriteLine("  [8] ?? AZ-204 Exam Tips");
        Console.WriteLine("      ?? Key concepts and exam preparation tips");
        Console.WriteLine();
        Console.WriteLine("  [Q] ? Exit");
        Console.WriteLine();
        Console.Write("Select an option: ");
    }

    static async Task RunAllDemosAsync(IHost host)
    {
        Console.WriteLine("\n?? Running All Security Demos\n");
        Console.WriteLine("???????????????????????????????????????????????????????????????\n");

        var services = new[]
        {
            ("Key Vault", host.Services.GetRequiredService<KeyVaultService>().RunKeyVaultDemoAsync()),
            ("MSAL Authentication", host.Services.GetRequiredService<MsalAuthenticationService>().RunMsalDemoAsync()),
            ("Microsoft Graph", host.Services.GetRequiredService<GraphApiService>().RunGraphApiDemoAsync()),
            ("Managed Identity", host.Services.GetRequiredService<ManagedIdentityService>().RunManagedIdentityDemoAsync()),
            ("Shared Access Signatures", host.Services.GetRequiredService<SharedAccessSignatureService>().RunSasDemoAsync())
        };

        for (int i = 0; i < services.Length; i++)
        {
            var (name, task) = services[i];
            Console.WriteLine($"\n[{i + 1}/{services.Length}] Running {name} demo...");
            Console.WriteLine("???????????????????????????????????????????????????????????????\n");

            try
            {
                await task;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"? {name} demo failed: {ex.Message}");
            }

            if (i < services.Length - 1)
            {
                Console.WriteLine("\n-----------------------------------------------------------");
            }
        }

        Console.WriteLine("\n? All demos completed!");
    }

    static void DisplaySetupGuide()
    {
        Console.WriteLine("\n?? Azure Setup Guide for AZ-204 Security Module\n");
        Console.WriteLine("???????????????????????????????????????????????????????????????????\n");

        Console.WriteLine("?? 1. AZURE KEY VAULT");
        Console.WriteLine("   Prerequisites:");
        Console.WriteLine("   • Azure subscription");
        Console.WriteLine("   • Azure CLI installed and authenticated (az login)");
        Console.WriteLine();
        Console.WriteLine("   Setup Commands:");
        Console.WriteLine("   ```");
        Console.WriteLine("   # Create resource group");
        Console.WriteLine("   az group create --name rg-az204-security --location eastus");
        Console.WriteLine();
        Console.WriteLine("   # Create Key Vault");
        Console.WriteLine("   az keyvault create --name kv-az204-demo-<unique> \\");
        Console.WriteLine("     --resource-group rg-az204-security \\");
        Console.WriteLine("     --location eastus");
        Console.WriteLine();
        Console.WriteLine("   # Grant yourself permissions");
        Console.WriteLine("   az keyvault set-policy --name kv-az204-demo-<unique> \\");
        Console.WriteLine("     --upn <your-email@domain.com> \\");
        Console.WriteLine("     --secret-permissions get list set delete \\");
        Console.WriteLine("     --key-permissions get list create delete \\");
        Console.WriteLine("     --certificate-permissions get list create delete");
        Console.WriteLine("   ```");
        Console.WriteLine();

        Console.WriteLine("?? 2. MSAL & MICROSOFT GRAPH");
        Console.WriteLine("   Prerequisites:");
        Console.WriteLine("   • Azure AD tenant");
        Console.WriteLine("   • Permissions to create App Registrations");
        Console.WriteLine();
        Console.WriteLine("   Setup Steps:");
        Console.WriteLine("   1. Go to Azure Portal > Azure Active Directory > App Registrations");
        Console.WriteLine("   2. Click 'New registration'");
        Console.WriteLine("   3. Name: 'AZ204-Security-Demo'");
        Console.WriteLine("   4. Supported account types: 'Accounts in this organizational directory only'");
        Console.WriteLine("   5. Redirect URI: 'Public client/native' > 'http://localhost'");
        Console.WriteLine("   6. Click 'Register'");
        Console.WriteLine();
        Console.WriteLine("   7. Note the 'Application (client) ID' and 'Directory (tenant) ID'");
        Console.WriteLine();
        Console.WriteLine("   8. Go to 'API permissions':");
        Console.WriteLine("      • Add 'Microsoft Graph' > 'Delegated permissions':");
        Console.WriteLine("        - User.Read");
        Console.WriteLine("        - Mail.Read");
        Console.WriteLine("        - Calendars.Read");
        Console.WriteLine("      • Click 'Grant admin consent'");
        Console.WriteLine();
        Console.WriteLine("   9. For Confidential Client:");
        Console.WriteLine("      • Go to 'Certificates & secrets'");
        Console.WriteLine("      • Create new client secret");
        Console.WriteLine("      • Copy the secret value immediately");
        Console.WriteLine();

        Console.WriteLine("?? 3. SHARED ACCESS SIGNATURES");
        Console.WriteLine("   Prerequisites:");
        Console.WriteLine("   • Azure Storage Account");
        Console.WriteLine();
        Console.WriteLine("   Setup Commands:");
        Console.WriteLine("   ```");
        Console.WriteLine("   # Create storage account");
        Console.WriteLine("   az storage account create \\");
        Console.WriteLine("     --name staz204demo<unique> \\");
        Console.WriteLine("     --resource-group rg-az204-security \\");
        Console.WriteLine("     --location eastus \\");
        Console.WriteLine("     --sku Standard_LRS");
        Console.WriteLine();
        Console.WriteLine("   # Get account key");
        Console.WriteLine("   az storage account keys list \\");
        Console.WriteLine("     --account-name staz204demo<unique> \\");
        Console.WriteLine("     --resource-group rg-az204-security");
        Console.WriteLine("   ```");
        Console.WriteLine();

        Console.WriteLine("??  4. CONFIGURATION");
        Console.WriteLine();
        Console.WriteLine("   Update appsettings.json:");
        Console.WriteLine("   ```json");
        Console.WriteLine("   {");
        Console.WriteLine("     \"Azure\": {");
        Console.WriteLine("       \"TenantId\": \"your-tenant-id\",");
        Console.WriteLine("       \"ClientId\": \"your-app-registration-client-id\",");
        Console.WriteLine("       \"ClientSecret\": \"your-client-secret\",");
        Console.WriteLine("       \"SubscriptionId\": \"your-subscription-id\"");
        Console.WriteLine("     },");
        Console.WriteLine("     \"KeyVault\": {");
        Console.WriteLine("       \"VaultUri\": \"https://kv-az204-demo-<unique>.vault.azure.net/\"");
        Console.WriteLine("     }");
        Console.WriteLine("   }");
        Console.WriteLine("   ```");
        Console.WriteLine();
        Console.WriteLine("   Set Environment Variables:");
        Console.WriteLine("   ```");
        Console.WriteLine("   $env:AZURE_STORAGE_ACCOUNT_NAME=\"staz204demo<unique>\"");
        Console.WriteLine("   $env:AZURE_STORAGE_ACCOUNT_KEY=\"<your-storage-account-key>\"");
        Console.WriteLine("   ```");
        Console.WriteLine();

        Console.WriteLine("?? TIPS:");
        Console.WriteLine("   • Use 'az login' to authenticate locally");
        Console.WriteLine("   • Keep secrets in User Secrets or Azure Key Vault");
        Console.WriteLine("   • Test each demo individually before running all");
        Console.WriteLine("   • Check Azure costs - delete resources when done");
        Console.WriteLine();

        Console.WriteLine("\nPress any key to return to menu...");
        Console.ReadKey(true);
        Console.Clear();
        DisplayHeader();
    }

    static void DisplayExamTips()
    {
        Console.WriteLine("\n?? AZ-204 Exam Tips - Security Module\n");
        Console.WriteLine("???????????????????????????????????????????????????????????????????\n");

        Console.WriteLine("?? KEY EXAM TOPICS (20-25% of exam):\n");

        Console.WriteLine("1??  AZURE KEY VAULT");
        Console.WriteLine("   ? Know the difference between secrets, keys, and certificates");
        Console.WriteLine("   ? Understand soft delete and purge protection");
        Console.WriteLine("   ? Access policies vs. RBAC");
        Console.WriteLine("   ? Key rotation strategies");
        Console.WriteLine("   ? Using managed identities to access Key Vault");
        Console.WriteLine();

        Console.WriteLine("2??  MANAGED IDENTITIES");
        Console.WriteLine("   ? System-assigned vs. User-assigned");
        Console.WriteLine("   ? When to use each type");
        Console.WriteLine("   ? DefaultAzureCredential authentication chain");
        Console.WriteLine("   ? Granting permissions to managed identities");
        Console.WriteLine("   ? Using MI with Key Vault, Storage, SQL, etc.");
        Console.WriteLine();

        Console.WriteLine("3??  MICROSOFT IDENTITY PLATFORM (MSAL)");
        Console.WriteLine("   ? Public client vs. Confidential client applications");
        Console.WriteLine("   ? Authentication flows:");
        Console.WriteLine("      • Authorization Code Flow");
        Console.WriteLine("      • Client Credentials Flow");
        Console.WriteLine("      • Device Code Flow");
        Console.WriteLine("      • Implicit Flow (legacy)");
        Console.WriteLine("   ? Delegated vs. Application permissions");
        Console.WriteLine("   ? Token caching and refresh tokens");
        Console.WriteLine("   ? Scopes and consent");
        Console.WriteLine();

        Console.WriteLine("4??  SHARED ACCESS SIGNATURES (SAS)");
        Console.WriteLine("   ? Service SAS vs. Account SAS vs. User Delegation SAS");
        Console.WriteLine("   ? SAS token components and parameters");
        Console.WriteLine("   ? Best practices for SAS security");
        Console.WriteLine("   ? Stored access policies for revocation");
        Console.WriteLine("   ? Setting proper permissions and expiration");
        Console.WriteLine();

        Console.WriteLine("5??  MICROSOFT GRAPH");
        Console.WriteLine("   ? Common Graph API endpoints");
        Console.WriteLine("   ? Querying users, groups, mail, calendar");
        Console.WriteLine("   ? Batch requests");
        Console.WriteLine("   ? Delta queries for incremental sync");
        Console.WriteLine("   ? Throttling and error handling");
        Console.WriteLine();

        Console.WriteLine("?? IMPORTANT CONCEPTS:\n");
        Console.WriteLine("   • OAuth 2.0 and OpenID Connect fundamentals");
        Console.WriteLine("   • Azure AD app registrations and service principals");
        Console.WriteLine("   • Role-Based Access Control (RBAC)");
        Console.WriteLine("   • Conditional Access policies");
        Console.WriteLine("   • Multi-factor authentication (MFA)");
        Console.WriteLine("   • Certificate-based authentication");
        Console.WriteLine();

        Console.WriteLine("?? EXAM STRATEGIES:\n");
        Console.WriteLine("   1. Understand WHEN to use each authentication method");
        Console.WriteLine("   2. Know the security implications of each approach");
        Console.WriteLine("   3. Recognize scenarios requiring specific permissions");
        Console.WriteLine("   4. Practice using Azure CLI and PowerShell commands");
        Console.WriteLine("   5. Understand token lifetimes and renewal");
        Console.WriteLine("   6. Know how to troubleshoot common authentication errors");
        Console.WriteLine();

        Console.WriteLine("??  COMMON PITFALLS:");
        Console.WriteLine("   ? Confusing system-assigned and user-assigned identities");
        Console.WriteLine("   ? Not understanding delegated vs. application permissions");
        Console.WriteLine("   ? Hardcoding secrets instead of using Key Vault");
        Console.WriteLine("   ? Creating SAS tokens that are too permissive");
        Console.WriteLine("   ? Not implementing proper token refresh logic");
        Console.WriteLine();

        Console.WriteLine("?? USEFUL RESOURCES:");
        Console.WriteLine("   • Microsoft Learn: https://learn.microsoft.com/training/");
        Console.WriteLine("   • Azure SDK Documentation: https://docs.microsoft.com/azure/");
        Console.WriteLine("   • Microsoft Graph Explorer: https://developer.microsoft.com/graph/graph-explorer");
        Console.WriteLine();

        Console.WriteLine("\nPress any key to return to menu...");
        Console.ReadKey(true);
        Console.Clear();
        DisplayHeader();
    }
}
