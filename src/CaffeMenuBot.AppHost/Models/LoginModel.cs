using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace CaffeMenuBot.AppHost.Models
{
    public sealed class LoginModel
    {
        [Required]
        [StringLength(64, MinimumLength = 4)]
        public string Username { get; set; } = null!;

        [Required]
        [StringLength(64, MinimumLength = 12)]
        public string Password { get; set; } = null!;
    }
}
