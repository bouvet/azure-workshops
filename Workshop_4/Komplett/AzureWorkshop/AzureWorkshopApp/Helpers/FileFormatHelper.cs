﻿using Microsoft.AspNetCore.Http;
using System;
using System.Linq;

namespace AzureWorkshopApp.Helpers
{
    public static class FileFormatHelper
    {

        public static bool IsImage(IFormFile file)
        {
            if (file.ContentType.Contains("image"))
            {
                return true;
            }

            string[] formats = { ".jpg", ".png", ".gif", ".jpeg" };

            return formats.Any(item => file.FileName.EndsWith(item, StringComparison.OrdinalIgnoreCase));
        }
    }
}
