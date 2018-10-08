using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AzureWorkshopApp.Models;
using Microsoft.AspNetCore.Http;

namespace AzureWorkshopApp.Helpers
{
    public static class StorageHelper
    {

        public static bool IsImage(IFormFile file)
        {
            if (file.ContentType.Contains("image"))
            {
                return true;
            }

            string[] formats = { ".jpg", ".png", ".gif", ".jpeg" };

            return formats.Any(item => file.FileName.EndsWith(item, StringComparison.OrdinalIgnoreCase));
        }

        public static async Task<bool> UploadFileToStorage(Stream fileStream, string fileName, AzureStorageConfig storageConfig)
        {

            return await Task.FromResult(true);
        }

        public static async Task<List<string>> GetImageUrls(AzureStorageConfig storageConfig)
        {
            List<string> imageUrls = new List<string>();


            return await Task.FromResult(imageUrls);
        }
    }
}
