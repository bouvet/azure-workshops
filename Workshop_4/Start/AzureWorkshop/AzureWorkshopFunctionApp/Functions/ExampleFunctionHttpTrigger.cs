using AzureWorkshopFunctionApp.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace AzureWorkshopFunctionApp.Functions
{
    public class ExampleFunctionHttpTrigger
    {
        private IBlobStorageService BlobStorageService { get; set; }
        private IImageService ImageService { get; set; }

        public ExampleFunctionHttpTrigger(IImageService imageService, IBlobStorageService blobStorageService)
        {
            BlobStorageService = blobStorageService;
            ImageService = imageService;
        }

        [FunctionName("ExampleFunctionHttpTrigger")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            var blobName = req.Query["blobName"];
            
            var stream = await BlobStorageService.GetBlobAsStream(Constants.ImageContainer, blobName);
            var mirror = ImageService.FlipHorizontal(stream);

            await BlobStorageService.UploadStreamToBlob(Constants.MirrorImageContainer, blobName, mirror);
            
            return new OkResult();
        }

    }
}
