using System.ComponentModel.DataAnnotations;


namespace CaffeMenuBot.AppHost.Models
{
    public sealed record LoginModel
    {
        [Required]
        [StringLength(254, MinimumLength = 4)]
        [RegularExpression("/^[a-z0-9!#$%&'*+\\/=?^_`{|}~.-]+@[a-z0-9](?:[a-z0-9-]{0,61}[a-z0-9])?(?:\\.[a-z0-9]([a-z0-9-]{0,61}[a-z0-9])?)*$/iD")]
        public string Email { get; init; } = null!;

        [Required]
        [StringLength(64, MinimumLength = 8)]
        public string Password { get; init; } = null!;
    }
}
