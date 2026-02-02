using Azure.Identity;
using Azure.ResourceManager;
using Azure.ResourceManager.Resources;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

/// Learning Summary: /// This code demonstrates the use of Managed Identities in Azure for secure authentication. 
/// And it covers both system-assigned and user-assigned managed identities, explaining their characteristics and usage scenarios.
/// What is Managed Identity: Managed Identities provide an automatically managed identity in Azure AD for applications to use when connecting to resources that support Azure AD authentication.
/// How is it different from Principal-based authentication: Managed Identities eliminate the need for developers to manage credentials, reducing security risks associated with hard-coded secrets.
/// What is principal-based authentication: Principal-based authentication typically involves using service principals with client IDs and secrets or certificates to authenticate applications.
/// Difference b/w Managed Identity , Service principal and RBAC: Managed Identities are a type of service principal managed by Azure, while RBAC (Role-Based Access Control) is a broader authorization framework that defines permissions for users and applications in Azure.
namespace AZ204.Security.Services;

/// <summary>
/// Service class that demonstrates various Azure Managed Identity scenarios and authentication patterns
/// </summary>
public class ManagedIdentityService
{
    // Holds application configuration settings (e.g., subscription ID, connection strings)
    private readonly IConfiguration _configuration;
    
    // Logger instance for recording diagnostic information and errors
    private readonly ILogger<ManagedIdentityService> _logger;

    /// <summary>
    /// Constructor that initializes the service with required dependencies
    /// </summary>
    /// <param name="configuration">Application configuration for accessing settings</param>
    /// <param name="logger">Logger for diagnostic output</param>
    public ManagedIdentityService(IConfiguration configuration, ILogger<ManagedIdentityService> logger)
    {
        // Store the configuration instance for later use in the service
        _configuration = configuration;
        
        // Store the logger instance for recording events throughout the service lifecycle
        _logger = logger;
    }

    /// <summary>
    /// Main entry point that orchestrates all managed identity demonstrations
    /// </summary>
    public async Task RunManagedIdentityDemoAsync()
    {
        try
        {
            // Log the start of the demo to the logging system
            _logger.LogInformation("=== Managed Identity Demo ===");
            
            // Display a header to the console for visual separation
            Console.WriteLine("\n=== Managed Identity Demo ===\n");

            // Display introductory information about Managed Identities
            Console.WriteLine("🔐 Understanding Managed Identities:\n");
            
            // Explain what Managed Identities are
            Console.WriteLine("Managed Identities provide an automatically managed identity in Azure AD");
            Console.WriteLine("for applications to use when connecting to resources that support Azure AD authentication.\n");

            // Display the two types of Managed Identities available
            Console.WriteLine("Types of Managed Identities:");
            
            // Explain System-Assigned identity - tied to a single resource's lifecycle
            Console.WriteLine("  1. System-Assigned: Lifecycle tied to the Azure resource");
            
            // Explain User-Assigned identity - standalone resource that can be shared
            Console.WriteLine("  2. User-Assigned: Standalone identity that can be assigned to multiple resources\n");

            // Execute the System-Assigned Managed Identity demonstration
            await SystemAssignedIdentityDemo();
            
            // Execute the User-Assigned Managed Identity demonstration
            await UserAssignedIdentityDemo();
            
            // Execute the DefaultAzureCredential demonstration (recommended approach)
            await DefaultAzureCredentialDemo();
        }
        catch (Exception ex)
        {
            // Log any exceptions that occur during the demo execution
            _logger.LogError(ex, "Error running Managed Identity demo");
            
            // Display the error message to the console for immediate visibility
            Console.WriteLine($"\n❌ Error: {ex.Message}");
        }
    }

