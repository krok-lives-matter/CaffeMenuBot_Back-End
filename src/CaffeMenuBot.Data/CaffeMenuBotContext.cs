using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using CaffeMenuBot.Data.Models.Menu;
using CaffeMenuBot.Data.Models.Bot;
using CaffeMenuBot.Data.Models.Reviews;
using CaffeMenuBot.Data.Models.Schedule;
using Npgsql;
using CaffeMenuBot.Data.Models.Dashboard;
using Microsoft.AspNetCore.Identity;

namespace CaffeMenuBot.Data
{
    public sealed class CaffeMenuBotContext :
        IdentityDbContext
        <
            DashboardUser, IdentityRole, string, IdentityUserClaim<string>, DashboardUserRole, IdentityUserLogin<string>,
            IdentityRoleClaim<string>, IdentityUserToken<string>
        >
    {
        public const string SchemaName = "public";

        static CaffeMenuBotContext()
        {
            NpgsqlConnection.GlobalTypeMapper.MapEnum<ChatState>();
            NpgsqlConnection.GlobalTypeMapper.MapEnum<Rating>();
        }
        public CaffeMenuBotContext(DbContextOptions<CaffeMenuBotContext> options) : base(options)
        {
        }
        public DbSet<BotUser> BotUsers { get; set; } = null!;
        public DbSet<Review> Reviews { get; set; } = null!;
        public DbSet<Schedule> Schedule { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;

        public DbSet<Dish> Dishes { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.HasPostgresEnum<ChatState>();
            builder.HasPostgresEnum<Rating>();

            builder.Entity<DashboardUser>().HasMany(u => u.Roles).WithOne().HasForeignKey(r => r.UserId)
                .IsRequired().OnDelete(DeleteBehavior.Cascade);

            builder.Entity<DashboardUserRole>()
                   .HasOne(e => e.Role)
                   .WithMany()
                   .HasForeignKey(e => e.RoleId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}