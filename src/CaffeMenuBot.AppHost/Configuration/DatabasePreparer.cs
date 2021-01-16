using System.Collections.Generic;
using CaffeMenuBot.AppHost.Authentication;
using CaffeMenuBot.Data;
using CaffeMenuBot.Data.Models.Authentication;
using System.Linq;
using CaffeMenuBot.Data.Models.Menu;

namespace CaffeMenuBot.AppHost.Configuration
{
    public static class DatabasePreparer
    {
        public static void SeedDatabase(CaffeMenuBotContext context, bool forceReseed = false)
        {
            var adminUser = AdminUserExists(context);

            if (adminUser != null && forceReseed == false) return;

            SeedAdminUser(context, adminUser, forceReseed);
            SeedDashboardData(context);
        }

        private static void SeedAdminUser(CaffeMenuBotContext context, ApplicationUser? adminUser, bool forceReseed = false)
        {
            if (forceReseed && adminUser != null)
                context.ApplicationUsers.Remove(adminUser);

            const string adminUserSalt = "YWRtaW5AY2FmZmVtZW51Ym90LmNvbQ==";

            adminUser = new ApplicationUser()
            {
                Username = "admin",
                Email = "admin@caffemenubot.com",
                Role = "admin",
                Salt = adminUserSalt,
                PasswordHash = EncryptionProvider.Encrypt("adminadmin",
                    EncryptionProvider.ReadSaltFromBase64(adminUserSalt))
            };

            context.ApplicationUsers.Add(adminUser);
            context.SaveChanges();
        }

        private static ApplicationUser? AdminUserExists(CaffeMenuBotContext context)

        {
            return context.ApplicationUsers.FirstOrDefault(user => user.Role == Roles.Admin);
        }

        private static void SeedDashboardData(CaffeMenuBotContext context)
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
            context.SaveChanges();
        }
    }
}
