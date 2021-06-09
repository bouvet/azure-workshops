using AzureWorkshopApp.Models;

namespace AzureWorkshopApp.Helpers
{
    public class StorageConfigValidator
    {
        public static AzureStorageConfigValidationResult Validate(AzureStorageConfig storageConfig)
        {
            AzureStorageConfigValidationResult validation = new AzureStorageConfigValidationResult();

            if (storageConfig.ConnectionString == string.Empty)
            {
                validation.AddError("ConnectionString", "ConnectionString key is empty. Check configuration.");
            }

            if (storageConfig.ImageContainer == string.Empty)
            {
                validation.AddError("ImageContainer", "Image container name is empty. Check configuration.");
            }

            return validation;
        }
    }
}
