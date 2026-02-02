# Azure Web App Authentication & Identity Providers Demo

## ?? Overview

Comprehensive demo suite for **Azure App Service Authentication** (Easy Auth) covering all aspects of authentication and authorization for the **AZ-204 certification exam**.

## ? Features

This demo includes **7 complete modules** covering:

### 1. Enable Authentication (Azure Portal)
- Step-by-step portal configuration
- Understanding authentication options
- Testing authentication flow
- Important endpoints (/.auth/login, /.auth/logout, /.auth/me)

### 2. Microsoft Entra ID (Azure AD) Configuration
- Single vs Multi-tenant scenarios
- App registration setup
- API permissions configuration
- Accessing user claims in .NET 8/9
- Token configuration

### 3. Social Identity Providers
- Google authentication setup
- Facebook integration
- GitHub, Twitter, Apple support
- Client secret management
- Key Vault integration

### 4. Access Tokens & API Authentication
- Understanding ID, Access, and Refresh tokens
- Token store configuration
- Calling Microsoft Graph API
- API-to-API authentication
- Token refresh strategies

### 5. Authorization & RBAC
- Authentication vs Authorization
- Defining Azure AD app roles
- Role-based access control (RBAC)
- Policy-based authorization
- Resource-based authorization

### 6. Azure CLI Commands
- Complete CLI reference
- Authentication configuration scripts
- Automation examples
- Best practices for scripting

### 7. Best Practices & Troubleshooting
- Security best practices
- Common issues and solutions
- Troubleshooting tools
- AZ-204 exam cheat sheet

## ?? Running the Demo

### From Main Menu
```bash
dotnet run --project "Azure- 204/Azure- 204.csproj"
# Select: 1. Azure App Service
# Select: 6. ?? Authentication & Identity Providers
```

### Standalone
```csharp
await WebAppAuthenticationDemoRunner.RunAsync();
```

## ?? Demo Menu

```
???????????????????????????????????????????????????????
AZURE WEB APP AUTHENTICATION DEMOS
???????????????????????????????????????????????????????
1. Demo 1: Enable Authentication (Azure Portal)
2. Demo 2: Microsoft Entra ID Configuration
3. Demo 3: Social Identity Providers
4. Demo 4: Access Tokens & API Authentication
5. Demo 5: Authorization & RBAC
6. Demo 6: Azure CLI Authentication Commands
7. Demo 7: Best Practices & Troubleshooting
8. Run All Demos
0. Exit
???????????????????????????????????????????????????????
```

## ?? AZ-204 Exam Coverage

### Key Topics
- ? App Service Authentication (Easy Auth)
- ? Microsoft Entra ID integration
- ? OAuth 2.0 and OpenID Connect
- ? Token management
- ? Claims and roles
- ? API authentication
- ? Multi-tenant applications

### Common Exam Scenarios
1. **Secure web app with Azure AD**
   - Add Microsoft identity provider
   - Configure single tenant
   - Require authentication

2. **Frontend calling backend API**
   - Enable authentication on both
   - Pass access token in Authorization header
   - Validate tokens on backend

3. **Role-based access control**
   - Define app roles in Azure AD
   - Use `[Authorize(Roles = "Admin")]`
   - Access roles in user claims

4. **Social login**
   - Configure Google/Facebook providers
   - Set redirect URIs correctly
   - Store client secrets securely

## ?? Code Examples

### .NET 8/9 Minimal API
```csharp
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();

// Require authentication
app.MapGet("/api/secure", () => "Secure data")
    .RequireAuthorization();

// Get user info
app.MapGet("/api/user", (HttpContext context) =>
{
    var userId = context.Request.Headers["X-MS-CLIENT-PRINCIPAL-ID"];
    var userName = context.Request.Headers["X-MS-CLIENT-PRINCIPAL-NAME"];
    return Results.Ok(new { userId, userName });
});

app.Run();
```

### Controller-Based (.NET 8/9)
```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DataController(ILogger<DataController> logger) : ControllerBase
{
    [HttpGet]
    public IActionResult GetData()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Ok(new { userId, data = "secure" });
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public IActionResult CreateData()
    {
        // Only admins
        return Created();
    }
}
```

### Call Microsoft Graph API
```csharp
public class GraphService(IHttpContextAccessor contextAccessor, IHttpClientFactory factory)
{
    public async Task<UserProfile?> GetUserProfile()
    {
        var context = contextAccessor.HttpContext;
        if (context is null) return null;
        
        var token = context.Request.Headers["X-MS-TOKEN-AAD-ACCESS-TOKEN"].ToString();
        if (string.IsNullOrEmpty(token)) return null;
        
        var httpClient = factory.CreateClient();
        httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
        
        return await httpClient.GetFromJsonAsync<UserProfile>(
            "https://graph.microsoft.com/v1.0/me");
    }
}
```

