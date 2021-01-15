using CaffeMenuBot.AppHost.Authentication;
using CaffeMenuBot.Data;
using CaffeMenuBot.Data.Models.Authentication;
using System.Linq;

namespace CaffeMenuBot.AppHost.Configuration
{
    public static class DatabasePreparer
    {
        public static void SeedDatabase(CaffeMenuBotContext context)
        {
            if (AdminUserExistsAsync(context)) return;

            SeedApplicationUsers(context);

            context.SaveChanges();
        }

        private static void SeedApplicationUsers(CaffeMenuBotContext context)
        {
            const string adminUserSalt = "VGhlIHF1aWNrIGJyb3duIGZveCBqdW1wcyBvdmVyIHRoZSBsYXp5IGRvZw==";

            ApplicationUser adminUser = new ApplicationUser()
            {
                Username = "admin",
                Email = "admin@caffemenubot.com",
                Role = "admin",
                Salt = adminUserSalt,
                PasswordHash = EncryptionProvider.Encrypt("admin", EncryptionProvider.ReadSaltFromBase64(adminUserSalt))
            };

            context.ApplicationUsers.AddAsync(adminUser);
        }
        private static bool AdminUserExistsAsync(CaffeMenuBotContext context)
        {
            return context.ApplicationUsers.FirstOrDefault(user => user.Role == Roles.Admin) != null;
        }
    }
}
