using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using AzureWorkshopFunctionApp.Interfaces;

namespace AzureWorkshopFunctionApp.Functions
{
    public class BlurServiceBusTrigger
    {
        private readonly IBlobStorageService _blobStorageService;
        private readonly IImageService _imageService;
        private readonly ILogger<BlurServiceBusTrigger> _logger;
        public BlurServiceBusTrigger(IImageService imageService, IBlobStorageService blobStorageService, ILogger<BlurServiceBusTrigger> logger)
        {
            _imageService = imageService;
            _blobStorageService = blobStorageService;
            _logger = logger;
        }

        [Function("BlurServiceBusTrigger")]
        public async Task Run([ServiceBusTrigger(Constants.ImageQueue, Connection = Constants.SBConnectionString)] string blobName)
        {
            _logger.LogInformation($"C# ServiceBus queue trigger function processed message: {blobName}");

            var blobStream = await _blobStorageService.GetBlobAsStream(Constants.ImageContainer, blobName);

            _logger.LogInformation($"C# ServiceBus queue trigger blob length: {blobStream.Length}");
            var blurStream = _imageService.Blur(blobStream);
            _logger.LogInformation($"C# ServiceBus queue trigger blur length: {blurStream.Length}");
            await _blobStorageService.UploadStreamToBlob(Constants.BlurImageContainer, "softblur-"+blobName, blurStream);

            blobStream.Position = 0;
            _imageService.SoftBlurImage = false;

            blurStream = _imageService.Blur(blobStream);
            await _blobStorageService.UploadStreamToBlob(Constants.BlurImageContainer, "hardblur-" + blobName, blurStream);
            _logger.LogInformation($"Uploaded blob to {Constants.BlurImageContainer}");
        }
    }
}
