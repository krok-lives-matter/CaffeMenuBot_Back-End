using System.Runtime.CompilerServices;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace CaffeMenuBot.AppHost.Models.DTO.Requests
{
    public sealed record ImageModel
    {
        [Required]
        public string ContentType { get; init; } = null!;
        [Required]
        public string FileExtension { get; init; } = null!;
        [Required]
        public Stream ImageStream { get; init; } = null!;
    }
}
