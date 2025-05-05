using AzureWorkshopFunctionApp.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading.Tasks;

namespace AzureWorkshopFunctionApp.Functions
{
    public class ExampleFunctionHttpTriggerSDK
    {
        private IBlobStorageService _blobStorageService { get; set; }
        private IImageService _imageService { get; set; }
        private readonly ILogger<ExampleFunctionHttpTriggerSDK> _logger;


        public ExampleFunctionHttpTriggerSDK(IImageService imageService, IBlobStorageService blobStorageService, ILogger<ExampleFunctionHttpTriggerSDK> logger)
        {
            _blobStorageService = blobStorageService;
            _imageService = imageService;
            _logger = logger;
        }

        [Function("ExampleFunctionHttpTriggerSDK")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req)
        {
            try
            {
                var blobName = req.Query["blobName"];

                if (string.IsNullOrEmpty(blobName))
                {
                    var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                    return badResponse;
                }

                var stream = await _blobStorageService.GetBlobAsStream(Constants.ImageContainer, blobName);
                if (stream == null)
                {
                    var notFoundResponse = req.CreateResponse(HttpStatusCode.NotFound);
                    return notFoundResponse;
                }

                var mirror = _imageService.FlipHorizontal(stream);

                await _blobStorageService.UploadStreamToBlob(Constants.MirrorImageContainer, blobName, mirror);

                return req.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing request");
                var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
                return errorResponse;
            }
        }

    }
}
