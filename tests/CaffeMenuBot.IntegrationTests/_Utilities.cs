using System.Diagnostics;
using System;
using System.Linq;
using CaffeMenuBot.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace CaffeMenuBot.IntegrationTests
{
    internal static class Utilities
    {
        public static void InitializeDatabaseForTests(CaffeMenuBotContext context, IServiceProvider services)
        {
            UserManager<IdentityUser> userManager = services.GetRequiredService<UserManager<IdentityUser>>();
            RoleManager<IdentityRole> roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            Debug.WriteLine(roleManager);
            Debug.WriteLine(userManager);
            SeedAdminUser(context, userManager, roleManager);
        }

        private static void SeedAdminUser(
            CaffeMenuBotContext context,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            var adminRole = new IdentityRole("admin");

            if (!context.Roles.Any(r => r.Name == adminRole.Name))
            {
                roleManager.CreateAsync(adminRole).GetAwaiter().GetResult();
            }

            if (!context.Users.Any(u => u.UserName == "admin"))
            {
                var adminUser = new IdentityUser
                {
                    UserName = "admin",
                    Email = "admin@caffemenubot.com"
                };
                var result = userManager.CreateAsync(adminUser, "_Change$ThisPlease3").GetAwaiter().GetResult();
                userManager.AddToRoleAsync(adminUser, adminRole.Name).GetAwaiter().GetResult();
            }
        }
    }
}