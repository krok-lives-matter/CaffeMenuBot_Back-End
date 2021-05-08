using Microsoft.EntityFrameworkCore;

namespace CaffeMenuBot.AppHost.Authentication
{
    public sealed class AuthorizationDbContext : DbContext
    {
        public AuthorizationDbContext(DbContextOptions options) : base(options)
        {
        }
    }
}