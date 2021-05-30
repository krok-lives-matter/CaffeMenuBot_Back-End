using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace CaffeMenuBot.AppHost.Models.DTO.Requests
{
    public sealed record SetProfilePhotoRequest
    {
        [FromForm(Name = "file"), SwaggerParameter("Image binary file sent via FormData", Required = true)]
        [Required]
        public IFormFile File { get; init; } = null!;
        [FromForm(Name = "userId"), SwaggerParameter("User id what this image belongs to", Required = true)]
        [Required]
        public string UserId { get; init; } = null!;
    }
}