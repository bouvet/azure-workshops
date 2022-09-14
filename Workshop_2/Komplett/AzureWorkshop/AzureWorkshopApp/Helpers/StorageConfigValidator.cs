using AzureWorkshopApp.Models;

namespace AzureWorkshopApp.Helpers
{
    public class StorageConfigValidator
    {
        public static AzureStorageConfigValidationResult Validate(AzureStorageConfig storageConfig)
        {
            AzureStorageConfigValidationResult validation = new AzureStorageConfigValidationResult();

            if (storageConfig.AccountKey == string.Empty)
            {
                validation.AddError("AccountKey", "AccountKey key is empty. Check configuration.");
            }

            if (storageConfig.AccountName == string.Empty)
            {
                validation.AddError("AccountName", "AccountName key is empty. Check configuration.");
            }

            if (storageConfig.ImageContainer == string.Empty)
            {
                validation.AddError("ImageContainer", "Image container name is empty. Check configuration.");
            }

            return validation;
        }
    }
}
