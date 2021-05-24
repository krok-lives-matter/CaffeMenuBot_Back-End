using CaffeMenuBot.AppHost.Models.DTO.Requests;
using Microsoft.AspNetCore.Hosting;
using System;
using System.IO;

namespace CaffeMenuBot.AppHost.Helpers
{
    public static class ImageHelper
    {
        /// <summary>
        /// saves base64 image to desired path inside media folder
        /// </summary>
        /// <param name="webHostEnvironment">to get app root folder</param>
        /// <param name="mediaSubfolder">subfolder inside media folder</param>
        /// <returns>file name of saved image</returns>
        public static string SaveImage
            (
            ImageModel image,
            IWebHostEnvironment webHostEnvironment,
            string mediaSubfolder
            )
        {
            string uniqueFileName = Guid.NewGuid().ToString() + image.ContentType;

            string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, $"media/{mediaSubfolder}");
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            File.WriteAllBytes(filePath, Convert.FromBase64String(image.Base64EncodedImage));

            return uniqueFileName;
        }      
    }
}
