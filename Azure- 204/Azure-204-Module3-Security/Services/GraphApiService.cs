using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Identity.Client;
using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AZ204.Security.Services;

public class GraphApiService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<GraphApiService> _logger;
    private GraphServiceClient? _graphClient;

    public GraphApiService(IConfiguration configuration, ILogger<GraphApiService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    private async Task<GraphServiceClient?> InitializeGraphClientAsync()
    {
        var tenantId = _configuration["Azure:TenantId"];
        var clientId = _configuration["Azure:ClientId"];

        if (string.IsNullOrEmpty(tenantId) || string.IsNullOrEmpty(clientId))
        {
            Console.WriteLine("??  TenantId or ClientId not configured.");
            return null;
        }

        try
        {
            // Using Interactive Browser Credential for user context
            var credential = new InteractiveBrowserCredential(new InteractiveBrowserCredentialOptions
            {
                TenantId = tenantId,
                ClientId = clientId,
                RedirectUri = new Uri("http://localhost")
            });

            var scopes = new[] { "User.Read", "Mail.Read", "Calendars.Read" };

            _graphClient = new GraphServiceClient(credential, scopes);
            return _graphClient;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize Graph client");
            Console.WriteLine($"? Failed to initialize Graph client: {ex.Message}");
            return null;
        }
    }

    public async Task RunGraphApiDemoAsync()
    {
        try
        {
            _logger.LogInformation("=== Microsoft Graph API Demo ===");
            Console.WriteLine("\n=== Microsoft Graph API Demo ===\n");

            var graphClient = await InitializeGraphClientAsync();
            if (graphClient == null)
            {
                Console.WriteLine("?? Make sure you have:");
                Console.WriteLine("   1. Created App Registration in Azure Portal");
                Console.WriteLine("   2. Updated appsettings.json with TenantId and ClientId");
                Console.WriteLine("   3. Configured API permissions: User.Read, Mail.Read, Calendars.Read");
                Console.WriteLine("   4. Added redirect URI: http://localhost\n");
                return;
            }

            await GetUserProfileAsync(graphClient);
            await GetUserPhotoAsync(graphClient);
            await GetUserMailsAsync(graphClient);
            await GetUserCalendarEventsAsync(graphClient);
            await GetOrganizationDetailsAsync(graphClient);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error running Graph API demo");
            Console.WriteLine($"\n? Error: {ex.Message}");
        }
    }

    private async Task GetUserProfileAsync(GraphServiceClient graphClient)
    {
        Console.WriteLine("?? Getting User Profile\n");

        try
        {
            var user = await graphClient.Me.GetAsync();

            if (user != null)
            {
                Console.WriteLine("? User Profile Retrieved:");
                Console.WriteLine($"   Display Name: {user.DisplayName}");
                Console.WriteLine($"   Email: {user.Mail ?? user.UserPrincipalName}");
                Console.WriteLine($"   Job Title: {user.JobTitle ?? "N/A"}");
                Console.WriteLine($"   Office Location: {user.OfficeLocation ?? "N/A"}");
                Console.WriteLine($"   Mobile Phone: {user.MobilePhone ?? "N/A"}");
                Console.WriteLine($"   Business Phone: {(user.BusinessPhones?.Any() == true ? string.Join(", ", user.BusinessPhones) : "N/A")}");
                Console.WriteLine($"   User ID: {user.Id}");
                Console.WriteLine($"   User Principal Name: {user.UserPrincipalName}");
            }

            Console.WriteLine("\n");
        }
        catch (ServiceException ex)
        {
            _logger.LogError(ex, "Error getting user profile");
            Console.WriteLine($"? Error: {ex.Message}\n");
        }
    }

    private async Task GetUserPhotoAsync(GraphServiceClient graphClient)
    {
        Console.WriteLine("?? Getting User Photo\n");

        try
        {
            // Get photo metadata
            var photo = await graphClient.Me.Photo.GetAsync();
            
            if (photo != null)
            {
                Console.WriteLine("? Photo metadata retrieved:");
                Console.WriteLine($"   Height: {photo.Height}px");
                Console.WriteLine($"   Width: {photo.Width}px");

                // Get actual photo content
                var photoStream = await graphClient.Me.Photo.Content.GetAsync();
                if (photoStream != null)
                {
                    Console.WriteLine($"   Photo size: {photoStream.Length} bytes");
                    Console.WriteLine("   Photo content retrieved successfully");
                }
            }

            Console.WriteLine("\n");
        }
        catch (ServiceException ex) when (ex.ResponseStatusCode == 404)
        {
            Console.WriteLine("??  No profile photo found\n");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user photo");
            Console.WriteLine($"? Error: {ex.Message}\n");
        }
    }

    private async Task GetUserMailsAsync(GraphServiceClient graphClient)
    {
        Console.WriteLine("?? Getting User Emails (Top 5)\n");

        try
        {
            var messages = await graphClient.Me.Messages
                .GetAsync(requestConfiguration =>
                {
                    requestConfiguration.QueryParameters.Top = 5;
                    requestConfiguration.QueryParameters.Select = new[] { "subject", "from", "receivedDateTime", "isRead" };
                    requestConfiguration.QueryParameters.Orderby = new[] { "receivedDateTime DESC" };
                });

            if (messages?.Value != null && messages.Value.Any())
            {
                Console.WriteLine($"? Found {messages.Value.Count} recent emails:\n");

                int count = 1;
                foreach (var message in messages.Value)
                {
                    Console.WriteLine($"   {count}. {message.Subject}");
                    Console.WriteLine($"      From: {message.From?.EmailAddress?.Name ?? "Unknown"} <{message.From?.EmailAddress?.Address ?? "Unknown"}>");
                    Console.WriteLine($"      Received: {message.ReceivedDateTime?.LocalDateTime}");
                    Console.WriteLine($"      Status: {(message.IsRead == true ? "Read" : "Unread")}");
                    Console.WriteLine();
                    count++;
                }
            }
            else
            {
                Console.WriteLine("??  No emails found\n");
            }
        }
        catch (ServiceException ex) when (ex.ResponseStatusCode == 403)
        {
            Console.WriteLine("? Insufficient permissions to read mail. Grant 'Mail.Read' permission.\n");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user mails");
            Console.WriteLine($"? Error: {ex.Message}\n");
        }
    }

    private async Task GetUserCalendarEventsAsync(GraphServiceClient graphClient)
    {
        Console.WriteLine("?? Getting Calendar Events (Next 5)\n");

        try
        {
            var events = await graphClient.Me.Calendar.Events
                .GetAsync(requestConfiguration =>
                {
                    requestConfiguration.QueryParameters.Top = 5;
                    requestConfiguration.QueryParameters.Select = new[] { "subject", "start", "end", "location", "organizer" };
                    requestConfiguration.QueryParameters.Orderby = new[] { "start/dateTime ASC" };
                });

            if (events?.Value != null && events.Value.Any())
            {
                Console.WriteLine($"? Found {events.Value.Count} upcoming events:\n");

                int count = 1;
                foreach (var calendarEvent in events.Value)
                {
                    Console.WriteLine($"   {count}. {calendarEvent.Subject}");
                    Console.WriteLine($"      Start: {calendarEvent.Start?.DateTime} ({calendarEvent.Start?.TimeZone})");
                    Console.WriteLine($"      End: {calendarEvent.End?.DateTime} ({calendarEvent.End?.TimeZone})");
                    Console.WriteLine($"      Location: {calendarEvent.Location?.DisplayName ?? "No location"}");
                    Console.WriteLine($"      Organizer: {calendarEvent.Organizer?.EmailAddress?.Name ?? "Unknown"}");
                    Console.WriteLine();
                    count++;
                }
            }
            else
            {
                Console.WriteLine("??  No calendar events found\n");
            }
        }
        catch (ServiceException ex) when (ex.ResponseStatusCode == 403)
        {
            Console.WriteLine("? Insufficient permissions to read calendar. Grant 'Calendars.Read' permission.\n");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting calendar events");
            Console.WriteLine($"? Error: {ex.Message}\n");
        }
    }

    private async Task GetOrganizationDetailsAsync(GraphServiceClient graphClient)
    {
        Console.WriteLine("?? Getting Organization Details\n");

        try
        {
            var organization = await graphClient.Organization.GetAsync();

            if (organization?.Value != null && organization.Value.Any())
            {
                var org = organization.Value.First();
                Console.WriteLine("? Organization Information:");
                Console.WriteLine($"   Name: {org.DisplayName}");
                Console.WriteLine($"   ID: {org.Id}");
                Console.WriteLine($"   Verified Domains: {(org.VerifiedDomains?.Any() == true ? string.Join(", ", org.VerifiedDomains.Select(d => d.Name)) : "N/A")}");
                Console.WriteLine($"   Street: {org.Street ?? "N/A"}");
                Console.WriteLine($"   City: {org.City ?? "N/A"}");
                Console.WriteLine($"   State: {org.State ?? "N/A"}");
                Console.WriteLine($"   Country: {org.CountryLetterCode ?? "N/A"}");
            }

            Console.WriteLine("\n");
        }
        catch (ServiceException ex) when (ex.ResponseStatusCode == 403)
        {
            Console.WriteLine("? Insufficient permissions to read organization details.\n");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting organization details");
            Console.WriteLine($"? Error: {ex.Message}\n");
        }
    }
}
