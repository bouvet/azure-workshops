using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using AzureWorkshopFunctionApp.Interfaces;

namespace AzureWorkshopFunctionApp
{
    public class ExampleFunctionHttpTrigger
    {
        private readonly ILogger<ExampleFunctionHttpTrigger> _logger;
        private IBlobStorageService _blobStorageService { get; set; }
        private IImageService _imageService { get; set; }

        public ExampleFunctionHttpTrigger(IImageService imageService, IBlobStorageService blobStorageService, ILogger<ExampleFunctionHttpTrigger> logger)
        {
            _blobStorageService = blobStorageService;
            _imageService = imageService;
            _logger = logger;
        }

        [Function("ExampleFunctionHttpTrigger")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData  req)
        {
            try
            {

                var blobName = req.Query["blobName"];

                var stream = await _blobStorageService.GetBlobAsStream(Constants.ImageContainer, blobName);
                if (stream == null)
                {
                    var notFoundResponse = req.CreateResponse(HttpStatusCode.NotFound);
                    await notFoundResponse.WriteStringAsync($"Blob {blobName} not found");
                    return notFoundResponse;
                }

                // 1. Perform image operation on stream (use imageService)
                // 2. Upload the processed image to blob storage (use blobStorageService)
                // 3. Return HttpResponse OK
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing request");
                var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
                await errorResponse.WriteStringAsync("An error occurred processing your request");
                return errorResponse;
            }
        }
    }
}
