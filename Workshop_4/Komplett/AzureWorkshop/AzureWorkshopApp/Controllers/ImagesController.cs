using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AzureWorkshopApp.Helpers;
using AzureWorkshopApp.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AzureWorkshopApp.Controllers
{
    [Route("api/[controller]")]
    public class ImagesController : Controller
    {
        private readonly IStorageService _storageService;
        private readonly IQueueService _queueService;

        public ImagesController(IStorageService storageService, IQueueService queueService)
        {
            _storageService = storageService ?? throw new ArgumentNullException(nameof(storageService));
            _queueService = queueService ?? throw new ArgumentNullException(nameof(queueService));
        }

        // POST /api/images/upload
        [HttpPost("[action]")]
        public async Task<IActionResult> Upload(ICollection<IFormFile> files)
        {
            var configValidation = _storageService.ValidateConfiguration();
            if (!configValidation.IsValid()) return BadRequest(configValidation.GetErrors());

            if (files.Count == 0)
                return BadRequest("No files received from the upload");

            foreach (var formFile in files)
                if (FileFormatHelper.IsImage(formFile))
                {
                    if (formFile.Length > 0)
                        using (var stream = formFile.OpenReadStream())
                        {
                            if (await _storageService.UploadFileToStorage(stream, formFile.FileName))
                            {
                                //Send message on queue
                                //Make sure to match up the queueName with a trigger and the message body with how
                                //your function reads the message. E.g.:
                                await _queueService.SendQueueMessage("greyimage", formFile.FileName);
                                await _queueService.SendQueueMessage("squareimage", formFile.FileName);
                                return new AcceptedResult();
                            }
                        }
                }
                else
                {
                    return new UnsupportedMediaTypeResult();
                }

            return BadRequest("Look like the image couldnt upload to the storage");
        }

        // GET /api/images
        [HttpGet]
        public async Task<IActionResult> GetImages()
        {
            var container = Request.Query["container"].Count > 0 ? Request.Query["container"][0] : null;
            var configValidation = _storageService.ValidateConfiguration();
            if (!configValidation.IsValid()) return BadRequest(configValidation.GetErrors());

            var imageUrls = await _storageService.GetImageUrls(container);

            return new ObjectResult(imageUrls);
        }
    }
}