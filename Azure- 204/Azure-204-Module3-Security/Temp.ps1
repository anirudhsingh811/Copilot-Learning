# Get your user object ID (not email)
$userId = az ad signed-in-user show --query id -o tsv
echo "Your User ID: $userId"


# Set your Key Vault name and get subscription ID
$keyVaultName = "kv-az204-demo-8919"
$subscriptionId = az account show --query id -o tsv

# Build the Key Vault resource ID
$keyVaultId = "/subscriptions/$subscriptionId/resourceGroups/rg-keyvault-demo/providers/Microsoft.KeyVault/vaults/$keyVaultName"

# Assign Key Vault Secrets Officer (for secrets demo)
az role assignment create `
  --role "Key Vault Secrets Officer" `
  --assignee $userId `
  --scope $keyVaultId

# Assign Key Vault Crypto Officer (for keys demo)
az role assignment create `
  --role "Key Vault Crypto Officer" `
  --assignee $userId `
  --scope $keyVaultId

# Assign Key Vault Certificates Officer (for certificates demo)
az role assignment create `
  --role "Key Vault Certificates Officer" `
  --assignee $userId `
  --scope $keyVaultId
  
  
  # List your role assignments on the Key Vault
az role assignment list --scope $keyVaultId --assignee $userId --output table