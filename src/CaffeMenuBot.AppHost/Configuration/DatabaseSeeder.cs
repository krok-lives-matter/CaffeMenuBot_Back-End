using System.Threading.Tasks;
using System.Linq;
using CaffeMenuBot.Data;
using CaffeMenuBot.Data.Models.Menu;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace CaffeMenuBot.AppHost.Configuration
{
    internal static class DatabaseSeeder
    {
        internal static async Task SeedDatabaseAsync(CaffeMenuBotContext context,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager) 
        {
            await SeedAdminUserAsync(context, userManager, roleManager);
            await SeedDashboardDataAsync(context);
        }

        private static async Task SeedAdminUserAsync(
            CaffeMenuBotContext context,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)     
        {
            var adminRole = new IdentityRole("admin");

                if (!context.Roles.Any())
                {
                    await roleManager.CreateAsync(adminRole);
                }

                if (!context.Users.Any(u => u.UserName == "admin"))
                {
                    var adminUser = new IdentityUser
                    {
                        UserName = "admin@caffemenubot.com",
                        Email = "admin@caffemenubot.com"
                    };
                    var result = await userManager.CreateAsync(adminUser, "_Change$ThisPlease3");
                    await userManager.AddToRoleAsync(adminUser, adminRole.Name);
                }
        }
        private static async Task SeedDashboardDataAsync(CaffeMenuBotContext context)
        {
            if (context.Categories.Any())
                return;
            
            context.Categories.AddRange(
                new Category
                {
                    CategoryName = "Десерти",
                    Dishes = new List<Dish>
                    {
                        new()
                        {
                            DishName = "Львівські Пляцки",
                            Description = "Дуже смачно",
                            Price = 100.5m,
                            Serving = "200гр."
                        }
                    }
                },
                new Category
                {
                    CategoryName = "М'ясо",
                    Dishes = new List<Dish>
                    {
                        new()
                        {
                            DishName = "Шашлик машлик",
                            Description = "мясо",
                            Price = 350.5m,
                            Serving = "250гр."
                        }
                    }
                });
            await context.SaveChangesAsync();
        }
    }
}