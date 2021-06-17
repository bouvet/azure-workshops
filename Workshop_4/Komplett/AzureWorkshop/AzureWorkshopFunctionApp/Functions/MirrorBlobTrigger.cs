using AzureWorkshopFunctionApp.Interfaces;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;

namespace AzureWorkshopFunctionApp.Functions
{
    public class MirrorBlobTrigger
    {
        private IImageService ImageService { get; set; }

        public MirrorBlobTrigger(IImageService imageService)
        {
            ImageService = imageService;
        }

        [FunctionName("BlobTriggerMirror")]
        public async Task Run(
            [BlobTrigger(Constants.ImageContainer + "/{name}", Connection = Constants.ConnectionString)] Stream myBlob,
            [Blob(Constants.MirrorImageContainer + "/{name}", FileAccess.Write, Connection = Constants.ConnectionString)] Stream mirror, ILogger log)
        {
            myBlob.Position = 0;
            var flipHorizontal = ImageService.FlipHorizontal(myBlob, ImageFormat.Jpeg);
            await flipHorizontal.CopyToAsync(mirror);
        }
    }
}