    /// <summary>
    /// Demonstrates System-Assigned Managed Identity features and usage patterns
    /// </summary>
    private async Task SystemAssignedIdentityDemo()
    {
        // Display section header for System-Assigned Managed Identity
        Console.WriteLine("🔑 System-Assigned Managed Identity\n");

        try
        {
            // Display the key characteristics of System-Assigned Managed Identities
            Console.WriteLine("System-assigned managed identity characteristics:");
            
            // Automatically created when you enable it on an Azure resource
            Console.WriteLine("  ✓ Automatically created and managed by Azure");
            
            // Lives and dies with the resource it's assigned to
            Console.WriteLine("  ✓ Tied to the lifecycle of the Azure resource");
            
            // Cannot outlive the resource - prevents orphaned identities
            Console.WriteLine("  ✓ Deleted when the resource is deleted");
            
            // One resource = one identity (1:1 relationship)
            Console.WriteLine("  ✓ Cannot be shared across multiple resources\n");

            // Show example code for using System-Assigned identity
            Console.WriteLine("Usage example:");
            Console.WriteLine(@"
// In Azure App Service, Function App, VM, etc.
var credential = new ManagedIdentityCredential();

// Use with Azure SDK clients
var secretClient = new SecretClient(vaultUri, credential);
var blobClient = new BlobServiceClient(storageUri, credential);
");

            // Inform user we're checking the environment
            Console.WriteLine("Checking if running in Azure environment with managed identity...");
            
            // Check for IDENTITY_ENDPOINT environment variable (App Service, Functions)
            var identityEndpoint = Environment.GetEnvironmentVariable("IDENTITY_ENDPOINT");
            
            // Check for IDENTITY_HEADER environment variable (used with IDENTITY_ENDPOINT)
            var identityHeader = Environment.GetEnvironmentVariable("IDENTITY_HEADER");
            
            // Check for MSI_ENDPOINT environment variable (older VM implementation)
            var msiEndpoint = Environment.GetEnvironmentVariable("MSI_ENDPOINT");

            // Determine if we're running in an environment that supports Managed Identity
            if (!string.IsNullOrEmpty(identityEndpoint) || !string.IsNullOrEmpty(msiEndpoint))
            {
                // Notify that Managed Identity is available in this environment
                Console.WriteLine("✅ Managed Identity environment detected!");
                
                // Display which endpoint is being used for token acquisition
                Console.WriteLine($"   Identity Endpoint: {identityEndpoint ?? msiEndpoint}");
                
                // Attempt to acquire an access token using Managed Identity
                try
                {
                    // Create a credential object for System-Assigned Managed Identity
                    var credential = new ManagedIdentityCredential();
                    
                    // Request an access token for Azure Resource Manager
                    var token = await credential.GetTokenAsync(
                        new Azure.Core.TokenRequestContext(new[] { "https://management.azure.com/.default" }));
                    
                    // Indicate successful token acquisition
                    Console.WriteLine($"✅ Successfully acquired token using Managed Identity");
                    
                    // Display when the token will expire (important for token refresh logic)
                    Console.WriteLine($"   Token expires: {token.ExpiresOn}");
                }
                catch (Exception ex)
                {
                    // Handle token acquisition failures (permissions, configuration issues)
                    Console.WriteLine($"⚠️  Could not acquire token: {ex.Message}");
                }
            }
            else
            {
                // Not running in an Azure environment with Managed Identity
                Console.WriteLine("⚠️  Not running in Azure managed identity environment");
                
                // Inform user about where this needs to run
                Console.WriteLine("   This demo needs to run in:");
                
                // List Azure services that support System-Assigned Managed Identity
                Console.WriteLine("   - Azure App Service");
                Console.WriteLine("   - Azure Functions");
                Console.WriteLine("   - Azure Virtual Machine");
                Console.WriteLine("   - Azure Container Instances");
                Console.WriteLine("   - Azure Kubernetes Service");
            }

            // Add spacing for readability
            Console.WriteLine("\n");
        }
        catch (Exception ex)
        {
            // Log any errors that occur during the System-Assigned identity demo
            _logger.LogError(ex, "Error in system-assigned identity demo");
            
            // Display error to console
            Console.WriteLine($"❌ Error: {ex.Message}\n");
        }
    }

    /// <summary>
    /// Demonstrates User-Assigned Managed Identity features and usage patterns
    /// </summary>
    private async Task UserAssignedIdentityDemo()
    {
        // Display section header for User-Assigned Managed Identity
        Console.WriteLine("🔑 User-Assigned Managed Identity\n");

        try
        {
            // Display the key characteristics of User-Assigned Managed Identities
            Console.WriteLine("User-assigned managed identity characteristics:");
            
            // Created independently as its own Azure resource
            Console.WriteLine("  ✓ Created as standalone Azure resource");
            
            // Not deleted when assigned resources are deleted
            Console.WriteLine("  ✓ Independent lifecycle from resources that use it");
            
            // Can assign the same identity to multiple resources
            Console.WriteLine("  ✓ Can be shared across multiple resources");
            
            // Provides more flexibility in identity management
            Console.WriteLine("  ✓ More control and flexibility\n");

            // Show example code for using User-Assigned identity
            Console.WriteLine("Usage example:");
            Console.WriteLine(@"
// Specify the Client ID of the user-assigned identity
var credential = new ManagedIdentityCredential(""<client-id-of-user-assigned-identity>"");

// Or use DefaultAzureCredential which will detect it automatically
var credential = new DefaultAzureCredential();
");

            // Show Azure CLI commands for creating and assigning User-Assigned identity
            Console.WriteLine("Creating a user-assigned identity via Azure CLI:");
            Console.WriteLine(@"
az identity create \
  --name myUserAssignedIdentity \
  --resource-group myResourceGroup

# Assign it to a resource (e.g., VM)
az vm identity assign \
  --name myVM \
  --resource-group myResourceGroup \
  --identities /subscriptions/{subscription-id}/resourcegroups/{resource-group}/providers/Microsoft.ManagedIdentity/userAssignedIdentities/myUserAssignedIdentity
");

            // Check if a User-Assigned identity client ID is configured via environment variable
            var identityClientId = Environment.GetEnvironmentVariable("AZURE_CLIENT_ID");
            
            // If the AZURE_CLIENT_ID environment variable is set
            if (!string.IsNullOrEmpty(identityClientId))
            {
                // Notify that a User-Assigned Identity was found
                Console.WriteLine($"\n✅ User-Assigned Identity detected!");
                
                // Display the Client ID of the detected identity
                Console.WriteLine($"   Client ID: {identityClientId}");

                // Attempt to acquire a token using the User-Assigned identity
                try
                {
                    // Create credential with the specific User-Assigned identity Client ID
                    var credential = new ManagedIdentityCredential(identityClientId);
                    
                    // Request an access token for Azure Resource Manager
                    var token = await credential.GetTokenAsync(
                        new Azure.Core.TokenRequestContext(new[] { "https://management.azure.com/.default" }));
                    
                    // Indicate successful token acquisition
                    Console.WriteLine($"✅ Successfully acquired token using User-Assigned Managed Identity");
                    
                    // Display token expiration time
                    Console.WriteLine($"   Token expires: {token.ExpiresOn}");
                }
                catch (Exception ex)
                {
                    // Handle token acquisition failures
                    Console.WriteLine($"⚠️  Could not acquire token: {ex.Message}");
                }
            }
            else
            {
                // No User-Assigned identity is configured
                Console.WriteLine("\n⚠️  No user-assigned identity configured");
            }

            // Add spacing for readability
            Console.WriteLine("\n");
        }
        catch (Exception ex)
        {
            // Log any errors that occur during the User-Assigned identity demo
            _logger.LogError(ex, "Error in user-assigned identity demo");
            
            // Display error to console
            Console.WriteLine($"❌ Error: {ex.Message}\n");
        }
    }

    /// <summary>
    /// Demonstrates DefaultAzureCredential which automatically tries multiple authentication methods
    /// </summary>
    private async Task DefaultAzureCredentialDemo()
    {
        // Display section header for DefaultAzureCredential
        Console.WriteLine("🔑 DefaultAzureCredential (Recommended)\n");

        try
        {
            // Explain that DefaultAzureCredential tries multiple auth methods in sequence
            Console.WriteLine("DefaultAzureCredential automatically tries multiple authentication methods:");
            
            // List all credential sources in the order they are tried
            Console.WriteLine("  1. EnvironmentCredential - Environment variables");
            Console.WriteLine("  2. ManagedIdentityCredential - Managed Identity");
            Console.WriteLine("  3. SharedTokenCacheCredential - Cached tokens");
            Console.WriteLine("  4. VisualStudioCredential - Visual Studio");
            Console.WriteLine("  5. VisualStudioCodeCredential - VS Code");
            Console.WriteLine("  6. AzureCliCredential - Azure CLI");
            Console.WriteLine("  7. AzurePowerShellCredential - Azure PowerShell");
            Console.WriteLine("  8. InteractiveBrowserCredential - Browser (last resort)\n");

            // Explain the benefits of using DefaultAzureCredential
            Console.WriteLine("This provides seamless authentication across:");
            
            // Works in local dev environments using developer credentials
            Console.WriteLine("  ✓ Local development (using Azure CLI, Visual Studio, etc.)");
            
            // Works in Azure using Managed Identity (no secrets needed)
            Console.WriteLine("  ✓ Azure production (using Managed Identity)");
            
            // Works in CI/CD using Service Principal credentials
            Console.WriteLine("  ✓ CI/CD pipelines (using Service Principal)\n");

            // Show example code for using DefaultAzureCredential
            Console.WriteLine("Usage example:");
            Console.WriteLine(@"
var credential = new DefaultAzureCredential();

// Works everywhere!
var secretClient = new SecretClient(vaultUri, credential);
var blobClient = new BlobServiceClient(storageUri, credential);
var graphClient = new GraphServiceClient(credential);
");

            // Inform user we're attempting authentication
            Console.WriteLine("Attempting authentication with DefaultAzureCredential...");

            // Attempt to authenticate using DefaultAzureCredential
            try
            {
                // Create DefaultAzureCredential with options to exclude interactive browser
                var credential = new DefaultAzureCredential(new DefaultAzureCredentialOptions
                {
                    ExcludeInteractiveBrowserCredential = true // Don't pop up browser for this demo
                });

                // Request an access token for Azure Resource Manager to test authentication
                var token = await credential.GetTokenAsync(
                    new Azure.Core.TokenRequestContext(new[] { "https://management.azure.com/.default" }));
                
                // Indicate successful authentication
                Console.WriteLine($"\n✅ Successfully authenticated using DefaultAzureCredential!");
                
                // Display when the acquired token expires
                Console.WriteLine($"   Token expires: {token.ExpiresOn}");
                
                // Explain that one of the credential sources worked
                Console.WriteLine($"   This means one of the credential sources is working.");

                // Try to retrieve the subscription ID from configuration
                var subscriptionId = _configuration["Azure:SubscriptionId"];
                
                // If a subscription ID is configured, test actual Azure API access
                if (!string.IsNullOrEmpty(subscriptionId))
                {
                    // Inform user we're testing Azure Resource Manager access
                    Console.WriteLine($"\nTesting access to Azure Resource Manager...");
                    
                    // Create ARM client using the authenticated credential
                    var armClient = new ArmClient(credential);
                    
                    // Get a reference to the specified subscription
                    var subscription = armClient.GetSubscriptionResource(
                        Azure.Core.ResourceIdentifier.Parse($"/subscriptions/{subscriptionId}"));
                    
                    // Indicate successful connection to the subscription
                    Console.WriteLine($"✅ Successfully connected to subscription");
                    
                    // Inform user we're listing resource groups
                    Console.WriteLine("\nListing resource groups:");
                    
                    // Get collection of resource groups in the subscription
                    var resourceGroups = subscription.GetResourceGroups();
                    
                    // Counter for limiting the number of displayed resource groups
                    int count = 0;
                    
                    // Iterate through resource groups asynchronously
                    await foreach (var rg in resourceGroups)
                    {
                        // Increment counter for each resource group
                        count++;
                        
                        // Display resource group name and location
                        Console.WriteLine($"   {count}. {rg.Data.Name} ({rg.Data.Location})");
                        
                        // Limit output to first 5 resource groups to avoid cluttering console
                        if (count >= 5) break;
                    }

                    // If no resource groups were found, display a message
                    if (count == 0)
                    {
                        Console.WriteLine("   No resource groups found");
                    }
                }
            }
            catch (Azure.Identity.CredentialUnavailableException ex)
            {
                // Handle case where no credentials are available
                Console.WriteLine($"\n⚠️  No credentials available: {ex.Message}");
                
                // Provide instructions for setting up local authentication
                Console.WriteLine("\n💡 To use DefaultAzureCredential locally:");
                Console.WriteLine("   1. Install Azure CLI: https://aka.ms/install-azure-cli");
                Console.WriteLine("   2. Run: az login");
                Console.WriteLine("   3. Run this application again");
            }
            catch (Exception ex)
            {
                // Handle any other authentication failures
                Console.WriteLine($"\n⚠️  Authentication failed: {ex.Message}");
            }

            // Add spacing for readability
            Console.WriteLine("\n");
        }
        catch (Exception ex)
        {
            // Log any errors that occur during the DefaultAzureCredential demo
            _logger.LogError(ex, "Error in DefaultAzureCredential demo");
            
            // Display error to console
            Console.WriteLine($"❌ Error: {ex.Message}\n");
        }
    }
}
