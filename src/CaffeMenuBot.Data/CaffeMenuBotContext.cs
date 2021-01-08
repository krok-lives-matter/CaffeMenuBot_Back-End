using Microsoft.EntityFrameworkCore;
using CaffeMenuBot.Data.Models;

namespace CaffeMenuBot.Data
{
    public sealed class CaffeMenuBotContext : DbContext
    {
        public CaffeMenuBotContext(DbContextOptions<CaffeMenuBotContext> options) : base(options)
        {
        }
        
        public DbSet<Category> Categories { get; set; }
        public DbSet<Dish> Dishes { get; set; }
    }
}