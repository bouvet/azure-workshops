using AzureWorkshopFunctionApp.Interfaces;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace AzureWorkshopFunctionApp.Functions
{
    public class TemplateQueueTrigger
    {
        private IBlobStorageService BlobStorageService { get; set; }
        private IImageService ImageService { get; set; }

        public TemplateQueueTrigger(IBlobStorageService blobStorageService, IImageService imageService)
        {
            BlobStorageService = blobStorageService;
            ImageService = imageService;
        }

        [FunctionName("TemplateQueueTrigger")]
        public void Run([QueueTrigger(Constants.GreyImageQueue)]string myQueueItem, ILogger log)
        {
            log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");
        }
    }
}
