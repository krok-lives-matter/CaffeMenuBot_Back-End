using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CaffeMenuBot.AppHost.Authentication
{
    public sealed class AuthorizationDbContextFactory
    {
        private const string PlaceholderConnectionString = "Host=localhost;Port=5432;UserId=postgres;Password=Password;Database=caffe_menu_bot;CommandTimeout=300;";
        
        public class BloggingContextFactory : IDesignTimeDbContextFactory<AuthorizationDbContext>
        {
            public AuthorizationDbContext CreateDbContext(string[] args)
            {
                var optionsBuilder = new DbContextOptionsBuilder<AuthorizationDbContext>();
                optionsBuilder.UseNpgsql(PlaceholderConnectionString);

                return new AuthorizationDbContext(optionsBuilder.Options);
            }
        }
    }
}