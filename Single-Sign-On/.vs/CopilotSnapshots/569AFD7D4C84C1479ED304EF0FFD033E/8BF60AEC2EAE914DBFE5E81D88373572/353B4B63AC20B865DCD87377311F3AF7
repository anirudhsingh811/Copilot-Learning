using Microsoft.EntityFrameworkCore;
using OpenIddict.Abstractions;
using Single_Sign_On;
using static OpenIddict.Abstractions.OpenIddictConstants;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Configure Entity Framework with In-Memory database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseInMemoryDatabase("SSODatabase");
    options.UseOpenIddict();
});

// Configure OpenIddict
builder.Services.AddOpenIddict()
    .AddCore(options =>
    {
        options.UseEntityFrameworkCore()
            .UseDbContext<ApplicationDbContext>();
    })
    .AddServer(options =>
    {
        // Enable the authorization, token, and userinfo endpoints
        options.SetAuthorizationEndpointUris("/connect/authorize")
            .SetTokenEndpointUris("/connect/token")
            .SetUserinfoEndpointUris("/connect/userinfo")
            .SetIntrospectionEndpointUris("/connect/introspect");

        // Enable supported flows
        options.AllowAuthorizationCodeFlow()
            .AllowClientCredentialsFlow()
            .AllowRefreshTokenFlow()
            .AllowPasswordFlow();

        // Register scopes (permissions)
        options.RegisterScopes(Scopes.Email, Scopes.Profile, Scopes.Roles, "api");

        // Configure token lifetimes
        options.SetAccessTokenLifetime(TimeSpan.FromHours(1))
            .SetRefreshTokenLifetime(TimeSpan.FromDays(7));

        // Register signing and encryption credentials
        options.AddDevelopmentEncryptionCertificate()
            .AddDevelopmentSigningCertificate();

        // Register ASP.NET Core host and enable authorization/token endpoints
        options.UseAspNetCore()
            .EnableAuthorizationEndpointPassthrough()
            .EnableTokenEndpointPassthrough()
            .EnableUserinfoEndpointPassthrough()
            .DisableTransportSecurityRequirement(); // Only for development!
    })
    .AddValidation(options =>
    {
        options.UseLocalServer();
        options.UseAspNetCore();
    });

// Add authentication
builder.Services.AddAuthentication();

// Add hosted service to seed database
builder.Services.AddHostedService<SeedDataService>();

// Add CORS for on-prem clients
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapGet("/", () => Results.Json(new
{
    message = "SSO Server is running",
    endpoints = new
    {
        authorization = "/connect/authorize",
        token = "/connect/token",
        userinfo = "/connect/userinfo",
        introspection = "/connect/introspect"
    }
}));

app.Run();
