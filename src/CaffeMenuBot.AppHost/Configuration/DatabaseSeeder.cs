using System.Threading.Tasks;
using System.Linq;
using CaffeMenuBot.Data;
using CaffeMenuBot.Data.Models.Menu;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using CaffeMenuBot.Data.Models.Schedule;
using System;

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
            await SeedScheduleDataAsync(context);
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

        private static async Task SeedScheduleDataAsync(CaffeMenuBotContext context)
        {
            if (context.Schedule.Any())
                return;
            
            context.Schedule.AddRange(
                new Schedule
                {
                    WeekdayName = "Понеділок",
                    OpenTime = "8:00",
                    CloseTime = "20:00"
                },
                new Schedule
                {
                    WeekdayName = "Вівторок",
                    OpenTime = "8:00",
                    CloseTime = "20:00"
                },
                new Schedule
                {
                    WeekdayName = "Середа",
                    OpenTime = "8:00",
                    CloseTime = "20:00"
                },
                new Schedule
                {
                    WeekdayName = "Четвер",
                    OpenTime = "8:00",
                    CloseTime = "20:00"
                },
                new Schedule
                {
                    WeekdayName = "П'ятниця",
                    OpenTime = "10:00",
                    CloseTime = "22:00"
                },
                new Schedule
                {
                    WeekdayName = "Субота",
                    OpenTime = "10:00",
                    CloseTime = "22:00"
                },
                new Schedule
                {
                    WeekdayName = "Неділя",
                    OpenTime = "9:00",
                    CloseTime = "19:00"
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