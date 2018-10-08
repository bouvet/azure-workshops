using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AzureWorkshopApp.Helpers;
using AzureWorkshopApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AzureWorkshopApp.Controllers
{
    [Route("api/[controller]")]
    public class ImagesController : Controller
    {
        // make sure that appsettings.json is filled with the necessary details of the azure storage
        private readonly AzureStorageConfig _storageConfig;

        public ImagesController(IOptions<AzureStorageConfig> config)
        {
            _storageConfig = config.Value;
        }

        // POST /api/images/upload
        [HttpPost("[action]")]
        public async Task<IActionResult> Upload(ICollection<IFormFile> files)
        {
            var isUploaded = false;

            try
            {
                if (files.Count == 0)

                    return BadRequest("No files received from the upload");

                if (_storageConfig.AccountKey == string.Empty || _storageConfig.AccountName == string.Empty)

                    return BadRequest("Sorry, can't retrieve your azure storage details from appsettings.js, make sure that you add azure storage details there");

                if (_storageConfig.ImageContainer == string.Empty)

                    return BadRequest("Please provide a name for your image container in the azure blob storage");

                foreach (var formFile in files)
                    if (StorageHelper.IsImage(formFile))
                    {
                        if (formFile.Length > 0)
                            using (var stream = formFile.OpenReadStream())
                            {
                                isUploaded = await StorageHelper.UploadFileToStorage(stream, formFile.FileName, _storageConfig);
                            }
                    }
                    else
                    {
                        return new UnsupportedMediaTypeResult();
                    }

                if (isUploaded)
                {
                    if (_storageConfig.ImageContainer != string.Empty)

                        return new AcceptedAtActionResult("GetImages", "Images", null, null);

                    return new AcceptedResult();
                }

                return BadRequest("Look like the image couldnt upload to the storage");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET /api/images
        [HttpGet]
        public async Task<IActionResult> GetImages()
        {
            try
            {
                if (_storageConfig.AccountKey == string.Empty || _storageConfig.AccountName == string.Empty)
                {
                    return BadRequest("Sorry, can't retrieve your azure storage details from appsettings.js, make sure that you add azure storage details there");
                }

                if (_storageConfig.ImageContainer == string.Empty)
                {
                    return BadRequest("Please provide a name for your image container in the azure blob storage");
                }

                var imageUrls = await StorageHelper.GetImageUrls(_storageConfig);

                return new ObjectResult(imageUrls);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}