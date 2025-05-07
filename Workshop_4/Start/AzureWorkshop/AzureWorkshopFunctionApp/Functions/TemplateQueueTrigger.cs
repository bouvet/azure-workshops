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
            
            // 1. Test med å sende en melding til storage køen
            // 2. Hent blob fra blob storage med navnet fra køen
            // 3. Greyscale transform på bloben
            // 4. Upload blob til blob container imagecontainer-grey/Constants.GreyImageContainer
            
            // Her kan du velge å enten bruke bindings eller SDKs, men vi anbefaler at du bruker bindings for både skriving og lesing.
        }
    }

}