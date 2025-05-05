using AzureWorkshopFunctionApp.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading.Tasks;

namespace AzureWorkshopFunctionApp.Functions
{
    public class ExampleFunctionHttpTriggerBindings
    {
        private readonly IBlobStorageService _blobStorageService;
        private readonly IImageService _imageService;
        private readonly ILogger<ExampleFunctionHttpTriggerBindings> _logger;

        public ExampleFunctionHttpTriggerBindings(IImageService imageService, IBlobStorageService blobStorageService, ILogger<ExampleFunctionHttpTriggerBindings> logger)
        {
            _blobStorageService = blobStorageService;
            _imageService = imageService;
            _logger = logger;
        }

        [Function("ExampleFunctionHttpTriggerBindings")]
        [BlobOutput("imagecontainer-mirror/{blobName}", Connection = "AzureWebJobsStorage")]
        public async Task<Stream> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req,
            [BlobInput("imagecontainer/{blobName}", Connection = "AzureWebJobsStorage")] Stream myBlob, 
            string blobName)
        {
            try
            {

                if (string.IsNullOrEmpty(blobName))
                {
                    _logger.LogError("Wrong query parameter");
                    return null;
                }

                _logger.LogInformation($"Processing blob: {blobName}");

                if (myBlob == null)
                {
                    _logger.LogError("Blob not found");
                    return null;
                }

                var mirror = _imageService.FlipHorizontal(myBlob);

                return mirror;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing request");
            }
            return null;
        }

    }
}
