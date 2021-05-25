using System.Threading.Tasks;
using System.Linq;
using CaffeMenuBot.Data;
using CaffeMenuBot.Data.Models.Menu;
using System.Collections.Generic;
using CaffeMenuBot.Data.Models.Schedule;
using Microsoft.AspNetCore.Identity;
using CaffeMenuBot.Data.Models.Dashboard;

namespace CaffeMenuBot.AppHost.Configuration
{
    internal static class DatabaseSeeder
    {
        internal static async Task SeedDatabaseAsync(CaffeMenuBotContext context,
            UserManager<DashboardUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            await SeedAdminUserAsync(context, userManager, roleManager);
            await SeedDashboardDataAsync(context);
            await SeedScheduleDataAsync(context);
        }

        private static async Task SeedAdminUserAsync(
           CaffeMenuBotContext context,
           UserManager<DashboardUser> userManager,
           RoleManager<IdentityRole> roleManager)
        {
            var adminRole = new IdentityRole("admin");

            if (!context.Roles.Any(r => r.Name == "admin"))
            {
                await roleManager.CreateAsync(adminRole);
            }

            if (!context.Users.Any(u => u.UserName == "admin"))
            {
                var adminUser = new DashboardUser
                {
                    UserName = "admin",
                    Email = "admin@caffemenubot.com",
                    ProfilePhotoFileName = "blank.jpg"
                };
                var result = await userManager.CreateAsync(adminUser, "_Change$ThisPlease3");
                await userManager.AddToRoleAsync(adminUser, adminRole.Name);
            }
        }

        private static async Task SeedScheduleDataAsync(CaffeMenuBotContext context)
        {
            if (context.Schedule.Any())
                return;

            context.Schedule.AddRange(
                new Schedule
                {
                    WeekdayName = "Понеділок",
                    OpenTime = "8:00",
                    CloseTime = "20:00",
                    OrderIndex = 1
                },
                new Schedule
                {
                    WeekdayName = "Вівторок",
                    OpenTime = "8:00",
                    CloseTime = "20:00",
                    OrderIndex = 2
                },
                new Schedule
                {
                    WeekdayName = "Середа",
                    OpenTime = "8:00",
                    CloseTime = "20:00",
                    OrderIndex = 3
                },
                new Schedule
                {
                    WeekdayName = "Четвер",
                    OpenTime = "8:00",
                    CloseTime = "20:00",
                    OrderIndex = 4
                },
                new Schedule
                {
                    WeekdayName = "П'ятниця",
                    OpenTime = "10:00",
                    CloseTime = "22:00",
                    OrderIndex = 5
                },
                new Schedule
                {
                    WeekdayName = "Субота",
                    OpenTime = "10:00",
                    CloseTime = "22:00",
                    OrderIndex = 6
                },
                new Schedule
                {
                    WeekdayName = "Неділя",
                    OpenTime = "9:00",
                    CloseTime = "19:00",
                    OrderIndex = 7
                }
            );
            await context.SaveChangesAsync();
        }

        private static async Task SeedDashboardDataAsync(CaffeMenuBotContext context)
        {
            if (context.Categories.Any())
                return;

            context.Categories.AddRange(
                new Category
                {
                    CategoryName = "Десерти",
                    IsVisible = true,
                    Dishes = new List<Dish>
                    {
                        new()
                        {
                            DishName = "Львівські Пляцки",
                            Description = "Дуже смачно",
                            Price = 100.5m,
                            Serving = "200гр."
                        },
                        new()
                        {
                            DishName = "Пляцки Пляцки",
                            Description = "Дуже Дуже",
                            Price = 150.5m,
                            Serving = "200гр."
                        }
                    }
                },
                new Category
                {
                    CategoryName = "М'ясо",
                    IsVisible = true,
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