# Single Sign-On (SSO) Solution

This solution provides a complete Single Sign-On implementation with:
- **SSO Server**: A hosted identity provider using OpenIddict (OAuth 2.0 + OpenID Connect)
- **On-Prem Client**: A sample on-premises application that authenticates via SSO

## Architecture

```
???????????????????????         ????????????????????????
?   On-Prem Client    ?         ?    SSO Server        ?
?   (Port 5001)       ???????????   (Port 5000)        ?
?                     ?  OAuth2 ?                      ?
? - Login/Logout      ?  OIDC   ? - Authorization      ?
? - Protected Pages   ?         ? - Token Generation   ?
? - User Claims       ?         ? - User Info          ?
???????????????????????         ????????????????????????
```

## Features

### SSO Server (Identity Provider)
- ? OAuth 2.0 & OpenID Connect support
- ? Multiple grant types:
  - Authorization Code Flow (with PKCE)
  - Client Credentials Flow
  - Password Flow
  - Refresh Token Flow
- ? Token generation and validation
- ? User information endpoint
- ? Token introspection
- ? Pre-configured client applications

### On-Prem Client
- ? Automatic authentication via SSO
- ? Secure token storage
- ? User claims display
- ? Login/Logout functionality
- ? Protected routes

## Getting Started

### Prerequisites
- .NET 10.0 SDK

### Running the Solution

#### Option 1: Run Both Projects Simultaneously (Recommended)

1. **Start the SSO Server** (Terminal 1):
   ```bash
   cd Single-Sign-On
   dotnet restore
   dotnet run
   ```
   The SSO server will start at `http://localhost:5000`

2. **Start the On-Prem Client** (Terminal 2):
   ```bash
   cd OnPremClient
   dotnet restore
   dotnet run
   ```
   The client will start at `http://localhost:5001`

3. **Test the SSO Flow**:
   - Open browser to `http://localhost:5001`
   - Click "Login with SSO"
   - You'll be automatically authenticated (auto-approved for demo)
   - View your user claims on the dashboard
   - Click "Logout" to sign out

#### Option 2: Using Visual Studio
- Set both projects to start simultaneously (Solution Properties ? Multiple Startup Projects)

## Pre-Configured Clients

### 1. On-Prem Client (Authorization Code Flow)
- **Client ID**: `onprem-client`
- **Client Secret**: `onprem-secret-key-12345`
- **Redirect URIs**: `http://localhost:5001/signin-oidc`
- **Scopes**: `openid`, `profile`, `email`, `roles`, `api`

### 2. API Client (Client Credentials Flow)
- **Client ID**: `api-client`
- **Client Secret**: `api-secret-key-67890`
- **Use Case**: Service-to-service authentication

## Testing Different Flows

### 1. Authorization Code Flow (via Browser)
Already implemented in the On-Prem Client. Just visit `http://localhost:5001`

### 2. Password Flow (via API)
```bash
curl -X POST http://localhost:5000/connect/token \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "grant_type=password&username=demo&password=demo123&client_id=onprem-client&client_secret=onprem-secret-key-12345&scope=openid profile email"
```

### 3. Client Credentials Flow (Service-to-Service)
```bash
curl -X POST http://localhost:5000/connect/token \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "grant_type=client_credentials&client_id=api-client&client_secret=api-secret-key-67890&scope=api"
```

### 4. Get User Information
```bash
# First get an access token, then:
curl -X GET http://localhost:5000/connect/userinfo \
  -H "Authorization: Bearer YOUR_ACCESS_TOKEN"
```

## Demo Credentials

For testing the password flow:
- **Username**: `demo`
- **Password**: `demo123`

## API Endpoints

| Endpoint | Description |
|----------|-------------|
| `GET /` | Server status and endpoint list |
| `GET/POST /connect/authorize` | Authorization endpoint |
| `POST /connect/token` | Token generation endpoint |
| `GET/POST /connect/userinfo` | User information endpoint |
| `POST /connect/introspect` | Token introspection endpoint |

## Security Notes

?? **This is a development/demo configuration. For production:**

1. **Use HTTPS**: Remove `DisableTransportSecurityRequirement()` and `RequireHttpsMetadata = false`
2. **Use Real Certificates**: Replace development certificates with production ones
3. **Implement User Authentication**: Replace auto-approval with real login pages
4. **Use a Real Database**: Replace in-memory database with SQL Server, PostgreSQL, etc.
5. **Secure Secrets**: Use Azure Key Vault or environment variables for secrets
6. **Add Consent Screen**: Implement proper user consent flow
7. **Rate Limiting**: Add rate limiting to prevent abuse
8. **Logging & Monitoring**: Implement comprehensive logging

## Extending the Solution

### Add More Clients
Edit `SeedDataService.cs` to register additional client applications.

### Add User Management
Integrate with ASP.NET Core Identity for user registration, password management, etc.

### Add Custom Claims
Modify `AuthorizationController.cs` to add custom claims based on your requirements.

### Connect to Real Database
Replace `UseInMemoryDatabase` with `UseSqlServer`, `UseNpgsql`, etc. in `Program.cs`.

## Troubleshooting

### Port Already in Use
Change the ports in `appsettings.json` for both projects.

### CORS Errors
The SSO server has CORS enabled for all origins in development. Adjust the CORS policy in `Program.cs` for production.

### Token Issues
Check that both servers are running and that the SSO server URL in the client matches `http://localhost:5000`.

## Learn More

- [OpenIddict Documentation](https://documentation.openiddict.com/)
- [OAuth 2.0 Specification](https://oauth.net/2/)
- [OpenID Connect Specification](https://openid.net/connect/)

## License

MIT License - Use for educational and commercial purposes.
