using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using CaffeMenuBot.Data.Models.Menu;
using CaffeMenuBot.Data.Models.Bot;
using CaffeMenuBot.Data.Models.Reviews;
using CaffeMenuBot.Data.Models.Schedule;

namespace CaffeMenuBot.Data
{
    public sealed class CaffeMenuBotContext : IdentityDbContext
    {
        public const string SchemaName = "public";
        
        public CaffeMenuBotContext(DbContextOptions<CaffeMenuBotContext> options) : base(options)
        {
        }
        public DbSet<BotUser> BotUsers { get; set; } = null!;
        public DbSet<Review> Reviews {get; set; } = null!;
        public DbSet<Schedule> Schedule { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;

        public DbSet<Dish> Dishes { get; set; } = null!;
    }
}