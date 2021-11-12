using AzureWorkshopFunctionApp.Interfaces;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading.Tasks;

namespace AzureWorkshopFunctionApp.Functions
{
    public class MirrorBlobTrigger
    {
        private IImageService ImageService { get; }

        public MirrorBlobTrigger(IImageService imageService)
        {
            ImageService = imageService;
        }

        [FunctionName("BlobTriggerMirror")]
        public async Task Run(
            [BlobTrigger(Constants.ImageContainer + "/{name}")] Stream myBlob,
            [Blob(Constants.MirrorImageContainer + "/{name}", FileAccess.Write)] Stream mirror, ILogger log)
        {
            myBlob.Position = 0;
            var flipHorizontal = ImageService.FlipHorizontal(myBlob);
            await flipHorizontal.CopyToAsync(mirror);
        }
    }
}
