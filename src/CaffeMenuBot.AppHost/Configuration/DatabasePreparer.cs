using CaffeMenuBot.AppHost.Authentication;
using CaffeMenuBot.Data;
using CaffeMenuBot.Data.Models.Authentication;
using System.Linq;
using System.Threading.Tasks;

namespace CaffeMenuBot.AppHost.Configuration
{
    public static class DatabasePreparer
    {
        public async static Task SeedDatabaseAsync(CaffeMenuBotContext context)
        {
            if (AdminUserExistsAsync(context)) return;

            await SeedApplicationUsersAsync(context);

            await context.SaveChangesAsync();
        }

        private async static Task SeedApplicationUsersAsync(CaffeMenuBotContext context)
        {
            ApplicationUser adminUser = new ApplicationUser()
            {
                Username = "admin",
                Role = "admin"
            };

            await context.ApplicationUsers.AddAsync(adminUser);
        }
        private static bool AdminUserExistsAsync(CaffeMenuBotContext context)
        {
            return context.ApplicationUsers.FirstOrDefault(user => user.Role == Roles.Admin) != null;
        }
    }
}
