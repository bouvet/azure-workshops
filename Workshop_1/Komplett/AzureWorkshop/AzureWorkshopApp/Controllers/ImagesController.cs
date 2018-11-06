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

        public ImagesController(IStorageService storageService)
        {
            _storageService = storageService ?? throw new ArgumentNullException(nameof(storageService));
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
            var configValidation = _storageService.ValidateConfiguration();
            if (!configValidation.IsValid()) return BadRequest(configValidation.GetErrors());

            var imageUrls = await _storageService.GetImageUrls();

            return new ObjectResult(imageUrls);
        }
    }
}