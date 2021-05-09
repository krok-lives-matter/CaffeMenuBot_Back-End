
using System.ComponentModel.DataAnnotations;

namespace CaffeMenuBot.AppHost.Model.DTO.Requests
{
    public record UserLoginRequest
    {
        [Required]
        public string Email { get; init; } = null!;
        [Required]
        public string Password { get; init; } = null!;
    }

}