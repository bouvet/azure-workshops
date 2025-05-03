using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using AzureWorkshopFunctionApp.Interfaces;

namespace AzureWorkshopFunctionApp.Functions
{
    public class TemplateQueueTrigger
    {
        private IBlobStorageService _blobStorageService { get; set; }
        private IImageService _imageService { get; set; }
        private ILogger _logger { get; set; }

        public TemplateQueueTrigger(IBlobStorageService blobStorageService, IImageService imageService, ILogger<TemplateQueueTrigger> logger)
        {
            _blobStorageService = blobStorageService;
            _imageService = imageService;
            _logger = logger;
        }

        [Function("TemplateQueueTrigger")]
        public void Run([QueueTrigger(Constants.GreyImageQueue)]string myQueueItem)
        {
            _logger.LogInformation($"C# Queue trigger function processed: {myQueueItem}");
        }
    }
}
