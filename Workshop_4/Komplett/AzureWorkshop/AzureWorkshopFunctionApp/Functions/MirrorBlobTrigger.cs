using Azure.Storage.Blobs;
using AzureWorkshopFunctionApp.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading.Tasks;

namespace AzureWorkshopFunctionApp.Functions
{
    public class MirrorBlobTrigger
    {
        private readonly IImageService _imageService;
        private readonly IBlobStorageService _blobStorageService;
        private readonly ILogger<MirrorBlobTrigger> _logger;

        public MirrorBlobTrigger(
            IImageService imageService,
            IBlobStorageService blobStorageService,
            ILogger<MirrorBlobTrigger> logger)
        {
            _imageService = imageService;
            _blobStorageService = blobStorageService;
            _logger = logger;
        }

        [Function(nameof(MirrorBlobTrigger))]
        public async Task Run(
            [BlobTrigger(Constants.ImageContainer + "/{name}")] Stream inputBlob,
            string name) // 'name' is captured from the blob path
        {
            _logger.LogInformation($"Mirroring blob: {name}");

            inputBlob.Position = 0;

            var flippedImageStream = _imageService.FlipHorizontal(inputBlob);

            await _blobStorageService.UploadStreamToBlob(
                Constants.MirrorImageContainer,
                name,
                flippedImageStream
            );
        }
    }
}