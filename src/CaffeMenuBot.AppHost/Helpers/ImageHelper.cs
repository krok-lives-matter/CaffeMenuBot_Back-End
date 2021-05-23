using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;

namespace CaffeMenuBot.AppHost.Helpers
{
    public static class ImageHelper
    {
        /// <summary>
        /// saved IFormFile image to desired path inside media folder
        /// </summary>
        /// <param name="webHostEnvironment">to get app root folder</param>
        /// <param name="mediaSubfolder">subfolder inside media folder</param>
        /// <returns>file name of saved image</returns>
        public static string SaveImage
            (
            IFormFile image,
            IWebHostEnvironment webHostEnvironment,
            string mediaSubfolder
            )
        {
            string uniqueFileName = null!;

            string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, $"media/{mediaSubfolder}");
            uniqueFileName = Guid.NewGuid().ToString() + "_" + image.FileName;
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                image.CopyTo(fileStream);
            }

            return uniqueFileName;
        }
    }
}
