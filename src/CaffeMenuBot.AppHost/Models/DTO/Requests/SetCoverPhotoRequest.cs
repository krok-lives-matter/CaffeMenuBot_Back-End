using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace CaffeMenuBot.AppHost.Models.DTO.Requests
{
    public sealed record SetCoverPhotoRequest
    {
        [FromForm(Name = "file"), SwaggerParameter("Image (1190x480) binary file sent via FormData", Required = true)]
        [Required]
        public IFormFile File { get; init; } = null!;
        [FromForm(Name = "categoryId"), SwaggerParameter("Category id what this image belongs to", Required = true)]
        [Range(1, int.MaxValue)]
        public int CategoryId { get; init; }
    }
}