using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using AzureWorkshopFunctionApp.Interfaces;

namespace AzureWorkshopFunctionApp.Functions
{
    public class ExampleFunctionHttpTrigger
    {
        private IBlobStorageService BlobStorageService { get; set; }

        public ExampleFunctionHttpTrigger(IBlobStorageService blobStorageService)
        {
            BlobStorageService = blobStorageService;
        }

        [FunctionName("ExampleFunctionHttpTrigger")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            var blobName = req.Query["blobName"];

            var stream = BlobStorageService.GetBlobAsStream("imagecontainer", blobName);

            return new OkResult();
        }
    }
}
