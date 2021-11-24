using AzureWorkshopFunctionApp.Interfaces;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace AzureWorkshopFunctionApp.Functions
{
    public class RotateTimerTrigger
    {
        private  IBlobStorageService BlobStorageService { get; }
        private  IImageService ImageService { get; }
        public RotateTimerTrigger(IBlobStorageService blobStorageService, IImageService imageService)
        {
            BlobStorageService = blobStorageService;
            ImageService = imageService;
        }

        [FunctionName("RotateTimerTrigger")]
        public async Task Run([TimerTrigger("0 */2 * * * *")]TimerInfo myTimer, ILogger log)
        {
            var blobNames = await BlobStorageService.GetFileNamesInContainer(Constants.ImageContainer);
            foreach(var blobName in blobNames)
            {
                log.LogDebug($"Processing blob {blobName}");
                var blobStream = await BlobStorageService.GetBlobAsStream(Constants.ImageContainer, blobName);
                var rotateRight = ImageService.RotateClockwise(blobStream);
                await BlobStorageService.UploadStreamToBlob(Constants.ClockwiseImageContainer, blobName, rotateRight);

                blobStream.Position = 0;
                var rotateLeft = ImageService.RotateAntiClockwise(blobStream);
                await BlobStorageService.UploadStreamToBlob(Constants.AntiClockwiseImageContainer, blobName, rotateLeft);
                log.LogDebug($"Done processing blob {blobName}");
            }
        }
    }
}
