using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using AzureWorkshopApp.Helpers;
using AzureWorkshopApp.Models;
using Microsoft.Extensions.Options;

namespace AzureWorkshopApp.Services
{
    public class StorageService : IStorageService
    {
        private readonly AzureStorageConfig _storageConfig;

        public StorageService(IOptions<AzureStorageConfig> storageConfig)
        {
            _storageConfig = storageConfig != null ? storageConfig.Value : throw new ArgumentNullException(nameof(storageConfig));
        }

        public AzureStorageConfigValidationResult ValidateConfiguration()
        {
            return StorageConfigValidator.Validate(_storageConfig);
        }

        public async Task<bool> UploadFileToStorage(Stream fileStream, string fileName)
        {
            // Create a BlobServiceClient object which will be used to create a container client
            BlobServiceClient blobServiceClient = new BlobServiceClient(_storageConfig.ConnectionString);

            //Create a BlobContainerClient
            var blobContainerClient = blobServiceClient.GetBlobContainerClient(_storageConfig.ImageContainer);

            //Create the container if it does not exist (can be removed)
            blobContainerClient.CreateIfNotExists();

            //Create BlobClient that points to a blob with the given filename
            var blobClient = blobContainerClient.GetBlobClient(fileName);

            //Upload the content
            await blobClient.UploadAsync(fileStream);

             return true;
        }

        public async Task<List<string>> GetImageUrls()
        {
            List<string> imageUrls = new List<string>();

            // Create a BlobServiceClient object which will be used to create a container client
            BlobServiceClient blobServiceClient = new BlobServiceClient(_storageConfig.ConnectionString);

            //Create a BlobContainerClient
            var blobContainerClient = blobServiceClient.GetBlobContainerClient(_storageConfig.ImageContainer);
            
            BlobSasBuilder builder;
            await foreach(var blobItem in blobContainerClient.GetBlobsAsync())
            {
                //Create a blobclient from the blobItem.Name
                var blobClient = blobContainerClient.GetBlobClient(blobItem.Name);

                //Create a shared access signature builder with name of the container, the blob, type of resource and expiration
                builder = new BlobSasBuilder()
                {
                    BlobContainerName = blobContainerClient.Name,
                    BlobName = blobClient.Name,
                    Resource = "b",
                    ExpiresOn = DateTime.UtcNow.AddMinutes(3),
                };

                //Set type of access, we only need read so we set that 
                builder.SetPermissions(BlobAccountSasPermissions.Read);

                //Create the sasUri and add it to the list
                imageUrls.Add(blobClient.GenerateSasUri(builder).AbsoluteUri);
            }

            return imageUrls;
        }
    }
}