using CaffeMenuBot.AppHost.Configuration;
using CaffeMenuBot.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace CaffeMenuBot.AppHost
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            using (IServiceScope scope = host.Services.CreateScope())
            {
                IServiceProvider provider = scope.ServiceProvider;
                CaffeMenuBotContext context = provider.GetRequiredService<CaffeMenuBotContext>();
                var logger = provider.GetRequiredService<ILogger<CaffeMenuBotContext>>();
                var config = provider.GetRequiredService<IConfiguration>();

                try
                {
                    if (context.Database.GetPendingMigrations().Any())
                        context.Database.Migrate();

                    DatabasePreparer.SeedDatabase(context);
                }
                catch (Exception e)
                {
                    logger.LogError(e.ToString());
                    logger.LogInformation(config.GetConnectionString("CaffeMenuBotDb"));
                }
            }
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, builder) =>
                {
                    var env = context.HostingEnvironment.EnvironmentName;
                    builder.AddJsonFile("dbsettings.json", false, true)
                        .AddJsonFile($"dbsettings.{env}.json", true, true);
                })
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
}