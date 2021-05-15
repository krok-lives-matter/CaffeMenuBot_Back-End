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

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // any guid
            const string ADMIN_ID = "a18be9c0-aa65-4af8-bd17-00bd9344e575";
            // any guid, but nothing is against to use the same one
            const string ROLE_ID = ADMIN_ID;
            builder.Entity<IdentityRole>().HasData(new IdentityRole
            {
                Id = ROLE_ID,
                Name = "admin",
                NormalizedName = "ADMIN"
            });

            var hasher = new PasswordHasher<IdentityUser>();
            builder.Entity<IdentityUser>().HasData(new IdentityUser
            {
                Id = ADMIN_ID,
                UserName = "admin",
                NormalizedUserName = "ADMIN",
                Email = "admin@caffemenubot.com",
                NormalizedEmail = "ADMIN@CAFFEMENUBOT.COM",
                EmailConfirmed = true,
                PasswordHash = hasher.HashPassword(null!, "_Change$ThisPlease3"),
                SecurityStamp = string.Empty
            });

            builder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string>
            {
                RoleId = ROLE_ID,
                UserId = ADMIN_ID
            });
        }
    }
}