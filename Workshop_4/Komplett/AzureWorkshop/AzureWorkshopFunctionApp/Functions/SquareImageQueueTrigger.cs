using AzureWorkshopFunctionApp.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace AzureWorkshopFunctionApp.Functions
{
    public class SquareImageQueueTrigger
    {
        private readonly IBlobStorageService _blobStorageService;
        private readonly IImageService _imageService;
        private readonly ILogger<SquareImageQueueTrigger> _logger;

        public SquareImageQueueTrigger(IImageService imageService, IBlobStorageService blobStorageService , ILogger<SquareImageQueueTrigger> logger)
        {
            _blobStorageService = blobStorageService;
            _imageService = imageService;
            _logger = logger;
        }

        [Function("SquareImageQueueTrigger")]
        public async Task Run([QueueTrigger(Constants.SquareImageQueue)] string imageName)
        {
            _logger.LogInformation($"C# Queue trigger function processed: {imageName}");

            var blobStream = await _blobStorageService.GetBlobAsStream(Constants.ImageContainer, imageName);

            _logger.LogInformation($"BlobService has connectionString");
            var squareImageStream = _imageService.Square(blobStream);

            await _blobStorageService.UploadStreamToBlob(Constants.SquareImageContainer, imageName, squareImageStream);
            _logger.LogInformation($"Finished processing");
        }
    }
}
