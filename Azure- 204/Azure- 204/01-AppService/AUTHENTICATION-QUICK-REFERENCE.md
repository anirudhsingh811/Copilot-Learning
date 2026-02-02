# ?? Azure App Service Authentication - Quick Reference

## ?? AZ-204 Exam Cheat Sheet

### Key Concepts
| Concept | Description |
|---------|-------------|
| **Easy Auth** | Zero-code authentication built into App Service |
| **Identity Provider** | Microsoft Entra ID, Google, Facebook, etc. |
| **Token Store** | Securely stores and manages tokens |
| **Claims** | User attributes (name, email, roles) |
| **RBAC** | Role-Based Access Control |

### Supported Identity Providers
| Provider | Priority | Use Case |
|----------|----------|----------|
| Microsoft Entra ID | ??? | Enterprise, B2B, most common for AZ-204 |
| Google | ?? | Consumer apps, social login |
| Facebook | ?? | Consumer apps, social media |
| GitHub | ? | Developer tools |
| OpenID Connect | ?? | Custom providers |

### Authentication Flow
```
1. User ? App
2. App Service checks auth
3. Not authenticated ? Redirect to provider
4. User signs in
5. Provider ? Token ? App Service
6. App Service validates token
7. Token stored (if enabled)
8. Request ? Your app (with user headers)
```

### Important Endpoints
| Endpoint | Purpose |
|----------|---------|
| `/.auth/login/<provider>` | Initiate login |
| `/.auth/logout` | Logout user |
| `/.auth/me` | Get user info & tokens |
| `/.auth/refresh` | Refresh access token |

### HTTP Headers
| Header | Contains |
|--------|----------|
| `X-MS-CLIENT-PRINCIPAL-ID` | User ID |
| `X-MS-CLIENT-PRINCIPAL-NAME` | Username |
| `X-MS-CLIENT-PRINCIPAL-EMAIL` | Email |
| `X-MS-TOKEN-AAD-ACCESS-TOKEN` | Azure AD access token |
| `X-MS-TOKEN-GOOGLE-ACCESS-TOKEN` | Google access token |
| `X-MS-CLIENT-PRINCIPAL` | Base64 encoded principal |

## ?? Configuration Steps

### Azure Portal
```
1. App Service ? Authentication
2. Add identity provider
3. Select provider (Microsoft recommended)
4. Configure:
   ? Require authentication (for private apps)
   ? Enable token store
5. Save
```

### Azure CLI
```bash
# Enable authentication
az webapp auth update \
  --resource-group myRG \
  --name myApp \
  --enabled true \
  --action LoginWithAzureActiveDirectory \
  --aad-client-id <client-id> \
  --aad-client-secret <secret>

# Enable token store
az webapp auth update \
  --resource-group myRG \
  --name myApp \
  --token-store true \
  --token-refresh-extension-hours 72
```

## ?? Code Examples

### .NET 8/9 - Get User Info
```csharp
// Minimal API
app.MapGet("/user", (HttpContext context) =>
{
    var userId = context.Request.Headers["X-MS-CLIENT-PRINCIPAL-ID"];
    var userName = context.Request.Headers["X-MS-CLIENT-PRINCIPAL-NAME"];
    return Results.Ok(new { userId, userName });
});
```

### Require Authentication
```csharp
// Minimal API
app.MapGet("/secure", () => "Secure data")
    .RequireAuthorization();

// Controller
[Authorize]
public class DataController : ControllerBase { }
```

### Require Role
```csharp
// Minimal API
app.MapPost("/admin", () => "Admin action")
    .RequireAuthorization("AdminPolicy");

// Controller
[Authorize(Roles = "Admin")]
public IActionResult AdminAction() { }
```

### Call Microsoft Graph
```csharp
var token = context.Request.Headers["X-MS-TOKEN-AAD-ACCESS-TOKEN"];
var httpClient = factory.CreateClient();
httpClient.DefaultRequestHeaders.Authorization = 
    new AuthenticationHeaderValue("Bearer", token);

var user = await httpClient.GetFromJsonAsync<UserProfile>(
    "https://graph.microsoft.com/v1.0/me");
```

## ?? Token Types

### ID Token
- **Purpose:** User identity
- **Format:** JWT
- **Contains:** Name, email, claims
- **Use:** Authentication

### Access Token
- **Purpose:** Call APIs
- **Lifetime:** ~1 hour
- **Scoped:** Specific resource
- **Use:** Authorization

### Refresh Token
- **Purpose:** Get new access tokens
- **Lifetime:** Days/months
- **Storage:** Token store
- **Use:** Token refresh

