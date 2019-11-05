using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AzureWorkshopApp.Helpers;
using AzureWorkshopApp.Services;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AzureWorkshopApp.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class ImagesController : Controller
    {
        private readonly IStorageService _storageService;
        private readonly TelemetryClient _telemetryClient;
        private readonly IHttpContextAccessor _contextAccessor;

        public ImagesController(IStorageService storageService, TelemetryClient telemetryClient, IHttpContextAccessor contextAccessor)
        {
            _storageService = storageService ?? throw new ArgumentNullException(nameof(storageService));
            _contextAccessor = contextAccessor ?? throw new ArgumentNullException(nameof(contextAccessor));
            _telemetryClient = telemetryClient;
        }

        // POST /api/images/upload
        [HttpPost("[action]")]
        public async Task<IActionResult> Upload(ICollection<IFormFile> files)
        {
            var user = _contextAccessor.HttpContext.User;

            if (user == null)
                return BadRequest("User could not be determined.");

            var userId = User.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier").Value;

            if (userId == null) 
                return BadRequest("User could not be determined.");

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
                    if (await _storageService.UploadFileToStorage(stream, formFile.FileName, userId))
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
            var user = _contextAccessor.HttpContext.User;
            
            if (user == null)
                return BadRequest("User could not be determined.");

            var userId = User.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier").Value;

            if (userId == null)
                return BadRequest("User could not be determined.");


            var configValidation = _storageService.ValidateConfiguration();
            if (!configValidation.IsValid()) return BadRequest(configValidation.GetErrors());

            var imageUrls = await _storageService.GetImageUrls(userId);

            return new ObjectResult(imageUrls);
        }
    }
}