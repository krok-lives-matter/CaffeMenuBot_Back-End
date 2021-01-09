using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CaffeMenuBot.Data
{
    public sealed class CaffeMenuBotContextFactory : IDesignTimeDbContextFactory<CaffeMenuBotContext>
    {
        private const string ConnectionString = "Host=postgres;Port=5432;UserId=postgres;Password=5koNorJ7WVqprgESuS;Database=caffe_menu_bot;CommandTimeout=300;";
        
        public CaffeMenuBotContext CreateDbContext(string[] args)
        {
            DbContextOptionsBuilder<CaffeMenuBotContext> optionsBuilder = new();
            optionsBuilder.UseNpgsql(ConnectionString,
                b => b.EnableRetryOnFailure()
                    .MigrationsAssembly("CaffeMenuBot.Data")
                    .MigrationsHistoryTable("__MigrationHistory", CaffeMenuBotContext.SchemaName));
            return new CaffeMenuBotContext(optionsBuilder.Options);
        }
    }
}