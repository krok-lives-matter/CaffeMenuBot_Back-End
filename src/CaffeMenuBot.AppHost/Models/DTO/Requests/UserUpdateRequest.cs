using Microsoft.AspNetCore.Http;

namespace CaffeMenuBot.AppHost.Models.DTO.Responses
{
    public sealed record UserUpdateRequest
    {
        public string Id { get; init; } = null!;
        public string? UserName { get; init; } = null;
        public IFormFile? ProfilePhoto { get; init; } = null;
    }
}