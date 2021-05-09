using System.Collections.Generic;
namespace CaffeMenuBot.AppHost.Domain
{
    public class AuthResult
    {
        public string Token { get; init; } = null!;
        public bool Result { get; init; }
        public List<string> Errors { get; set; } = null!;
    }

}