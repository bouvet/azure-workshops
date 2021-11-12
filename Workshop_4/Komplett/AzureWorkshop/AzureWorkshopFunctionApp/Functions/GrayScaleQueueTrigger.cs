using AzureWorkshopFunctionApp.Interfaces;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace AzureWorkshopFunctionApp.Functions
{
    public class GrayScaleQueueTrigger
    {

        private IBlobStorageService BlobStorageService { get; }
        private IImageService ImageService { get; }

        public GrayScaleQueueTrigger(IImageService imageService, IBlobStorageService blobStorageService)
        {
            BlobStorageService = blobStorageService;
            ImageService = imageService;
        }

        [FunctionName("GrayScaleQueueTrigger")]
        [return: Queue(Constants.SquareImageQueue)]
        public async Task<string> Run([QueueTrigger(Constants.GreyImageQueue)] string imageName,
            ILogger log)
        {
            log.LogInformation($"C# Queue trigger function processed: {imageName}");

            var blobStream = await BlobStorageService.GetBlobAsStream(Constants.ImageContainer, imageName);

            log.LogInformation($"BlobService has connectionString");
            var greyStream = ImageService.GreyScale(blobStream);
            greyStream.Position = 0;

            await BlobStorageService.UploadStreamToBlob(Constants.GreyImageContainer, imageName, greyStream);

            log.LogInformation($"Finished processing");
            return imageName;
        }
    }
}