## ?? Security Best Practices

### ? DO
- Use HTTPS for all authentication endpoints
- Enable token store for token management
- Store secrets in Azure Key Vault
- Use Managed Identity where possible
- Request minimum necessary scopes
- Enable MFA for admin accounts
- Rotate client secrets regularly
- Use single tenant for internal apps

### ? DON'T
- Don't expose tokens in URLs or logs
- Don't use HTTP for authentication
- Don't store secrets in code
- Don't request excessive permissions
- Don't skip token validation
- Don't cache tokens insecurely

## ?? Troubleshooting

### Common Issues

**Issue: Redirect URI Mismatch**
```
Error: AADSTS50011: Redirect URI mismatch
Solution: Verify exact match in Azure AD
Format: https://<app>.azurewebsites.net/.auth/login/<provider>/callback
```

**Issue: Token Expired**
```
Symptoms: 401 errors after 1 hour
Solution: 
- Enable token store
- Set token refresh extension hours
- Check refresh token configuration
```

**Issue: Claims Missing**
```
Symptoms: Email/roles not in claims
Solution:
- Add optional claims in token configuration
- Request appropriate scopes
- Grant API permissions
- Check /.auth/me endpoint
```

### Useful Endpoints
- `/.auth/login/<provider>` - Login
- `/.auth/logout` - Logout
- `/.auth/me` - User info and tokens
- `/.auth/refresh` - Refresh token

### HTTP Headers
- `X-MS-CLIENT-PRINCIPAL-ID` - User ID
- `X-MS-CLIENT-PRINCIPAL-NAME` - User name
- `X-MS-CLIENT-PRINCIPAL-EMAIL` - User email
- `X-MS-TOKEN-AAD-ACCESS-TOKEN` - Azure AD access token
- `X-MS-CLIENT-PRINCIPAL` - Base64 encoded principal

## ?? Additional Resources

### Microsoft Documentation
- [App Service Authentication](https://learn.microsoft.com/azure/app-service/overview-authentication-authorization)
- [Microsoft Entra ID](https://learn.microsoft.com/entra/identity-platform/)
- [Configure Azure AD authentication](https://learn.microsoft.com/azure/app-service/configure-authentication-provider-aad)

### Azure CLI Reference
```bash
# View authentication settings
az webapp auth show --resource-group <rg> --name <app>

# Enable Azure AD authentication
az webapp auth update \
  --resource-group <rg> \
  --name <app> \
  --enabled true \
  --action LoginWithAzureActiveDirectory

# Enable token store
az webapp auth update \
  --resource-group <rg> \
  --name <app> \
  --token-store true
```

## ?? Exam Tips

### Remember These
- **Easy Auth** = Zero-code authentication
- **Token Store** = Enable for token refresh
- **Single Tenant** = Internal apps
- **Multi-Tenant** = SaaS apps
- **Delegated Permissions** = On behalf of user
- **Application Permissions** = App identity

### Common Exam Questions
1. How to enable authentication without code changes?
   ? Add identity provider in portal

2. How to pass user context to backend API?
   ? Get token from headers, pass in Authorization header

3. How to restrict access by role?
   ? Define app roles, use [Authorize(Roles = "Admin")]

4. What's the redirect URI format?
   ? https://<app>.azurewebsites.net/.auth/login/<provider>/callback

## ??? Technologies Used

- **.NET 8 / .NET 9** - Modern C# features
- **Azure App Service** - Hosting platform
- **Microsoft Entra ID** - Identity provider
- **OAuth 2.0 / OpenID Connect** - Authentication protocols
- **Microsoft Graph** - User data API

## ?? Notes

This demo is designed for:
- AZ-204 exam preparation
- Understanding App Service authentication
- Learning best practices
- Hands-on Azure AD integration
- Real-world scenarios

All code examples use modern .NET 8/9 patterns including:
- Primary constructors
- Top-level statements
- Minimal APIs
- File-scoped namespaces
- Record types

## ?? Contributing

This is part of the comprehensive AZ-204 study guide. Each demo includes:
- Visual ASCII diagrams
- Step-by-step instructions
- Code examples
- Best practices
- Troubleshooting tips
- Exam-focused content

Perfect for experienced developers preparing for the AZ-204 certification!
