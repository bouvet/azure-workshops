using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AzureWorkshopApp.Models;

namespace AzureWorkshopApp.Services
{
    public interface IStorageService
    {
        AzureStorageConfigValidationResult ValidateConfiguration();
        Task<bool> UploadFileToStorage(Stream fileStream, string fileName, string containerName);
        Task<List<string>> GetImageUrls(string containerName);
    }
}