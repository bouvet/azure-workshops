using AzureWorkshopFunctionApp.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;

namespace AzureWorkshopFunctionApp.Functions
{
    public class ExampleFunctionHttpTrigger
    {
        private readonly IBlobStorageService _blobStorageService;
        private readonly IImageService _imageService;
        private readonly ILogger<ExampleFunctionHttpTrigger> _logger;

        public ExampleFunctionHttpTrigger(IImageService imageService, IBlobStorageService blobStorageService, ILogger<ExampleFunctionHttpTrigger> logger)
        {
            _blobStorageService = blobStorageService;
            _imageService = imageService;
            _logger = logger;
        }

        [Function("ExampleFunctionHttpTrigger")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequestData req)
        {
            var blobName = req.Query["blobName"];
            var func = req.Query["func"];
            Stream imageStream;
            Stream changedImageStream;

            if (func != null)
            {
                switch (func)
                {
                    case "mirror":
                        imageStream = await _blobStorageService.GetBlobAsStream(Constants.ImageContainer, blobName);
                        changedImageStream = _imageService.FlipHorizontal(imageStream);
                        await _blobStorageService.UploadStreamToBlob(Constants.MirrorImageContainer, blobName, changedImageStream);
                        break;
                    case "flip":
                        imageStream = await _blobStorageService.GetBlobAsStream(Constants.ImageContainer, blobName);
                        changedImageStream = _imageService.FlipVertical(imageStream);
                        await _blobStorageService.UploadStreamToBlob(Constants.FlippedImageContainer, blobName, changedImageStream);
                        break;
                    case "rotate":
                        imageStream = await _blobStorageService.GetBlobAsStream(Constants.ImageContainer, blobName);
                        changedImageStream = _imageService.RotateClockwise(imageStream);
                        await _blobStorageService.UploadStreamToBlob(Constants.ClockwiseImageContainer, blobName, changedImageStream);
                        break;
                    case "antirotate":
                        imageStream = await _blobStorageService.GetBlobAsStream(Constants.ImageContainer, blobName);
                        changedImageStream = _imageService.RotateClockwise(imageStream);
                        await _blobStorageService.UploadStreamToBlob(Constants.AntiClockwiseImageContainer, blobName, changedImageStream);
                        break;
                    case "greyscale":
                        imageStream = await _blobStorageService.GetBlobAsStream(Constants.ImageContainer, blobName);
                        changedImageStream = _imageService.GreyScale(imageStream);
                        await _blobStorageService.UploadStreamToBlob(Constants.GreyImageContainer, blobName, changedImageStream);
                        break;
                }
            }
            else
            {
                imageStream = await _blobStorageService.GetBlobAsStream(Constants.ImageContainer, blobName);
                changedImageStream = _imageService.FlipHorizontal(imageStream);

                await _blobStorageService.UploadStreamToBlob(Constants.MirrorImageContainer, blobName, changedImageStream);
            }
            return req.CreateResponse(HttpStatusCode.OK);
        }

    }
}
