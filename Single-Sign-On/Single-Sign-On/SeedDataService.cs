using Microsoft.EntityFrameworkCore;
using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Single_Sign_On;

public class SeedDataService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;

    public SeedDataService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await context.Database.EnsureCreatedAsync(cancellationToken);

        var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();

        // Register on-prem client application
        if (await manager.FindByClientIdAsync("onprem-client", cancellationToken) is null)
        {
            await manager.CreateAsync(new OpenIddictApplicationDescriptor
            {
                ClientId = "onprem-client",
                ClientSecret = "onprem-secret-key-12345",
                DisplayName = "On-Premises Client Application",
                RedirectUris = 
                { 
                    new Uri("http://localhost:5001/signin-oidc"),
                    new Uri("https://localhost:5001/signin-oidc")
                },
                PostLogoutRedirectUris = 
                { 
                    new Uri("http://localhost:5001/signout-callback-oidc"),
                    new Uri("https://localhost:5001/signout-callback-oidc")
                },
                Permissions =
                {
                    Permissions.Endpoints.Authorization,
                    Permissions.Endpoints.Token,
                    Permissions.Endpoints.Introspection,
                    Permissions.GrantTypes.AuthorizationCode,
                    Permissions.GrantTypes.ClientCredentials,
                    Permissions.GrantTypes.RefreshToken,
                    Permissions.ResponseTypes.Code,
                    Permissions.Scopes.Email,
                    Permissions.Scopes.Profile,
                    Permissions.Scopes.Roles,
                    Permissions.Prefixes.Scope + "api"
                },
                Requirements =
                {
                    Requirements.Features.ProofKeyForCodeExchange
                }
            }, cancellationToken);
        }

        // Register API client (for service-to-service communication)
        if (await manager.FindByClientIdAsync("api-client", cancellationToken) is null)
        {
            await manager.CreateAsync(new OpenIddictApplicationDescriptor
            {
                ClientId = "api-client",
                ClientSecret = "api-secret-key-67890",
                DisplayName = "API Client",
                Permissions =
                {
                    Permissions.Endpoints.Token,
                    Permissions.Endpoints.Introspection,
                    Permissions.GrantTypes.ClientCredentials,
                    Permissions.Prefixes.Scope + "api"
                }
            }, cancellationToken);
        }

        Console.WriteLine("? SSO Server initialized successfully");
        Console.WriteLine("? Registered clients: onprem-client, api-client");
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
