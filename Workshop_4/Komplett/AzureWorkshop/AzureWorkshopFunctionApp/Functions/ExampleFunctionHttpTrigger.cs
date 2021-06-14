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
            Stream stream;
            Stream mirror;

            if (func.Count != 0)
            {
                switch (func[0])
                {
                    case "mirror":
                        stream = await BlobStorageService.GetBlobAsStream("imagecontainer", blobName);
                        mirror = ImageService.FlipHorizontal(stream, ImageFormat.Jpeg);
                        await BlobStorageService.UploadStreamToBlob("imagecontainer-mirror", blobName, mirror);
                        break;
                    case "flip":
                        stream = await BlobStorageService.GetBlobAsStream("imagecontainer", blobName);
                        mirror = ImageService.FlipVertical(stream, ImageFormat.Jpeg);
                        await BlobStorageService.UploadStreamToBlob("imagecontainer-flipped", blobName, mirror);
                        break;
                    case "rotate":
                        stream = await BlobStorageService.GetBlobAsStream("imagecontainer", blobName);
                        mirror = ImageService.RotateClockwise(stream, ImageFormat.Jpeg);
                        await BlobStorageService.UploadStreamToBlob("imagecontainer-clockwise", blobName, mirror);
                        break;
                    case "antirotate":
                        stream = await BlobStorageService.GetBlobAsStream("imagecontainer", blobName);
                        mirror = ImageService.RotateClockwise(stream, ImageFormat.Jpeg);
                        await BlobStorageService.UploadStreamToBlob("imagecontainer-anticlockwise", blobName, mirror);
                        break;
                    case "greyscale":
                        stream = await BlobStorageService.GetBlobAsStream("imagecontainer", blobName);
                        mirror = ImageService.GreyScale(stream, ImageFormat.Jpeg);
                        await BlobStorageService.UploadStreamToBlob("imagecontainer-grey", blobName, mirror);
                        break;
                }
            }
            else
            {
                stream = await BlobStorageService.GetBlobAsStream("imagecontainer", blobName);
                mirror = ImageService.FlipHorizontal(stream, ImageFormat.Jpeg);

                await BlobStorageService.UploadStreamToBlob("imagecontainer-mirror", blobName, mirror);
            }
            return new OkResult();
        }

    }
}
