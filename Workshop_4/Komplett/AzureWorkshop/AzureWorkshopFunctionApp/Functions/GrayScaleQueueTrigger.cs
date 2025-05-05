using AzureWorkshopFunctionApp.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Functions.Worker;
using System.Threading.Tasks;

namespace AzureWorkshopFunctionApp.Functions
{
    public class GrayScaleQueueTrigger
    {

        private readonly IBlobStorageService _blobStorageService;
        private readonly IImageService _imageService;
        private readonly ILogger<GrayScaleQueueTrigger> _logger;

        public GrayScaleQueueTrigger(IImageService imageService, IBlobStorageService blobStorageService, ILogger<GrayScaleQueueTrigger> logger)
        {
            _imageService = imageService;
            _blobStorageService = blobStorageService;
            _logger = logger;
        }

        [Function("GrayScaleQueueTrigger")]
        [QueueOutput(Constants.SquareImageQueue)]
        public async Task<string> Run([QueueTrigger(Constants.GreyImageQueue)] string imageName)
        {
            _logger.LogInformation($"C# Queue trigger function processed: {imageName}");

            var blobStream = await _blobStorageService.GetBlobAsStream(Constants.ImageContainer, imageName);

            _logger.LogInformation($"BlobService has connectionString");
            var greyStream = _imageService.GreyScale(blobStream);
            greyStream.Position = 0;

            await _blobStorageService.UploadStreamToBlob(Constants.GreyImageContainer, imageName, greyStream);

            _logger.LogInformation($"Finished processing");
            return imageName;
        }
    }
}
