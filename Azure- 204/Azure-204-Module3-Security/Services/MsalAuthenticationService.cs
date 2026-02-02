using Microsoft.Identity.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Security;

namespace AZ204.Security.Services;

public class MsalAuthenticationService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<MsalAuthenticationService> _logger;
    private IPublicClientApplication? _publicClientApp;
    private IConfidentialClientApplication? _confidentialClientApp;

    public MsalAuthenticationService(IConfiguration configuration, ILogger<MsalAuthenticationService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task RunMsalDemoAsync()
    {
        try
        {
            _logger.LogInformation("=== MSAL Authentication Demo ===");
            Console.WriteLine("\n=== MSAL (Microsoft Authentication Library) Demo ===\n");

            await PublicClientDemo();
            await ConfidentialClientDemo();
            await DeviceCodeFlowDemo();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error running MSAL demo");
            Console.WriteLine($"\n? Error: {ex.Message}");
            Console.WriteLine("\n?? Setup required:");
            Console.WriteLine("   1. Create App Registration in Azure Portal");
            Console.WriteLine("   2. Update appsettings.json with TenantId and ClientId");
            Console.WriteLine("   3. For confidential client, add ClientSecret");
            Console.WriteLine("   4. Configure redirect URIs and API permissions");
        }
    }

    private async Task PublicClientDemo()
    {
        Console.WriteLine("?? Public Client Application (Interactive Authentication)\n");

        try
        {
            var tenantId = _configuration["Azure:TenantId"];
            var clientId = _configuration["Azure:ClientId"];

            if (string.IsNullOrEmpty(tenantId) || string.IsNullOrEmpty(clientId))
            {
                Console.WriteLine("??  TenantId or ClientId not configured. Skipping public client demo.\n");
                return;
            }

            _publicClientApp = PublicClientApplicationBuilder
                .Create(clientId)
                .WithAuthority($"https://login.microsoftonline.com/{tenantId}")
                .WithDefaultRedirectUri() // http://localhost
                .Build();

            Console.WriteLine($"Public Client App created");
            Console.WriteLine($"Authority: https://login.microsoftonline.com/{tenantId}");
            Console.WriteLine($"Client ID: {clientId}");

            // Define scopes
            string[] scopes = { "User.Read" };

            Console.WriteLine($"\nAttempting interactive authentication...");
            Console.WriteLine("(This will open a browser window for sign-in)");

            // Try to acquire token silently first (from cache)
            var accounts = await _publicClientApp.GetAccountsAsync();
            AuthenticationResult? result = null;

            if (accounts.Any())
            {
                Console.WriteLine($"Found {accounts.Count()} cached account(s)");
                try
                {
                    result = await _publicClientApp
                        .AcquireTokenSilent(scopes, accounts.FirstOrDefault())
                        .ExecuteAsync();
                    Console.WriteLine("? Token acquired silently from cache");
                }
                catch (MsalUiRequiredException)
                {
                    Console.WriteLine("Silent acquisition failed, interactive login required");
                }
            }

            // If silent acquisition failed, use interactive
            if (result == null)
            {
                result = await _publicClientApp
                    .AcquireTokenInteractive(scopes)
                    .WithPrompt(Prompt.SelectAccount)
                    .ExecuteAsync();
                Console.WriteLine("? Token acquired interactively");
            }

            DisplayAuthenticationResult(result);

            Console.WriteLine("\n");
        }
        catch (MsalException msalEx)
        {
            _logger.LogError(msalEx, "MSAL error in public client demo");
            Console.WriteLine($"? MSAL Error: {msalEx.ErrorCode} - {msalEx.Message}\n");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in public client demo");
            Console.WriteLine($"? Error: {ex.Message}\n");
        }
    }

    private async Task ConfidentialClientDemo()
    {
        Console.WriteLine("?? Confidential Client Application (Client Credentials Flow)\n");

        try
        {
            var tenantId = _configuration["Azure:TenantId"];
            var clientId = _configuration["Azure:ClientId"];
            var clientSecret = _configuration["Azure:ClientSecret"];

            if (string.IsNullOrEmpty(tenantId) || string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
            {
                Console.WriteLine("??  TenantId, ClientId, or ClientSecret not configured. Skipping confidential client demo.\n");
                return;
            }

            _confidentialClientApp = ConfidentialClientApplicationBuilder
                .Create(clientId)
                .WithClientSecret(clientSecret)
                .WithAuthority(new Uri($"https://login.microsoftonline.com/{tenantId}"))
                .Build();

            Console.WriteLine($"Confidential Client App created");
            Console.WriteLine($"Authority: https://login.microsoftonline.com/{tenantId}");
            Console.WriteLine($"Client ID: {clientId}");

            // Application permissions (not delegated)
            // This is typically used for daemon/service applications
            string[] scopes = { "https://graph.microsoft.com/.default" };

            Console.WriteLine($"\nAcquiring token using client credentials...");

            var result = await _confidentialClientApp
                .AcquireTokenForClient(scopes)
                .ExecuteAsync();

            Console.WriteLine("? Token acquired using client credentials");
            DisplayAuthenticationResult(result);

            Console.WriteLine("\n?? Note: This is app-only access (no user context)");
            Console.WriteLine("   Use this for background services and daemons\n");
        }
        catch (MsalException msalEx)
        {
            _logger.LogError(msalEx, "MSAL error in confidential client demo");
            Console.WriteLine($"? MSAL Error: {msalEx.ErrorCode} - {msalEx.Message}\n");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in confidential client demo");
            Console.WriteLine($"? Error: {ex.Message}\n");
        }
    }

    private async Task DeviceCodeFlowDemo()
    {
        Console.WriteLine("?? Device Code Flow (for devices without web browser)\n");

        try
        {
            var tenantId = _configuration["Azure:TenantId"];
            var clientId = _configuration["Azure:ClientId"];

            if (string.IsNullOrEmpty(tenantId) || string.IsNullOrEmpty(clientId))
            {
                Console.WriteLine("??  TenantId or ClientId not configured. Skipping device code flow demo.\n");
                return;
            }

            if (_publicClientApp == null)
            {
                _publicClientApp = PublicClientApplicationBuilder
                    .Create(clientId)
                    .WithAuthority($"https://login.microsoftonline.com/{tenantId}")
                    .Build();
            }

            string[] scopes = { "User.Read" };

            Console.WriteLine("Starting device code flow...");
            Console.WriteLine("This is useful for:");
            Console.WriteLine("  - Devices without web browser");
            Console.WriteLine("  - IoT devices");
            Console.WriteLine("  - CLI applications\n");

            var result = await _publicClientApp
                .AcquireTokenWithDeviceCode(scopes, deviceCodeResult =>
                {
                    Console.WriteLine($"\n{deviceCodeResult.Message}");
                    Console.WriteLine($"\nCode: {deviceCodeResult.UserCode}");
                    Console.WriteLine($"URL: {deviceCodeResult.VerificationUrl}");
                    Console.WriteLine($"Expires: {deviceCodeResult.ExpiresOn}\n");
                    return Task.CompletedTask;
                })
                .ExecuteAsync();

            Console.WriteLine("? Device code authentication successful");
            DisplayAuthenticationResult(result);

            Console.WriteLine("\n");
        }
        catch (MsalException msalEx)
        {
            _logger.LogError(msalEx, "MSAL error in device code flow demo");
            Console.WriteLine($"? MSAL Error: {msalEx.ErrorCode} - {msalEx.Message}\n");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in device code flow demo");
            Console.WriteLine($"? Error: {ex.Message}\n");
        }
    }

    private void DisplayAuthenticationResult(AuthenticationResult result)
    {
        Console.WriteLine($"\n?? Authentication Result:");
        Console.WriteLine($"   Account: {result.Account?.Username ?? "N/A (app-only)"}");
        Console.WriteLine($"   Token Type: {result.TokenType}");
        Console.WriteLine($"   Expires On: {result.ExpiresOn.LocalDateTime}");
        Console.WriteLine($"   Scopes: {string.Join(", ", result.Scopes)}");
        Console.WriteLine($"   Access Token (first 50 chars): {result.AccessToken[..Math.Min(50, result.AccessToken.Length)]}...");
        Console.WriteLine($"   ID Token Available: {!string.IsNullOrEmpty(result.IdToken)}");
        
        if (result.Account != null)
        {
            Console.WriteLine($"\n?? Account Details:");
            Console.WriteLine($"   Username: {result.Account.Username}");
            Console.WriteLine($"   Environment: {result.Account.Environment}");
            Console.WriteLine($"   Home Account ID: {result.Account.HomeAccountId.Identifier}");
        }
    }
}