## ?? Configuration Options

### Account Types (Azure AD)
| Type | Description | Use Case |
|------|-------------|----------|
| **Single Tenant** | Current directory only | Internal apps |
| **Multi-Tenant** | Any Azure AD directory | B2B, SaaS |
| **Personal + Work** | Azure AD + Microsoft accounts | Hybrid |

### Authentication Mode
| Mode | Behavior |
|------|----------|
| **Require Authentication** | Block all unauthenticated |
| **Allow Unauthenticated** | App handles auth |

## ??? Troubleshooting

### Redirect URI Mismatch
```
Error: AADSTS50011
Fix: Verify exact URI in Azure AD
Format: https://<app>.azurewebsites.net/.auth/login/<provider>/callback
```

### Token Expired
```
Error: 401 Unauthorized
Fix: Enable token store
     Set refresh extension hours
     Check refresh token config
```

### Claims Missing
```
Error: Email/roles not available
Fix: Add optional claims in token config
     Request appropriate scopes
     Grant API permissions
```

## ?? Common Exam Scenarios

### Scenario 1: Secure Enterprise App
**Q:** "Enable Azure AD authentication for internal app"
**A:** 
1. Add Microsoft identity provider
2. Create new app registration
3. Select "Current tenant only" (single tenant)
4. Require authentication
5. Users sign in with work accounts

### Scenario 2: API Authentication
**Q:** "Frontend needs to call backend API with user context"
**A:**
1. Enable authentication on both apps
2. Enable token store
3. Frontend gets token from headers
4. Frontend passes token in Authorization header
5. Backend validates token

### Scenario 3: Role-Based Access
**Q:** "Only admins can access /admin endpoint"
**A:**
1. Define "Admin" app role in Azure AD
2. Assign users to Admin role
3. Use `[Authorize(Roles = "Admin")]`
4. Roles automatically in user claims

### Scenario 4: Social Login
**Q:** "Add Google authentication for consumer app"
**A:**
1. Create OAuth client in Google Console
2. Configure redirect URI
3. Add Google provider in Azure
4. Provide Client ID and Secret
5. Test login flow

## ?? Best Practices

### Security ?
- Always use HTTPS
- Enable token store
- Use Managed Identity
- Store secrets in Key Vault
- Enable MFA for admins
- Rotate secrets regularly
- Request minimum scopes

### Configuration ?
- Use "Require authentication" for private apps
- Allow unauthenticated for mixed content
- Configure redirect URLs correctly
- Use single tenant for internal apps
- Enable token refresh
- Monitor authentication failures

### Development ?
- Test in incognito mode
- Use `/.auth/me` for debugging
- Check browser network tab
- Validate tokens on backend
- Handle token expiration
- Log authorization failures
- Use IHttpClientFactory

## ?? Verification

### Test Authentication
```bash
# 1. Open in incognito browser
https://myapp.azurewebsites.net

# 2. Should redirect to login
https://login.microsoftonline.com/...

# 3. After login, check user info
https://myapp.azurewebsites.net/.auth/me
```

### Check Claims
```bash
curl https://myapp.azurewebsites.net/.auth/me \
  -H "Cookie: AppServiceAuthSession=..."
```

## ?? Decision Matrix

### Choose Authentication Method
| Scenario | Provider | Account Type |
|----------|----------|--------------|
| Internal company app | Microsoft | Single tenant |
| Multi-company B2B | Microsoft | Multi-tenant |
| Consumer web app | Google/Facebook | N/A |
| Developer tools | GitHub | N/A |
| Custom IdP | OpenID Connect | N/A |

### Choose Authorization Pattern
| Scenario | Pattern |
|----------|---------|
| Admin vs User | Role-based (RBAC) |
| Department access | Claims-based |
| Own resources only | Resource-based |
| Complex rules | Policy-based |

## ?? Remember for Exam

1. **Easy Auth** = Built-in, zero-code authentication
2. **Token Store** = Must enable for token refresh
3. **/.auth/*** endpoints = Built-in authentication endpoints
4. **X-MS-*** headers = User info automatically injected
5. **Single Tenant** = One Azure AD directory
6. **Multi-Tenant** = Multiple organizations
7. **Redirect URI** = Must match exactly in provider config
8. **Claims** = User attributes in token
9. **Roles** = Azure AD app roles for RBAC
10. **Managed Identity** = Best for Azure-to-Azure auth

---

**Perfect for:** Quick lookup during AZ-204 exam preparation! ??
