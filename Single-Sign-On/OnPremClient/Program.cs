using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// Configure authentication with SSO Server
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
.AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
{
    options.Authority = "http://localhost:5000";
    options.ClientId = "onprem-client";
    options.ClientSecret = "onprem-secret-key-12345";
    options.ResponseType = "code";
    options.RequireHttpsMetadata = false; // Only for development!
    
    options.SaveTokens = true;
    options.GetClaimsFromUserInfoEndpoint = true;
    
    options.Scope.Clear();
    options.Scope.Add("openid");
    options.Scope.Add("profile");
    options.Scope.Add("email");
    options.Scope.Add("roles");
    options.Scope.Add("api");
    
    options.UsePkce = true;
});

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", (HttpContext context) =>
{
    if (!context.User.Identity?.IsAuthenticated ?? true)
    {
        return Results.Content(@"
<!DOCTYPE html>
<html>
<head>
    <title>On-Prem Client - Login</title>
    <style>
        body { font-family: Arial, sans-serif; max-width: 800px; margin: 50px auto; padding: 20px; }
        .btn { background-color: #0066cc; color: white; padding: 10px 20px; 
               text-decoration: none; border-radius: 5px; display: inline-block; margin: 10px 0; }
        .btn:hover { background-color: #0052a3; }
        h1 { color: #333; }
    </style>
</head>
<body>
    <h1>?? On-Premises Client Application</h1>
    <p>This is a demonstration of an on-premises client application using Single Sign-On (SSO).</p>
    <p>Click the button below to authenticate via the SSO Server.</p>
    <a href='/login' class='btn'>Login with SSO</a>
</body>
</html>", "text/html");
    }

    var claims = context.User.Claims.Select(c => $"<li><strong>{c.Type}:</strong> {c.Value}</li>");
    return Results.Content($@"
<!DOCTYPE html>
<html>
<head>
    <title>On-Prem Client - Dashboard</title>
    <style>
        body {{ font-family: Arial, sans-serif; max-width: 800px; margin: 50px auto; padding: 20px; }}
        .btn {{ background-color: #dc3545; color: white; padding: 10px 20px; 
               text-decoration: none; border-radius: 5px; display: inline-block; margin: 10px 0; }}
        .btn:hover {{ background-color: #c82333; }}
        .success {{ background-color: #d4edda; border: 1px solid #c3e6cb; 
                   padding: 15px; border-radius: 5px; margin: 20px 0; }}
        h1 {{ color: #333; }}
        ul {{ background-color: #f8f9fa; padding: 20px; border-radius: 5px; }}
    </style>
</head>
<body>
    <h1>? Authenticated Successfully!</h1>
    <div class='success'>
        <strong>Welcome, {context.User.Identity.Name}!</strong><br>
        You are now authenticated via the SSO Server.
    </div>
    <h2>Your Claims:</h2>
    <ul>
        {string.Join("", claims)}
    </ul>
    <a href='/logout' class='btn'>Logout</a>
</body>
</html>", "text/html");
});

app.MapGet("/login", () => Results.Challenge(
    new Microsoft.AspNetCore.Authentication.AuthenticationProperties 
    { 
        RedirectUri = "/" 
    },
    new[] { OpenIdConnectDefaults.AuthenticationScheme }
));

app.MapGet("/logout", () => Results.SignOut(
    new Microsoft.AspNetCore.Authentication.AuthenticationProperties 
    { 
        RedirectUri = "/" 
    },
    new[] { CookieAuthenticationDefaults.AuthenticationScheme, OpenIdConnectDefaults.AuthenticationScheme }
));

app.Run();
