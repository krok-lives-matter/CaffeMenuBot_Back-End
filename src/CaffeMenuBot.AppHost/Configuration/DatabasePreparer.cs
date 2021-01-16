using CaffeMenuBot.AppHost.Authentication;
using CaffeMenuBot.Data;
using CaffeMenuBot.Data.Models.Authentication;
using System.Linq;

namespace CaffeMenuBot.AppHost.Configuration
{
    public static class DatabasePreparer
    {
        public static void SeedDatabase(CaffeMenuBotContext context, bool forceReseed = false)
        {
            var adminUser = AdminUserExists(context);

            if (adminUser != null && forceReseed == false) return;

            SeedAdminUser(context, adminUser, forceReseed);

            context.SaveChanges();
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
        }

        private static ApplicationUser? AdminUserExists(CaffeMenuBotContext context)

        {
            return context.ApplicationUsers.FirstOrDefault(user => user.Role == Roles.Admin);
        }
    }
}
