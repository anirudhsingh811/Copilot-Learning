using Azure;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Azure.Security.KeyVault.Keys;
using Azure.Security.KeyVault.Certificates;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AZ204.Security.Services;

public class KeyVaultService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<KeyVaultService> _logger;
    private SecretClient? _secretClient;
    private KeyClient? _keyClient;
    private CertificateClient? _certificateClient;

    public KeyVaultService(IConfiguration configuration, ILogger<KeyVaultService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    private void InitializeClients()
    {
        var vaultUri = _configuration["KeyVault:VaultUri"]; // e.g., https://your-key-vault-name.vault.azure.net/ from appsettings.json
        if (string.IsNullOrEmpty(vaultUri))
        {
            throw new InvalidOperationException("KeyVault:VaultUri not configured in appsettings.json");
        }

        // Use DefaultAzureCredential for authentication
        // This supports multiple authentication methods in order:
        // 1. Environment variables
        // 2. Managed Identity
        // 3. Visual Studio
        // 4. Azure CLI
        // 5. Azure PowerShell
        var credential = new DefaultAzureCredential(); // Adjust as needed for your environment

        _secretClient = new SecretClient(new Uri(vaultUri), credential); // what is Secret Client ? // The SecretClient class is used to interact with Azure Key Vault secrets.
        
        _keyClient = new KeyClient(new Uri(vaultUri), credential);// The KeyClient class is used to interact with Azure Key Vault keys.
        _certificateClient = new CertificateClient(new Uri(vaultUri), credential); // The CertificateClient class is used to interact with Azure Key Vault certificates.
    }

    public async Task RunKeyVaultDemoAsync()
    {
        try
        {
            _logger.LogInformation("=== Azure Key Vault Demo ===");
            Console.WriteLine("\n=== Azure Key Vault Demo ===\n");

            InitializeClients();

            await SecretsDemo();
            await KeysDemo();
            await CertificatesDemo();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error running Key Vault demo");
            Console.WriteLine($"\n? Error: {ex.Message}");
            Console.WriteLine("\n?? Make sure you have:");
            Console.WriteLine("   1. Created a Key Vault in Azure");
            Console.WriteLine("   2. Updated appsettings.json with your Key Vault URI");
            Console.WriteLine("   3. Authenticated via Azure CLI: az login");
            Console.WriteLine("   4. Given your user permissions (Get, List, Set, Delete) on secrets, keys, and certificates");
        }
    }
    /// <summary>
    /// // Demonstrates secret management in Azure Key Vault
    /// // Creates, retrieves, updates, lists, and deletes secrets
    /// </summary>
    /// <returns></returns>
    private async Task SecretsDemo()
    {
        Console.WriteLine("?? Secrets Management\n");

        try
        {
            // Create a secret
            string secretName = "demo-secret";
            string secretValue = $"Secret-Value-{DateTime.UtcNow:yyyyMMdd-HHmmss}";

            Console.WriteLine($"Creating secret: {secretName}");
            KeyVaultSecret secret = await _secretClient!.SetSecretAsync(secretName, secretValue); // The SetSecretAsync method creates or updates a secret in Azure Key Vault.
            Console.WriteLine($"? Secret created with version: {secret.Properties.Version}");

            // Retrieve the secret
            Console.WriteLine($"\nRetrieving secret: {secretName}");
            KeyVaultSecret retrievedSecret = await _secretClient.GetSecretAsync(secretName);// The GetSecretAsync method retrieves a secret from Azure Key Vault.
            Console.WriteLine($"? Secret value: {retrievedSecret.Value}");

            // Update secret (creates new version)
            string newValue = $"Updated-{DateTime.UtcNow:yyyyMMdd-HHmmss}";
            Console.WriteLine($"\nUpdating secret to new value");
            await _secretClient.SetSecretAsync(secretName, newValue);// Creates a new version of the secret with the updated value.
            Console.WriteLine($"? Secret updated");

            // List secret versions
            Console.WriteLine($"\nListing all versions of secret: {secretName}");
            int versionCount = 0;
            await foreach (SecretProperties version in _secretClient.GetPropertiesOfSecretVersionsAsync(secretName)) // Who set version ? // The GetPropertiesOfSecretVersionsAsync method retrieves all versions of a specified secret.// Each version is represented by a SecretProperties object.
            {
                versionCount++;
                Console.WriteLine($"   Version {versionCount}: {version.Version} (Created: {version.CreatedOn})");
            }

            // List all secrets
            Console.WriteLine($"\nListing all secrets in Key Vault:");
            await foreach (SecretProperties secretProps in _secretClient.GetPropertiesOfSecretsAsync())
            {
                Console.WriteLine($"   - {secretProps.Name}");
            }

            // Delete secret (soft delete)
            Console.WriteLine($"\nSoft deleting secret: {secretName}");
            DeleteSecretOperation operation = await _secretClient.StartDeleteSecretAsync(secretName);// Does it delete all version ? // The StartDeleteSecretAsync method initiates the deletion of a secret from Azure Key Vault.// It returns a DeleteSecretOperation that can be used to track the deletion process.
            await operation.WaitForCompletionAsync();
            Console.WriteLine($"? Secret soft deleted");

            Console.WriteLine("\n");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in secrets demo");
            Console.WriteLine($"? Secrets demo error: {ex.Message}\n");
        }
    }

    private async Task KeysDemo()
    {
        Console.WriteLine("?? Cryptographic Keys Management\n");

        try
        {
            string keyName = "demo-rsa-key";

            // Create an RSA key
            Console.WriteLine($"Creating RSA key: {keyName}");
            var keyOptions = new CreateRsaKeyOptions(keyName, hardwareProtected: false) // The CreateRsaKeyOptions class is used to specify options when creating an RSA key in Azure Key Vault.
            {
                KeySize = 2048,
                ExpiresOn = DateTimeOffset.UtcNow.AddYears(1)
            };

            KeyVaultKey key = await _keyClient!.CreateRsaKeyAsync(keyOptions);// The CreateRsaKeyAsync method creates a new RSA key in Azure Key Vault using the specified options.
            Console.WriteLine($"? Key created: {key.Name} (Type: {key.KeyType})");
            Console.WriteLine($"   Key ID: {key.Id}");

            // Retrieve the key
            Console.WriteLine($"\nRetrieving key: {keyName}");
            KeyVaultKey retrievedKey = await _keyClient.GetKeyAsync(keyName);
            Console.WriteLine($"? Key retrieved: {retrievedKey.Name}");
            Console.WriteLine($"   Enabled: {retrievedKey.Properties.Enabled}");
            Console.WriteLine($"   Created: {retrievedKey.Properties.CreatedOn}");

            // List all keys
            Console.WriteLine($"\nListing all keys in Key Vault:");
            await foreach (KeyProperties keyProps in _keyClient.GetPropertiesOfKeysAsync())// The GetPropertiesOfKeysAsync method retrieves all keys in Azure Key Vault.// Each key is represented by a KeyProperties object.
            {
                Console.WriteLine($"   - {keyProps.Name}");
            }

            // Delete key (soft delete)
            Console.WriteLine($"\nSoft deleting key: {keyName}");
            DeleteKeyOperation deleteOperation = await _keyClient.StartDeleteKeyAsync(keyName);//   The StartDeleteKeyAsync method initiates the deletion of a key from Azure Key Vault.// It returns a DeleteKeyOperation that can be used to track the deletion process.
            await deleteOperation.WaitForCompletionAsync();
            Console.WriteLine($"? Key soft deleted");

            Console.WriteLine("\n");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in keys demo");
            Console.WriteLine($"? Keys demo error: {ex.Message}\n");
        }
    }

    private async Task CertificatesDemo()
    {
        Console.WriteLine("?? Certificates Management\n");

        try
        {
            string certName = "demo-certificate";

            // Create a self-signed certificate
            Console.WriteLine($"Creating self-signed certificate: {certName}");
            var policy = new CertificatePolicy("CN=AZ204Demo", "Self") // What is CertificatePolicy ? // The CertificatePolicy class defines the policy for a certificate in Azure Key Vault, including its subject name, issuer, key type, and validity period.
            {
                KeyType = CertificateKeyType.Rsa,
                KeySize = 2048,
                ReuseKey = true,
                ContentType = CertificateContentType.Pkcs12,
                ValidityInMonths = 12
            };

            CertificateOperation operation = await _certificateClient!.StartCreateCertificateAsync(certName, policy); // The StartCreateCertificateAsync method initiates the creation of a new certificate in Azure Key Vault using the specified policy.// It returns a CertificateOperation that can be used to track the creation process.

            Console.WriteLine("Waiting for certificate creation...");
            
            // Wait for completion with timeout
            var timeout = TimeSpan.FromSeconds(30);
            var startTime = DateTime.UtcNow;
            
            while (!operation.HasCompleted && (DateTime.UtcNow - startTime) < timeout)
            {
                await Task.Delay(1000);
                await operation.UpdateStatusAsync();
            }

            if (operation.HasCompleted)
            {
                KeyVaultCertificateWithPolicy certificate = operation.Value;
                Console.WriteLine($"? Certificate created: {certificate.Name}");
                Console.WriteLine($"   Thumbprint: {BitConverter.ToString(certificate.Properties.X509Thumbprint.ToArray()).Replace("-", "")}");
                Console.WriteLine($"   Expires: {certificate.Properties.ExpiresOn}");
            }

            // List all certificates
            Console.WriteLine($"\nListing all certificates in Key Vault:");
            await foreach (CertificateProperties certProps in _certificateClient.GetPropertiesOfCertificatesAsync())// The GetPropertiesOfCertificatesAsync method retrieves all certificates in Azure Key Vault.// Each certificate is represented by a CertificateProperties object.
            {
                Console.WriteLine($"   - {certProps.Name} (Expires: {certProps.ExpiresOn})");
            }

            // Delete certificate (soft delete)
            Console.WriteLine($"\nSoft deleting certificate: {certName}");
            DeleteCertificateOperation deleteOperation = await _certificateClient.StartDeleteCertificateAsync(certName);// The StartDeleteCertificateAsync method initiates the deletion of a certificate from Azure Key Vault.// It returns a DeleteCertificateOperation that can be used to track the deletion process.
            await deleteOperation.WaitForCompletionAsync();
            Console.WriteLine($"? Certificate soft deleted");

            Console.WriteLine("\n");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in certificates demo");
            Console.WriteLine($"? Certificates demo error: {ex.Message}\n");
        }
    }
}
