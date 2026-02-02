using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

// Learninig Summary : This code demonstrates how to create and use Shared Access Signatures (SAS) in Azure Storage using C#.
// It covers Blob Service SAS, Account SAS, and Stored Access Policies, showcasing how to generate SAS tokens with specific permissions and test access to storage resources.
// The code includes error handling and logging for better traceability.
//  It requires Azure.Storage.Blobs NuGet package.
namespace AZ204.Security.Services;

public class SharedAccessSignatureService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<SharedAccessSignatureService> _logger;

    public SharedAccessSignatureService(IConfiguration configuration, ILogger<SharedAccessSignatureService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task RunSasDemoAsync()
    {
        try
        {
            _logger.LogInformation("=== Shared Access Signatures (SAS) Demo ===");
            Console.WriteLine("\n=== Shared Access Signatures (SAS) Demo ===\n");

            Console.WriteLine("?? SAS Token Types:\n");
            Console.WriteLine("1. Account SAS - Delegates access to resources in one or more storage services");
            Console.WriteLine("2. Service SAS - Delegates access to a resource in a single storage service");
            Console.WriteLine("3. User Delegation SAS - Secured with Azure AD credentials (most secure)\n");

            await BlobSasDemo();
            await AccountSasDemo();
            await StoredAccessPolicyDemo();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error running SAS demo");
            Console.WriteLine($"\n? Error: {ex.Message}");
            Console.WriteLine("\n?? Setup required:");
            Console.WriteLine("   1. Create a Storage Account in Azure");
            Console.WriteLine("   2. Get the account name and account key");
            Console.WriteLine("   3. Update appsettings.json or set environment variables:");
            Console.WriteLine("      AZURE_STORAGE_ACCOUNT_NAME=<your-account-name>");
            Console.WriteLine("      AZURE_STORAGE_ACCOUNT_KEY=<your-account-key>");
        }
    }

    private async Task BlobSasDemo()
    {
        Console.WriteLine("?? Blob Service SAS (Blob-level access)\n");

        try
        {
            var accountName = Environment.GetEnvironmentVariable("AZURE_STORAGE_ACCOUNT_NAME");
            var accountKey = Environment.GetEnvironmentVariable("AZURE_STORAGE_ACCOUNT_KEY");

            if (string.IsNullOrEmpty(accountName) || string.IsNullOrEmpty(accountKey))
            {
                Console.WriteLine("??  Storage account credentials not configured.");
                Console.WriteLine("   Set AZURE_STORAGE_ACCOUNT_NAME and AZURE_STORAGE_ACCOUNT_KEY environment variables.\n");
                return;
            }

            // Create a test container and blob
            string containerName = $"sas-demo-{Guid.NewGuid():N}";
            string blobName = "test-document.txt";
            string blobContent = $"This is a test document created at {DateTime.UtcNow}";

            var credential = new StorageSharedKeyCredential(accountName, accountKey);
            var blobServiceClient = new BlobServiceClient(
                new Uri($"https://{accountName}.blob.core.windows.net"),
                credential);

            Console.WriteLine($"Creating container: {containerName}");
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            await containerClient.CreateIfNotExistsAsync(PublicAccessType.None);

            Console.WriteLine($"Uploading blob: {blobName}");
            var blobClient = containerClient.GetBlobClient(blobName);
            await blobClient.UploadAsync(BinaryData.FromString(blobContent), overwrite: true);

            Console.WriteLine("? Test blob created\n");

            // Generate SAS token for the blob
            Console.WriteLine("Generating SAS token with READ permissions...");

            if (!blobClient.CanGenerateSasUri)
            {
                Console.WriteLine("? BlobClient must be authorized with Shared Key credentials to create a SAS.");
                return;
            }

            var sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = containerName,
                BlobName = blobName,
                Resource = "b", // "b" for blob
                StartsOn = DateTimeOffset.UtcNow.AddMinutes(-5), // Start 5 minutes ago to account for clock skew
                ExpiresOn = DateTimeOffset.UtcNow.AddHours(1) // Valid for 1 hour
            };

            // Set permissions
            sasBuilder.SetPermissions(BlobSasPermissions.Read);

            Uri sasUri = blobClient.GenerateSasUri(sasBuilder);

            Console.WriteLine("? SAS token generated!");
            Console.WriteLine($"\n?? SAS Details:");
            Console.WriteLine($"   Blob URL: {blobClient.Uri}");
            Console.WriteLine($"   SAS Token: {sasUri.Query}");
            Console.WriteLine($"   Full URL with SAS: {sasUri}");
            Console.WriteLine($"   Permissions: Read");
            Console.WriteLine($"   Starts: {sasBuilder.StartsOn}");
            Console.WriteLine($"   Expires: {sasBuilder.ExpiresOn}");

            // Test accessing the blob with SAS
            Console.WriteLine($"\nTesting access using SAS token...");
            var sasBlob = new BlobClient(sasUri);
            var downloadResult = await sasBlob.DownloadContentAsync();
            string downloadedContent = downloadedContent = downloadResult.Value.Content.ToString();

            Console.WriteLine($"? Successfully accessed blob using SAS!");
            Console.WriteLine($"   Content: {downloadedContent}");

            // Try to delete (should fail - no delete permission)
            Console.WriteLine($"\nTrying to delete blob with READ-only SAS (should fail)...");
            try
            {
                await sasBlob.DeleteAsync();
                Console.WriteLine("? Unexpected: Delete succeeded (it shouldn't have)");
            }
            catch (Azure.RequestFailedException ex) when (ex.Status == 403)
            {
                Console.WriteLine("? Delete correctly denied (403 Forbidden) - READ-only SAS working as expected");
            }

            // Generate SAS with write permissions
            Console.WriteLine($"\nGenerating new SAS token with WRITE permissions...");
            sasBuilder.SetPermissions(BlobSasPermissions.Read | BlobSasPermissions.Write);
            Uri writeSasUri = blobClient.GenerateSasUri(sasBuilder);

            var writeSasBlob = new BlobClient(writeSasUri);
            string newContent = $"Updated content at {DateTime.UtcNow}";
            await writeSasBlob.UploadAsync(BinaryData.FromString(newContent), overwrite: true);
            Console.WriteLine($"? Successfully wrote to blob using WRITE SAS!");

            // Cleanup
            Console.WriteLine($"\nCleaning up - deleting container: {containerName}");
            await containerClient.DeleteIfExistsAsync();
            Console.WriteLine("? Cleanup complete\n");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in blob SAS demo");
            Console.WriteLine($"? Error: {ex.Message}\n");
        }
    }

    private async Task AccountSasDemo()
    {
        Console.WriteLine("?? Account SAS (Account-level access)\n");

        try
        {
            var accountName = Environment.GetEnvironmentVariable("AZURE_STORAGE_ACCOUNT_NAME");
            var accountKey = Environment.GetEnvironmentVariable("AZURE_STORAGE_ACCOUNT_KEY");

            if (string.IsNullOrEmpty(accountName) || string.IsNullOrEmpty(accountKey))
            {
                Console.WriteLine("??  Storage account credentials not configured.\n");
                return;
            }

            Console.WriteLine("Account SAS characteristics:");
            Console.WriteLine("  ? Grants access to multiple storage services (Blob, Queue, Table, File)");
            Console.WriteLine("  ? Can access multiple resources");
            Console.WriteLine("  ? Supports service-level operations\n");

            var credential = new StorageSharedKeyCredential(accountName, accountKey);

            // Create Account SAS
            var sasBuilder = new AccountSasBuilder
            {
                Services = AccountSasServices.Blobs | AccountSasServices.Queues,
                ResourceTypes = AccountSasResourceTypes.Service | AccountSasResourceTypes.Container | AccountSasResourceTypes.Object,
                StartsOn = DateTimeOffset.UtcNow.AddMinutes(-5),
                ExpiresOn = DateTimeOffset.UtcNow.AddHours(2),
                Protocol = SasProtocol.Https
            };

            // Set permissions for multiple services
            sasBuilder.SetPermissions(
                AccountSasPermissions.Read |
                AccountSasPermissions.Write |
                AccountSasPermissions.List);

            string sasToken = sasBuilder.ToSasQueryParameters(credential).ToString();

            Console.WriteLine("? Account SAS token generated!");
            Console.WriteLine($"\n?? Account SAS Details:");
            Console.WriteLine($"   Account: {accountName}");
            Console.WriteLine($"   SAS Token: {sasToken}");
            Console.WriteLine($"   Services: Blobs, Queues");
            Console.WriteLine($"   Resource Types: Service, Container, Object");
            Console.WriteLine($"   Permissions: Read, Write, List");
            Console.WriteLine($"   Protocol: HTTPS only");
            Console.WriteLine($"   Starts: {sasBuilder.StartsOn}");
            Console.WriteLine($"   Expires: {sasBuilder.ExpiresOn}");

            Console.WriteLine($"\n?? Example usage:");
            Console.WriteLine($"   Blob: https://{accountName}.blob.core.windows.net/?{sasToken}");
            Console.WriteLine($"   Queue: https://{accountName}.queue.core.windows.net/?{sasToken}");

            // Test the Account SAS
            Console.WriteLine($"\nTesting Account SAS by listing containers...");
            var blobServiceUri = new Uri($"https://{accountName}.blob.core.windows.net?{sasToken}");
            var blobServiceClient = new BlobServiceClient(blobServiceUri);

            var containers = blobServiceClient.GetBlobContainersAsync();
            int containerCount = 0;
            await foreach (var container in containers)
            {
                containerCount++;
                Console.WriteLine($"   - {container.Name}");
                if (containerCount >= 5) break; // Limit output
            }

            if (containerCount == 0)
            {
                Console.WriteLine("   (No containers found)");
            }

            Console.WriteLine($"? Account SAS working - listed {containerCount} container(s)");

            Console.WriteLine("\n");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in account SAS demo");
            Console.WriteLine($"? Error: {ex.Message}\n");
        }
    }

    private async Task StoredAccessPolicyDemo()
    {
        Console.WriteLine("?? Stored Access Policy (Revocable SAS)\n");

        try
        {
            var accountName = Environment.GetEnvironmentVariable("AZURE_STORAGE_ACCOUNT_NAME");
            var accountKey = Environment.GetEnvironmentVariable("AZURE_STORAGE_ACCOUNT_KEY");

            if (string.IsNullOrEmpty(accountName) || string.IsNullOrEmpty(accountKey))
            {
                Console.WriteLine("??  Storage account credentials not configured.\n");
                return;
            }

            Console.WriteLine("Stored Access Policy benefits:");
            Console.WriteLine("  ? Allows you to revoke SAS tokens without regenerating account keys");
            Console.WriteLine("  ? Change permissions after SAS is issued");
            Console.WriteLine("  ? Better control over distributed SAS tokens\n");

            var credential = new StorageSharedKeyCredential(accountName, accountKey);
            var blobServiceClient = new BlobServiceClient(
                new Uri($"https://{accountName}.blob.core.windows.net"),
                credential);

            string containerName = $"policy-demo-{Guid.NewGuid():N}";
            Console.WriteLine($"Creating container: {containerName}");
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            await containerClient.CreateIfNotExistsAsync(PublicAccessType.None);

            // Create a stored access policy
            string policyName = "read-policy";
            Console.WriteLine($"\nCreating stored access policy: {policyName}");

            var accessPolicy = new BlobSignedIdentifier
            {
                Id = policyName,
                AccessPolicy = new BlobAccessPolicy
                {
                    PolicyStartsOn = DateTimeOffset.UtcNow.AddMinutes(-5),
                    PolicyExpiresOn = DateTimeOffset.UtcNow.AddHours(2),
                    Permissions = "r" // Read permission
                }
            };

            await containerClient.SetAccessPolicyAsync(permissions: new[] { accessPolicy });
            Console.WriteLine("? Stored access policy created");

            // Generate SAS using the stored policy
            Console.WriteLine($"\nGenerating SAS token using stored access policy...");

            var sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = containerName,
                Identifier = policyName // Reference to stored policy
            };

            // No need to specify permissions, start time, or expiry - they come from the policy
            if (containerClient.CanGenerateSasUri)
            {
                Uri sasUri = containerClient.GenerateSasUri(sasBuilder);

                Console.WriteLine("? SAS token generated using stored policy!");
                Console.WriteLine($"\n?? Details:");
                Console.WriteLine($"   Container URL: {containerClient.Uri}");
                Console.WriteLine($"   SAS with Policy: {sasUri}");
                Console.WriteLine($"   Policy ID: {policyName}");
                Console.WriteLine($"   Permissions: Defined in policy (Read)");

                // Upload a test blob
                string blobName = "policy-test.txt";
                var blobClient = containerClient.GetBlobClient(blobName);
                await blobClient.UploadAsync(BinaryData.FromString("Test content for policy demo"), overwrite: true);

                // Test access with SAS
                Console.WriteLine($"\nTesting access with policy-based SAS...");
                var sasContainerClient = new BlobContainerClient(sasUri);
                var sasBlob = sasContainerClient.GetBlobClient(blobName);
                
                var downloadResult = await sasBlob.DownloadContentAsync();
                Console.WriteLine($"? Successfully read blob using policy-based SAS!");
                Console.WriteLine($"   Content: {downloadResult.Value.Content}");

                Console.WriteLine($"\n?? To revoke access:");
                Console.WriteLine($"   1. Delete or modify the stored access policy");
                Console.WriteLine($"   2. All SAS tokens referencing that policy become invalid");
                Console.WriteLine($"   3. No need to regenerate account keys!");
            }

            // Cleanup
            Console.WriteLine($"\nCleaning up - deleting container: {containerName}");
            await containerClient.DeleteIfExistsAsync();
            Console.WriteLine("? Cleanup complete\n");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in stored access policy demo");
            Console.WriteLine($"? Error: {ex.Message}\n");
        }
    }
}
