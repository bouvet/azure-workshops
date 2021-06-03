using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AzureWorkshopApp.Helpers;
using AzureWorkshopApp.Services;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AzureWorkshopApp.Controllers
{
    // TODO: Legg til attributt for at bruker må være innlogget for å aksessere controller
    // [Authorize] 
    [Route("api/[controller]")]
    public class ImagesController : Controller
    {
        private readonly IStorageService _storageService;
        private readonly TelemetryClient _telemetryClient;

        public ImagesController(IStorageService storageService, TelemetryClient telemetryClient)
        {
            _storageService = storageService ?? throw new ArgumentNullException(nameof(storageService));
            _telemetryClient = telemetryClient;
        }


        // TODO: Legg til attributt for at bruker må være innlogget og ha rollen Uploader for å laste oppe bilde.
        // [Authorize(Roles = "Uploader")]
        // POST /api/images
        [HttpPost("[action]")]
        public async Task<IActionResult> Upload(ICollection<IFormFile> files)
        {

            var configValidation = _storageService.ValidateConfiguration();

            if (!configValidation.IsValid())
                return BadRequest(configValidation.GetErrors());

            if (files.Count == 0)
                return BadRequest("No files received from the upload");

            foreach (var formFile in files)
            {
                if (!FileFormatHelper.IsImage(formFile))
                {
                    return new UnsupportedMediaTypeResult();
                }
                if (formFile.Length <= 0)
                {
                    continue;
                }

                _telemetryClient.TrackEvent("UPLOADED_FILE", new Dictionary<string, string>
                {
                    { "FILE_NAME", formFile.FileName},
                    { "CONTENT_LENGTH", formFile.Length.ToString()}
                });

                using (var stream = formFile.OpenReadStream())
                {
                    if (await _storageService.UploadFileToStorage(stream, formFile.FileName))
                    {
                        return new AcceptedResult();
                    }
                }
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