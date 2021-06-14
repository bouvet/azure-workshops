using AzureWorkshopFunctionApp.Interfaces;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Drawing.Imaging;
using System.Threading.Tasks;

namespace AzureWorkshopFunctionApp.Functions
{
    public class GrayScaleQueueTrigger
    {

        private IBlobStorageService BlobStorageService { get; set; }
        private IImageService ImageService { get; set; }

        public GrayScaleQueueTrigger(IImageService imageService, IBlobStorageService blobStorageService)
        {
            BlobStorageService = blobStorageService; 
            ImageService = imageService;
        }

        [FunctionName("GrayScaleQueueTrigger")]
        public async Task Run([QueueTrigger("greyimage", Connection = "AzureWebJobsStorage")]string imageName, ILogger log)
        {
            log.LogInformation($"C# Queue trigger function processed: {imageName}");

            var blobStream = await BlobStorageService.GetBlobAsStream("imagecontainer", imageName);

            var greyStream = ImageService.GreyScale(blobStream, ImageFormat.Jpeg);
            greyStream.Position = 0;

            await BlobStorageService.UploadStreamToBlob("imagecontainer-grey", imageName, greyStream);
        }
    }
}
