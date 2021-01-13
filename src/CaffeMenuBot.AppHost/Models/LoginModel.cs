using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace CaffeMenuBot.AppHost.Models
{
    public sealed record LoginModel
    {
        [Required]
        [StringLength(64, MinimumLength = 4)]
        [EmailAddress]
        public string Email { get; init; } = null!;

        [Required]
        [StringLength(64, MinimumLength = 8)]
        public string Password { get; init; } = null!;
    }
}
