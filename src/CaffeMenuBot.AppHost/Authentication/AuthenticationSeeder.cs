using Microsoft.AspNetCore.Identity;
using System.Linq;

namespace CaffeMenuBot.AppHost.Authentication
{
    internal static class AuthenticationSeeder
    {
        internal static void SeedAdminUser(
            AuthorizationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)     
        {
            /*if (!context.Users.Any(u => u.UserName == "admin"))
            {
                var adminUser = new ApplicationUser
                {
                    UserName = "admin@caffemenubot.com",
                    Email = "admin@caffemenubot.com"
                };
                var result = userManager.CreateAsync(adminUser, "_Change$ThisPlease3)").GetAwaiter().GetResult();
                userManager.AddToRoleAsync(adminUser, adminRole.Name).GetAwaiter().GetResult();
            }*/
        }
    }
}