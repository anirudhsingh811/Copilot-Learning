using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace AZ204.RealFunctions;

// Learning Summary: This code defines real blob-triggered Azure Functions using C# 12.0 features. 
// It includes three functions: one for generating image thumbnails, another for processing CSV file imports, and a third for archiving documents.
//  The code demonstrates dependency injection for logging, blob trigger bindings, and asynchronous processing.

// Key Concept and Learning : Blob Triggers in Azure Functions allow you to automatically execute code in response to changes in Azure Blob Storage.
// This is useful for scenarios such as image processing, data import/export, and document management.
//  Blob triggers monitor a specified blob container and invoke the function whenever a new blob is added or an existing blob is updated.
//  This enables automated workflows that respond to file uploads without the need for manual intervention.
// The code also showcases best practices in error handling and logging within Azure Functions.

/// <summary>
/// Real Blob Triggered Azure Functions
/// Triggered when files are uploaded to Azure Blob Storage
/// </summary>
public class BlobTriggerFunctions
{
    private readonly ILogger<BlobTriggerFunctions> _logger;

    public BlobTriggerFunctions(ILogger<BlobTriggerFunctions> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Processes images uploaded to the "uploads" container
    /// Automatically creates thumbnails
    /// </summary>
    [Function("ImageThumbnailGenerator")]
    public async Task GenerateThumbnail(
        [BlobTrigger("uploads/{name}", Connection = "StorageConnection")] byte[] imageBlob,
        string name)
    {
        _logger.LogInformation($"???  Processing image: {name}");// Log image name
        _logger.LogInformation($"   Size: {imageBlob.Length / 1024} KB"); // Log image size in KB

        try
        {
            // Simulate thumbnail generation
            _logger.LogInformation("???  Step 1: Validating image format...");
            await Task.Delay(200);

            _logger.LogInformation("???  Step 2: Generating thumbnail (200x200)...");
            await Task.Delay(500);

            _logger.LogInformation("???  Step 3: Optimizing image quality...");
            await Task.Delay(300);

            // In real implementation:
            // 1. Use ImageSharp or similar library to resize
            // 2. Use BlobClient to upload to "thumbnails" container
            // 3. Return the thumbnail bytes or use [BlobOutput] binding

            _logger.LogInformation($"? Thumbnail created for {name}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"? Failed to generate thumbnail for {name}");
            throw;
        }
    }

    /// <summary>
    /// Processes CSV files for data import
    /// Triggered when files are uploaded to "data-imports" container
    /// </summary>
    [Function("ProcessCsvImport")]
    public async Task ProcessCsvFile(
        [BlobTrigger("data-imports/{name}.csv", Connection = "StorageConnection")] Stream csvStream,
        string name)
    {
        _logger.LogInformation($"?? Processing CSV file: {name}.csv");

        try
        {
            using var reader = new StreamReader(csvStream);
            
            _logger.LogInformation("?? Step 1: Reading CSV data...");
            var lineCount = 0;
            while (!reader.EndOfStream)
            {
                var line = await reader.ReadLineAsync();
                lineCount++;
            }

            _logger.LogInformation($"?? Step 2: Parsed {lineCount} rows");
            await Task.Delay(500);

            _logger.LogInformation("?? Step 3: Validating data...");
            await Task.Delay(300);

            _logger.LogInformation("?? Step 4: Importing to database...");
            await Task.Delay(1000);

            _logger.LogInformation($"? Successfully imported {lineCount} records from {name}.csv");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"? Failed to process CSV file: {name}");
            throw;
        }
    }

    /// <summary>
    /// Archives old documents
    /// Triggered when files are uploaded to "documents" container
    /// </summary>
    [Function("ArchiveDocument")]
    public async Task ArchiveDocument(
        [BlobTrigger("documents/{name}", Connection = "StorageConnection")] Stream documentStream,
        string name)
    {
        _logger.LogInformation($"?? Archiving document: {name}");

        try
        {
            _logger.LogInformation("?? Step 1: Scanning for sensitive data...");
            await Task.Delay(300);

            _logger.LogInformation("?? Step 2: Encrypting document...");
            await Task.Delay(500);

            _logger.LogInformation("?? Step 3: Would copy to archive container...");
            // In production: Use BlobClient to upload to "archive" container
            await Task.Delay(200);

            _logger.LogInformation($"? Document {name} processed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"? Failed to archive document: {name}");
            throw;
        }
    }
}
