using AzureWorkshopFunctionApp.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace AzureWorkshopFunctionApp.Functions
{
    public class RotateTimerTrigger
    {
        private readonly IBlobStorageService _blobStorageService;
        private readonly IImageService _imageService;
        private readonly ILogger<RotateTimerTrigger> _logger;
        public RotateTimerTrigger(IBlobStorageService blobStorageService, IImageService imageService, ILogger<RotateTimerTrigger> logger)
        {
            _blobStorageService = blobStorageService;
            _imageService = imageService;
            _logger = logger;
        }

        [Function("RotateTimerTrigger")]
        public async Task Run([TimerTrigger("0 */2 * * * *")]TimerInfo myTimer)
        {
            var blobNames = await _blobStorageService.GetFileNamesInContainer(Constants.ImageContainer);
            foreach(var blobName in blobNames)
            {
                _logger.LogDebug($"Processing blob {blobName}");
                var blobStream = await _blobStorageService.GetBlobAsStream(Constants.ImageContainer, blobName);
                var rotateRight = _imageService.RotateClockwise(blobStream);
                await _blobStorageService.UploadStreamToBlob(Constants.ClockwiseImageContainer, blobName, rotateRight);

                blobStream.Position = 0;
                var rotateLeft = _imageService.RotateAntiClockwise(blobStream);
                await _blobStorageService.UploadStreamToBlob(Constants.AntiClockwiseImageContainer, blobName, rotateLeft);
                _logger.LogDebug($"Done processing blob {blobName}");
            }
        }
    }
}
