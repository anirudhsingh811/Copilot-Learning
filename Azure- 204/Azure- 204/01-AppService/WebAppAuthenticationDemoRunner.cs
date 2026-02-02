namespace AZ204.AppService;

/// <summary>
/// Interactive demo runner for Azure Web App Authentication
/// Compatible with .NET 8 and .NET 9
/// </summary>
public static class WebAppAuthenticationDemoRunner
{
    public static async Task RunAsync()
    {
        Console.Clear();
        await WebAppAuthenticationDemo.ShowAuthenticationOverview();

        while (true)
        {
            Console.WriteLine("\n???????????????????????????????????????????????????????");
            Console.WriteLine("AZURE WEB APP AUTHENTICATION DEMOS");
            Console.WriteLine("???????????????????????????????????????????????????????");
            Console.WriteLine("1. Demo 1: Enable Authentication (Azure Portal)");
            Console.WriteLine("2. Demo 2: Microsoft Entra ID Configuration");
            Console.WriteLine("3. Demo 3: Social Identity Providers");
            Console.WriteLine("4. Demo 4: Access Tokens & API Authentication");
            Console.WriteLine("5. Demo 5: Authorization & RBAC");
            Console.WriteLine("6. Demo 6: Azure CLI Authentication Commands");
            Console.WriteLine("7. Demo 7: Best Practices & Troubleshooting");
            Console.WriteLine("8. Run All Demos");
            Console.WriteLine("0. Exit");
            Console.WriteLine("???????????????????????????????????????????????????????");
            Console.Write("\nEnter your choice (0-8): ");

            var choice = Console.ReadLine();

            try
            {
                switch (choice)
                {
                    case "1":
                        await WebAppAuthenticationDemo.Demo1_EnableAuthentication();
                        break;
                    case "2":
                        await WebAppAuthenticationDemo.Demo2_ConfigureMicrosoftEntraID();
                        break;
                    case "3":
                        await WebAppAuthenticationDemo.Demo3_SocialIdentityProviders();
                        break;
                    case "4":
                        await WebAppAuthenticationDemo.Demo4_AccessTokensAndAPI();
                        break;
                    case "5":
                        await WebAppAuthenticationDemo.Demo5_AuthorizationAndRBAC();
                        break;
                    case "6":
                        await WebAppAuthenticationDemo.Demo6_AzureCliAuthentication();
                        break;
                    case "7":
                        await WebAppAuthenticationDemo.Demo7_BestPracticesAndTroubleshooting();
                        break;
                    case "8":
                        await RunAllDemos();
                        break;
                    case "0":
                        Console.WriteLine("\n? Good luck with AZ-204! ??\n");
                        return;
                    default:
                        Console.WriteLine("\n? Invalid choice. Please try again.\n");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n? Error: {ex.Message}\n");
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
            Console.Clear();
        }
    }

    private static async Task RunAllDemos()
    {
        Console.WriteLine("\n?? Running all demos sequentially...\n");
        
        await WebAppAuthenticationDemo.Demo1_EnableAuthentication();
        Console.WriteLine("\nPress any key to continue to next demo...");
        Console.ReadKey();
        
        await WebAppAuthenticationDemo.Demo2_ConfigureMicrosoftEntraID();
        Console.WriteLine("\nPress any key to continue to next demo...");
        Console.ReadKey();
        
        await WebAppAuthenticationDemo.Demo3_SocialIdentityProviders();
        Console.WriteLine("\nPress any key to continue to next demo...");
        Console.ReadKey();
        
        await WebAppAuthenticationDemo.Demo4_AccessTokensAndAPI();
        Console.WriteLine("\nPress any key to continue to next demo...");
        Console.ReadKey();
        
        await WebAppAuthenticationDemo.Demo5_AuthorizationAndRBAC();
        Console.WriteLine("\nPress any key to continue to next demo...");
        Console.ReadKey();
        
        await WebAppAuthenticationDemo.Demo6_AzureCliAuthentication();
        Console.WriteLine("\nPress any key to continue to next demo...");
        Console.ReadKey();
        
        await WebAppAuthenticationDemo.Demo7_BestPracticesAndTroubleshooting();
        
        Console.WriteLine("\n?? All demos completed!");
    }
}
