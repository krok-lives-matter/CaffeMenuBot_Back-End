using System;
using CaffeMenuBot.AppHost.Authentication;
using CaffeMenuBot.Data;
using CaffeMenuBot.Data.Models.Authentication;

namespace CaffeMenuBot.IntegrationTests
{
    internal static class Utilities
    {
        public static void InitializeDatabaseForTests(CaffeMenuBotContext context, IServiceProvider services)
        {
            const string salt = "YWRtaW5AY2FmZmVtZW51Ym90LmNvbQ==";
            var user = new ApplicationUser
            {
                Id = 1,
                Email = "test@example.com",
                Role = "admin",
                Username = "my_username",
                Salt = salt,
                PasswordHash = EncryptionProvider.Encrypt("my_password", EncryptionProvider.ReadSaltFromBase64(salt))
            };
            context.ApplicationUsers.Add(user);
            context.SaveChanges();
        }
    }
}