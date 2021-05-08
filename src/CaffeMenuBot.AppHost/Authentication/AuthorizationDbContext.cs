using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CaffeMenuBot.AppHost.Authentication
{
    public sealed class AuthorizationDbContext : IdentityDbContext
    {
        public AuthorizationDbContext(DbContextOptions options) : base(options)
        {
        }
    }
}