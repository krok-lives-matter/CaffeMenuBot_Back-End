using Microsoft.EntityFrameworkCore;
using CaffeMenuBot.Data.Models.Menu;
using CaffeMenuBot.Data.Models.Authentication;
using CaffeMenuBot.Data.Models.Bot;

namespace CaffeMenuBot.Data
{
    public sealed class CaffeMenuBotContext : DbContext
    {
        public const string SchemaName = "public";
        
        public CaffeMenuBotContext(DbContextOptions<CaffeMenuBotContext> options) : base(options)
        {
        }
        

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<BotUser> BotUsers { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Dish> Dishes { get; set; }
    }
}