using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using AzureWorkshopFunctionApp.Interfaces;

namespace AzureWorkshopFunctionApp.Functions
{
    public class TemplateQueueTrigger
    {
        private IBlobStorageService _blobStorageService { get; set; }
        private IImageService _imageService { get; set; }

        public TemplateQueueTrigger(IBlobStorageService blobStorageService, IImageService imageService)
        {
            _blobStorageService = blobStorageService;
            _imageService = imageService;
        }

        [Function("TemplateQueueTrigger")]
        public void Run([QueueTrigger(Constants.GreyImageQueue)]string myQueueItem, ILogger log)
        {
            log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");
        }
    }
}
