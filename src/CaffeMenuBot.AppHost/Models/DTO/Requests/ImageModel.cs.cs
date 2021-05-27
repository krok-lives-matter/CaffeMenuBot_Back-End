using System.IO;

namespace CaffeMenuBot.AppHost.Models.DTO.Requests
{
    public sealed record ImageModel
    {
        public string ContentType { get; init; } = null!;
        public Stream ImageStream { get; init; } = null!;
    }
}
