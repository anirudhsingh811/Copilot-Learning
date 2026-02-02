using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Azure.Storage.Sas;
using Microsoft.Extensions.Logging;

// Learning Summary of this class:
// This BlobStorageManager class provides a comprehensive set of operations for managing Azure Blob Storage.
//  It includes methods for uploading and downloading blobs, generating SAS tokens, setting access tiers,
//  enabling versioning, soft deleting and restoring blobs, creating snapshots, copying blobs between containers,
namespace AZ204.Storage.BlobStorage;

/// <summary>
/// Comprehensive Blob Storage operations
/// Covers: Upload, download, SAS, lifecycle, tiers, static websites
/// </summary>
public class BlobStorageManager // Comprehensive Blob Storage operations
{
    private readonly ILogger<BlobStorageManager> _logger;
    private readonly BlobServiceClient _blobServiceClient;

    public BlobStorageManager(ILogger<BlobStorageManager> logger, BlobServiceClient blobServiceClient)
    {
        _logger = logger;
        _blobServiceClient = blobServiceClient; // Blob service client from dependency injection, who is injecting? // Startup.cs or Program.cs
    }

    /// <summary>
    /// Upload blob (simple)
    /// </summary>
    public async Task<string> UploadBlobAsync( // simple upload and     return URI
        string containerName,
        string blobName,
        Stream content,
        string contentType = "application/octet-stream")// default content type , means binary data
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName); // get container client
        await containerClient.CreateIfNotExistsAsync();// create container if not exists

        var blobClient = containerClient.GetBlobClient(blobName); // get blob client in the container

        var uploadOptions = new BlobUploadOptions
        {
            HttpHeaders = new BlobHttpHeaders { ContentType = contentType }
        };

        await blobClient.UploadAsync(content, uploadOptions);// upload the blob from stream

        _logger.LogInformation("Blob uploaded: {BlobName} to {Container}", blobName, containerName);
        return blobClient.Uri.ToString(); // return the URI of the uploaded blob, How to get SAS token for this blob?// see GenerateBlobSasToken method
    }

    /// <summary>
    /// Upload large blob with progress tracking
    /// </summary>
    public async Task<string> UploadLargeBlobAsync(
        string containerName,
        string blobName,
        string filePath,
        IProgress<long>? progress = null)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        var blobClient = containerClient.GetBlobClient(blobName);

        var uploadOptions = new BlobUploadOptions // upload options for large blob and how is different then normal upload build option ? 
        // The main difference is the TransferOptions, which allows for more control over the upload process, such as parallelism and chunk size
        {
            TransferOptions = new Azure.Storage.StorageTransferOptions
            {
                MaximumConcurrency = 4,
                MaximumTransferSize = 4 * 1024 * 1024 // 4 MB chunks
            },
            ProgressHandler = progress
        };

        using var fileStream = File.OpenRead(filePath);
        await blobClient.UploadAsync(fileStream, uploadOptions);

        _logger.LogInformation("Large blob uploaded: {BlobName}", blobName);
        return blobClient.Uri.ToString();
    }

    /// <summary>
    /// Download blob
    /// </summary>
    public async Task<Stream> DownloadBlobAsync(string containerName, string blobName)
    {
        var blobClient = _blobServiceClient
            .GetBlobContainerClient(containerName)
            .GetBlobClient(blobName);

        var response = await blobClient.DownloadStreamingAsync();// download streaming async method
        _logger.LogInformation("Blob downloaded: {BlobName}", blobName);
        
        return response.Value.Content;
    }

    /// <summary>
    /// Generate SAS token for blob
    /// </summary>
    public string GenerateBlobSasToken(
        string containerName,
        string blobName,
        TimeSpan expiresIn,
        BlobSasPermissions permissions = BlobSasPermissions.Read) 
    {
        // Get the blob client by first obtaining the container client from the service client
        // then getting the specific blob client within that container
        var blobClient = _blobServiceClient
            .GetBlobContainerClient(containerName) // Retrieve the container client for the specified container
            .GetBlobClient(blobName); // Retrieve the blob client for the specific blob within the container

        // Create a SAS builder object to configure the Shared Access Signature parameters
        var sasBuilder = new BlobSasBuilder
        {
            BlobContainerName = containerName, // Set the container name for the SAS token
            BlobName = blobName, // Set the blob name for the SAS token
            Resource = "b", // Specify resource type as "b" for blob (not container)
            StartsOn = DateTimeOffset.UtcNow.AddMinutes(-5), // Set start time 5 minutes in the past to account for clock skew between client and server
            ExpiresOn = DateTimeOffset.UtcNow.Add(expiresIn) // Set expiration time based on the provided TimeSpan duration
        };

        // Apply the specified permissions (Read, Write, Delete, etc.) to the SAS builder
        sasBuilder.SetPermissions(permissions);

        // Generate the SAS URI using the blob client and extract only the query string portion (the actual SAS token)
        var sasToken = blobClient.GenerateSasUri(sasBuilder).Query;
        
        // Log the successful generation of the SAS token with blob name and expiration duration
        _logger.LogInformation("SAS token generated for: {BlobName}, expires in {Minutes} minutes",
            blobName, expiresIn.TotalMinutes);

        // Return the generated SAS token query string
        return sasToken;
    }

    /// <summary>
    /// Set blob access tier (Hot, Cool, Archive)
    /// // How to decide which tier to use?
    /// - Hot: Frequently accessed data
    /// - Cool: Infrequently accessed data
    /// - Archive: Rarely accessed data
    /// </summary>
    public async Task SetBlobAccessTierAsync(
        string containerName,
        string blobName,
        AccessTier accessTier) // Hot, Cool, Archive
    {
        var blobClient = _blobServiceClient // what is blob service client here? // injected via constructor
            .GetBlobContainerClient(containerName)// get container client
            .GetBlobClient(blobName);// get blob client

        await blobClient.SetAccessTierAsync(accessTier);// set access tier async

        _logger.LogInformation("Blob {BlobName} moved to {Tier} tier", blobName, accessTier);
    }

    /// <summary>
    /// Enable blob versioning
    /// </summary>
    public async Task EnableBlobVersioningAsync()
    {
        var properties = await _blobServiceClient.GetPropertiesAsync();// get properties of blob service client

        // Note: Versioning must be enabled at the storage account level via Azure Portal or ARM template
        // How to do in azure portal? // Go to Storage Account > Data management > Blob versioning > Enable
        // This is a read-only property in the SDK

        _logger.LogInformation("Blob versioning status checked. Enable via Azure Portal if needed.");
        await Task.CompletedTask;
    }

    /// <summary>
    /// Soft delete blob
    /// </summary>
    public async Task DeleteBlobAsync(string containerName, string blobName)
    {
        var blobClient = _blobServiceClient
            .GetBlobContainerClient(containerName)
            .GetBlobClient(blobName);

        await blobClient.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots);
        _logger.LogInformation("Blob soft deleted: {BlobName}", blobName);
    }

    /// <summary>
    /// Restore soft-deleted blob
    /// </summary>
    public async Task RestoreBlobAsync(string containerName, string blobName) // Under what time can we restore it ?    // Within retention period set for soft delete in storage account
                                                                              //HOw to set retention period? // Azure Portal > Storage Account > Data management > Soft delete for blobs > Set retention period
                                                                              // And through SDK? // Not directly via SDK, must be set in Azure Portal or ARM template
    {
        var blobClient = _blobServiceClient
            .GetBlobContainerClient(containerName)
            .GetBlobClient(blobName);

        await blobClient.UndeleteAsync();
        _logger.LogInformation("Blob restored: {BlobName}", blobName);
    }

    /// <summary>
    /// Create blob snapshot
    /// </summary>
    public async Task<string> CreateBlobSnapshotAsync(string containerName, string blobName) // What is actually a snapshot ? // A read-only version of a blob at a specific point in time
    {
        var blobClient = _blobServiceClient
            .GetBlobContainerClient(containerName)
            .GetBlobClient(blobName);

        var snapshot = await blobClient.CreateSnapshotAsync();
        
        _logger.LogInformation("Snapshot created for: {BlobName}", blobName);
        return snapshot.Value.Snapshot;
    }

    /// <summary>
    /// Copy blob between containers
    /// </summary>
    public async Task<string> CopyBlobAsync(
        string sourceContainer,
        string sourceBlobName,
        string destContainer,
        string destBlobName)
    {
        var sourceBlob = _blobServiceClient
            .GetBlobContainerClient(sourceContainer)
            .GetBlobClient(sourceBlobName);

        var destBlob = _blobServiceClient
            .GetBlobContainerClient(destContainer)
            .GetBlobClient(destBlobName);

        var copyOperation = await destBlob.StartCopyFromUriAsync(sourceBlob.Uri);// start copy from source blob URI to destination blob
        await copyOperation.WaitForCompletionAsync();// Is copy operation synchronous? // No, it's asynchronous, we wait for completion here

        _logger.LogInformation("Blob copied: {Source} ? {Dest}", sourceBlobName, destBlobName);
        return destBlob.Uri.ToString();
    }

    /// <summary>
    /// List blobs with prefix and pagination
    /// </summary>
    public async Task<List<BlobItem>> ListBlobsAsync(
        string containerName,
        string? prefix = null,
        int? maxResults = null)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        
        var blobs = new List<BlobItem>();

        await foreach (var blob in containerClient.GetBlobsAsync(prefix: prefix))// How does getblobclient differ than getblobsasync? // GetBlobClient gets a single blob client, GetBlobsAsync lists blobs in the container
        {
            blobs.Add(blob);
            
            if (maxResults.HasValue && blobs.Count >= maxResults.Value)
                break;
        }

        _logger.LogInformation("Listed {Count} blobs from container: {Container}",
            blobs.Count, containerName);

        return blobs;
    }

    /// <summary>
    /// Set blob metadata
    /// </summary>
    public async Task SetBlobMetadataAsync(
        string containerName,
        string blobName,
        Dictionary<string, string> metadata)
    {
        var blobClient = _blobServiceClient
            .GetBlobContainerClient(containerName)
            .GetBlobClient(blobName);

        await blobClient.SetMetadataAsync(metadata);
        
        _logger.LogInformation("Metadata set for blob: {BlobName}", blobName);
    }

    /// <summary>
    /// Get blob metadata
    /// </summary>
    public async Task<IDictionary<string, string>> GetBlobMetadataAsync(
        string containerName,
        string blobName)
    {
        var blobClient = _blobServiceClient
            .GetBlobContainerClient(containerName)
            .GetBlobClient(blobName);

        var properties = await blobClient.GetPropertiesAsync();
        return properties.Value.Metadata;
    }

    /// <summary>
    /// Configure static website // enable static website hosting
    /// How does Static website differ from dynamic website? // Static websites serve fixed content, while dynamic websites generate content on-the-fly based on user interactions or server-side logic.
    /// What do we mean by content ? // HTML, CSS, JavaScript files that make up the website.
    /// When does Blob hosting is prefered over traditional web hosting? // For simple sites with low interactivity, cost-effectiveness, and scalability.
    /// How does scalability works here ? // Azure Blob Storage automatically scales to handle traffic without manual intervention.
    /// Does it mean it create copy of Website or increase instaces ? // It increases instances and manages load balancing automatically.// What actually are instance here as it's not node or VM ? // Instances refer to the underlying infrastructure that serves the static content.
    /// </summary>
    public async Task ConfigureStaticWebsiteAsync(
        string indexDocument = "index.html",
        string errorDocument = "404.html")// But Where will be website file stores and how do you provide path of file apart from index and error document ? 
                                          // Files are stored in the $web container, and paths are relative to that container.
                                          //If I have multiple wesites to host then I should have different container or can it be achieved in single container ?
                                          // Each static website should have its own storage account with a $web container.So is it possible to have multiple $web container in single storage account ?
                                          // No, each storage account can have only one $web container for static website hosting.
    {
        var properties = await _blobServiceClient.GetPropertiesAsync();// How to add content in web container ? // Use BlobContainerClient to upload files to the $web container. Example: var webContainer = _blobServiceClient.GetBlobContainerClient("$web");

        properties.Value.StaticWebsite = new BlobStaticWebsite // Can mutiple websites hosted in one web container ? // No, a single $web container can host only one static website.
        {
            Enabled = true,
            IndexDocument = indexDocument,
            DefaultIndexDocumentPath = errorDocument
        };

        await _blobServiceClient.SetPropertiesAsync(properties.Value);
        
        _logger.LogInformation("Static website configured");
    }

    /// <summary>
    /// Lease blob (distributed locking)
    /// What does lease mean here ? // A lease is a lock on a blob that prevents other clients from modifying it.
    /// </summary>
    public async Task<string> AcquireBlobLeaseAsync(
        string containerName,
        string blobName,
        TimeSpan duration)
    {
        var blobClient = _blobServiceClient
            .GetBlobContainerClient(containerName)
            .GetBlobClient(blobName);

        var leaseClient = blobClient.GetBlobLeaseClient();// get lease client for the blob
        var lease = await leaseClient.AcquireAsync(duration);// acquire lease for specified duration

        _logger.LogInformation("Lease acquired for: {BlobName}, LeaseId: {LeaseId}",
            blobName, lease.Value.LeaseId);

        return lease.Value.LeaseId;
    }

    /// <summary>
    /// Release blob lease
    /// </summary>
    public async Task ReleaseBlobLeaseAsync(
        string containerName,
        string blobName,
        string leaseId)
    {
        var blobClient = _blobServiceClient
            .GetBlobContainerClient(containerName)
            .GetBlobClient(blobName);

        var leaseClient = blobClient.GetBlobLeaseClient(leaseId);
        await leaseClient.ReleaseAsync();

        _logger.LogInformation("Lease released for: {BlobName}", blobName);
    }
}
