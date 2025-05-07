using AzureWorkshopFunctionApp.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Threading.Tasks;

namespace AzureWorkshopFunctionApp.Functions
{
    public class TemplateQueueTrigger
    {
        private readonly IBlobStorageService _blobStorageService;
        private readonly IImageService _imageService;
        private readonly ILogger<TemplateQueueTrigger> _logger;

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
            
            // 1. Test med � sende en melding til storage k�en
            // 2. Hent blob fra blob storage med navnet fra k�en
            // 3. Greyscale transform p� bloben
            // 4. Upload blob til blob container imagecontainer-grey/Constants.GreyImageContainer
            
            // Her kan du velge � enten bruke bindings eller SDKs, men vi anbefaler at du bruker bindings for b�de skriving og lesing.
        }
    }

}