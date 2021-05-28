using System;
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
        /// <summary>
        /// Seeds database with basic data and creates root admin user
        /// </summary>
        /// <param name="context">database context</param>
        /// <param name="userManager">user manager</param>
        /// <param name="roleManager">role manager</param>
        /// <param name="interactiveAdminCreation">default is false, set to true to create admin with shell interaction</param>
        internal static async Task SeedDatabaseAsync(CaffeMenuBotContext context,
            UserManager<DashboardUser> userManager,
            RoleManager<IdentityRole> roleManager,
            bool interactiveAdminCreation = false)
        {
            await CreateManagingRoles(context, userManager, roleManager);
            await SeedAdminUserAsync(context, userManager, roleManager, interactiveAdminCreation);
            await SeedDashboardDataAsync(context);
            await SeedScheduleDataAsync(context);
        }

        private static async Task SeedAdminUserAsync
        (
           CaffeMenuBotContext context,
           UserManager<DashboardUser> userManager,
           RoleManager<IdentityRole> roleManager,
           bool interactiveAdminCreation)
        {
            if(interactiveAdminCreation)
                await CreateRootAdminInteractiveAsync(context, userManager, roleManager);
            else
                await CreateRootAdminAutoAsync(context, userManager, roleManager);
        }

        private static async Task CreateRootAdminAutoAsync
        (
            CaffeMenuBotContext context,
            UserManager<DashboardUser> userManager,
            RoleManager<IdentityRole> roleManager
        )
        {
            var rootRole = new IdentityRole("root");

            if (!context.Roles.Any(r => r.Name == "root"))
            {
                await roleManager.CreateAsync(rootRole);
            }

            if (!context.Users.Any(u => u.UserName == "root"))
            {
                var rootAdmin = new DashboardUser
                {
                    UserName = "root",
                    Email = "root@caffemenubot.com",
                    ProfilePhotoFileName = "blank.jpg"
                };
                var result = await userManager.CreateAsync(rootAdmin, "_Change$ThisPlease3");
                await userManager.AddToRoleAsync(rootAdmin, rootRole.Name);
            }
        }

        private static async Task CreateRootAdminInteractiveAsync
        (
            CaffeMenuBotContext context,
            UserManager<DashboardUser> userManager,
            RoleManager<IdentityRole> roleManager
        )
        {
            throw new NotImplementedException();
        }

        private static async Task CreateManagingRoles
        (
            CaffeMenuBotContext context,
            UserManager<DashboardUser> userManager,
            RoleManager<IdentityRole> roleManager
        )
        {
            var adminRole = new IdentityRole("admin");

            if (!context.Roles.Any(r => r.Name == "admin"))
            {
                await roleManager.CreateAsync(adminRole);
            }
        }

        private static async Task SeedScheduleDataAsync(CaffeMenuBotContext context)
        {
            if (context.Schedule.Any())
                return;

            context.Schedule.AddRange(
                new Schedule
                {
                    WeekdayName = "Monday",
                    OpenTime = "8:00",
                    CloseTime = "20:00",
                    OrderIndex = 1
                },
                new Schedule
                {
                    WeekdayName = "Tuesday",
                    OpenTime = "8:00",
                    CloseTime = "20:00",
                    OrderIndex = 2
                },
                new Schedule
                {
                    WeekdayName = "Wednesday",
                    OpenTime = "8:00",
                    CloseTime = "20:00",
                    OrderIndex = 3
                },
                new Schedule
                {
                    WeekdayName = "Thursday",
                    OpenTime = "8:00",
                    CloseTime = "20:00",
                    OrderIndex = 4
                },
                new Schedule
                {
                    WeekdayName = "Friday",
                    OpenTime = "10:00",
                    CloseTime = "22:00",
                    OrderIndex = 5
                },
                new Schedule
                {
                    WeekdayName = "Saturday",
                    OpenTime = "10:00",
                    CloseTime = "22:00",
                    OrderIndex = 6
                },
                new Schedule
                {
                    WeekdayName = "Sunday",
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
                    CategoryName = "Coffee",
                    IsVisible = true,
                    Dishes = new List<Dish>
                    {
                        new()
                        {
                            DishName = "Late",
                            Description = "",
                            Price = 5m,
                            Serving = "300ml"
                        },
                        new()
                        {
                            DishName = "Americano",
                            Description = "",
                            Price = 6.5m,
                            Serving = "100ml."
                        }
                    }
                },
                new Category
                {
                    CategoryName = "Salads",
                    IsVisible = true,
                    Dishes = new List<Dish>
                    {
                        new()
                        {
                            DishName = "Greek Salad",
                            Description = "Fresh and tasty",
                            Price = 20m,
                            Serving = "300gr."
                        }
                    }
                },
                new Category
                {
                    CategoryName = "Burgers",
                    IsVisible = true,
                    Dishes = new List<Dish>
                    {
                        new()
                        {
                            DishName = "Chiken King",
                            Description = "Great start of a day",
                            Price = 5m,
                            Serving = "200gr."
                        }
                    }
                });
            await context.SaveChangesAsync();
        }
    }
}