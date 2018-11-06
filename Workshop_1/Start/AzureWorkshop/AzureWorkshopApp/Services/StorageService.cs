using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
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
            throw new NotImplementedException();
        }

        public async Task<List<string>> GetImageUrls()
        {
            throw new NotImplementedException();
        }
    }
}