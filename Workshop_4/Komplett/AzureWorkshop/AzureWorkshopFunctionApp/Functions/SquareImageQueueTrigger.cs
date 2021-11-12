using AzureWorkshopFunctionApp.Interfaces;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace AzureWorkshopFunctionApp.Functions
{
    public class SquareImageQueueTrigger
    {
        private IBlobStorageService BlobStorageService { get; }
        private IImageService ImageService { get; }

        public SquareImageQueueTrigger(IImageService imageService, IBlobStorageService blobStorageService)
        {
            BlobStorageService = blobStorageService;
            ImageService = imageService;
        }

        [FunctionName("SquareImageQueueTrigger")]
        public async Task Run([QueueTrigger(Constants.SquareImageQueue)] string imageName, ILogger log)
        {
            log.LogInformation($"C# Queue trigger function processed: {imageName}");

            var blobStream = await BlobStorageService.GetBlobAsStream(Constants.ImageContainer, imageName);

            log.LogInformation($"BlobService has connectionString");
            var squareImageStream = ImageService.Square(blobStream);

            await BlobStorageService.UploadStreamToBlob(Constants.SquareImageContainer, imageName, squareImageStream);
            log.LogInformation($"Finished processing");
        }
    }
}
