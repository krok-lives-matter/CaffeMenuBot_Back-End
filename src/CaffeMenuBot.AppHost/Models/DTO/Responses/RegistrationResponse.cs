using CaffeMenuBot.AppHost.Domain;
using Microsoft.AspNetCore.Identity;

namespace CaffeMenuBot.AppHost.Model.DTO.Responses
{
    public class RegistrationResponse : AuthResult
    {
        public IdentityUser User {get;set;} = null!;
    }
}