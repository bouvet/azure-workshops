using AzureWorkshopFunctionApp.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;

namespace AzureWorkshopFunctionApp.Functions
{
    public class ExampleFunctionHttpTrigger
    {
        private IBlobStorageService BlobStorageService { get; set; }
        private IImageService ImageService { get; set; }

        public ExampleFunctionHttpTrigger(IImageService imageService, IBlobStorageService blobStorageService)
        {
            BlobStorageService = blobStorageService; // new BlobStorageService(ConfigurationManager.ConnectionStrings["AzureWebJobsStorage"].ConnectionString);
            ImageService = imageService;
        }

        [FunctionName("ExampleFunctionHttpTrigger")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            var blobName = req.Query["blobName"];
            var func = req.Query["func"];
            Stream imageStream;
            Stream changedImageStream;

            if (func.Count != 0)
            {
                switch (func[0])
                {
                    case "mirror":
                        imageStream = await BlobStorageService.GetBlobAsStream(Constants.ImageContainer, blobName);
                        changedImageStream = ImageService.FlipHorizontal(imageStream, ImageFormat.Jpeg);
                        await BlobStorageService.UploadStreamToBlob(Constants.MirrorImageContainer, blobName, changedImageStream);
                        break;
                    case "flip":
                        imageStream = await BlobStorageService.GetBlobAsStream(Constants.ImageContainer, blobName);
                        changedImageStream = ImageService.FlipVertical(imageStream, ImageFormat.Jpeg);
                        await BlobStorageService.UploadStreamToBlob(Constants.FlippedImageContainer, blobName, changedImageStream);
                        break;
                    case "rotate":
                        imageStream = await BlobStorageService.GetBlobAsStream(Constants.ImageContainer, blobName);
                        changedImageStream = ImageService.RotateClockwise(imageStream, ImageFormat.Jpeg);
                        await BlobStorageService.UploadStreamToBlob(Constants.ClockwiseImageContainer, blobName, changedImageStream);
                        break;
                    case "antirotate":
                        imageStream = await BlobStorageService.GetBlobAsStream(Constants.ImageContainer, blobName);
                        changedImageStream = ImageService.RotateClockwise(imageStream, ImageFormat.Jpeg);
                        await BlobStorageService.UploadStreamToBlob(Constants.AntiClockwiseImageContainer, blobName, changedImageStream);
                        break;
                    case "greyscale":
                        imageStream = await BlobStorageService.GetBlobAsStream(Constants.ImageContainer, blobName);
                        changedImageStream = ImageService.GreyScale(imageStream, ImageFormat.Jpeg);
                        await BlobStorageService.UploadStreamToBlob(Constants.GreyImageContainer, blobName, changedImageStream);
                        break;
                }
            }
            else
            {
                imageStream = await BlobStorageService.GetBlobAsStream(Constants.ImageContainer, blobName);
                changedImageStream = ImageService.FlipHorizontal(imageStream, ImageFormat.Jpeg);

                await BlobStorageService.UploadStreamToBlob(Constants.MirrorImageContainer, blobName, changedImageStream);
            }
            return new OkResult();
        }

    }
}
