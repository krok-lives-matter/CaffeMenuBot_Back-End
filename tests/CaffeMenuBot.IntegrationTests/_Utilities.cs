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
            SeedTestUser(context, userManager, roleManager);
        }

        private static void SeedTestUser(
            CaffeMenuBotContext context,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            var testRole = new IdentityRole("test");

            if (!context.Roles.Any(r => r.Name == testRole.Name))
            {
                roleManager.CreateAsync(testRole).GetAwaiter().GetResult();
            }

            if (!context.Users.Any(u => u.UserName == "test"))
            {
                var testUser = new IdentityUser
                {
                    UserName = "test",
                    Email = "test@caffemenubot.com"
                };
                var result = userManager.CreateAsync(testUser, "_Change$ThisPlease3").GetAwaiter().GetResult();
                userManager.AddToRoleAsync(testUser, testRole.Name).GetAwaiter().GetResult();
            }
        }
    }
}