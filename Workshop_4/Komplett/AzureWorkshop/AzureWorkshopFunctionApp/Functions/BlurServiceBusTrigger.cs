using AzureWorkshopFunctionApp.Interfaces;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace AzureWorkshopFunctionApp.Functions
{
    public class BlurServiceBusTrigger
    {
        private IImageService ImageService { get; }
        private IBlobStorageService BlobStorageService { get; }
        public BlurServiceBusTrigger(IImageService imageService, IBlobStorageService blobStorageService)
        {
            ImageService = imageService;
            BlobStorageService = blobStorageService;
        }

        [FunctionName("BlurServiceBusTrigger")]
        public async Task Run([ServiceBusTrigger(Constants.ImageQueue, Connection = Constants.SBConnectionString)] string blobName, ILogger log)
        {
            log.LogInformation($"C# ServiceBus queue trigger function processed message: {blobName}");

            var blobStream = await BlobStorageService.GetBlobAsStream(Constants.ImageContainer, blobName);

            log.LogInformation($"C# ServiceBus queue trigger blob length: {blobStream.Length}");
            var blurStream = ImageService.Blur(blobStream);
            log.LogInformation($"C# ServiceBus queue trigger blur length: {blurStream.Length}");
            await BlobStorageService.UploadStreamToBlob(Constants.BlurImageContainer, "softblur-"+blobName, blurStream);

            blobStream.Position = 0;
            ImageService.SoftBlurImage = false;

            blurStream = ImageService.Blur(blobStream);
            await BlobStorageService.UploadStreamToBlob(Constants.BlurImageContainer, "hardblur-" + blobName, blurStream);
            log.LogInformation($"Uploaded blob to {Constants.BlurImageContainer}");
        }
    }
}
