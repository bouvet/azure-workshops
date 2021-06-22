using Azure.Storage.Blobs;
using AzureWorkshopFunctionApp.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AzureWorkshopFunctionApp.Services
{
    public class BlobStorageService : IBlobStorageService
    {
        private readonly string _connectionString;

        public BlobStorageService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<Stream> GetBlobAsStream(string container, string blobName)
        {
            // Create a BlobServiceClient object which will be used to create a container client
            BlobServiceClient blobServiceClient = new BlobServiceClient(_connectionString);

            //Create a BlobContainerClient
            var blobContainerClient = blobServiceClient.GetBlobContainerClient(container);

            var blobClient = blobContainerClient.GetBlobClient(blobName);

            var ms = new MemoryStream();

            await blobClient.DownloadToAsync(ms);

            return ms;
        }

        public async Task UploadStreamToBlob(string container, string blobName, Stream stream)
        {
            //In case the stream is not set to the beginning
            stream.Position = 0;
            // Create a BlobServiceClient object which will be used to create a container client
            BlobServiceClient blobServiceClient = new BlobServiceClient(_connectionString);

            //Create a BlobContainerClient
            var blobContainerClient = blobServiceClient.GetBlobContainerClient(container);
            await blobContainerClient.CreateIfNotExistsAsync();

            var blobClient = blobContainerClient.GetBlobClient(blobName);

            await blobClient.UploadAsync(stream, overwrite: true);
        }

        public async Task<IEnumerable<string>> GetFileNamesInContainer(string container)
        {
            var list = new List<string>();
            // Create a BlobServiceClient object which will be used to create a container client
            BlobServiceClient blobServiceClient = new BlobServiceClient(_connectionString);

            //Create a BlobContainerClient
            var blobContainerClient = blobServiceClient.GetBlobContainerClient(container);
            await blobContainerClient.CreateIfNotExistsAsync();

            await foreach (var blobItem in blobContainerClient.GetBlobsAsync())
            {
                list.Add(blobItem.Name);
            }
            return list.Distinct();
        }
    }
}
