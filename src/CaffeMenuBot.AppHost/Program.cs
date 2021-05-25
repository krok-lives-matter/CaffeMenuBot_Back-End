using System;
using System.Linq;
using System.Threading.Tasks;
using CaffeMenuBot.AppHost.Configuration;
using CaffeMenuBot.Data;
using CaffeMenuBot.Data.Models.Dashboard;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;
using Serilog;
using Serilog.Sinks.Loki;

namespace CaffeMenuBot.AppHost
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            using (IServiceScope scope = host.Services.CreateScope())
            {
                IServiceProvider provider   = scope.ServiceProvider;
                CaffeMenuBotContext context = provider.GetRequiredService<CaffeMenuBotContext>();
                UserManager<DashboardUser> userManager = provider.GetRequiredService<UserManager<DashboardUser>>();
                RoleManager<IdentityRole> roleManager = provider.GetRequiredService<RoleManager<IdentityRole>>();


                if (context.Database.GetPendingMigrations().Any())
                {              
                    context.Database.Migrate();
                    await DatabaseSeeder.SeedDatabaseAsync(context, userManager, roleManager); // if you do this later - connection will be disposed after ReloadTypes()
                    using (var conn = (NpgsqlConnection)context.Database.GetDbConnection())
                    {
                        conn.Open();
                        conn.ReloadTypes();
                    }
                }
                else
                    await DatabaseSeeder.SeedDatabaseAsync(context, userManager, roleManager);
            }
            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, builder) =>
                {
                    var env = context.HostingEnvironment.EnvironmentName;
                    builder.AddJsonFile("dbsettings.json", false, true)
                           .AddJsonFile($"dbsettings.{env}.json", true, true)
                           .AddJsonFile("botsettings.json", false, true)
                           .AddJsonFile($"botsettings.{env}.json", true, true)
                           .AddJsonFile("jwt.json", false, true)
                           .AddJsonFile($"jwt.{env}.json", true, true);
                })
                .ConfigureLogging((context, builder) =>
                {
                    builder.AddConfiguration(context.Configuration);
                    builder.AddConsole();
                    builder.AddDebug();

                    if (context.HostingEnvironment.IsProduction())
                    {
                        var credentials = new NoAuthCredentials("https://loki.vova-lantsov.dev/");
                        var lokiLogger = new LoggerConfiguration()
                            .MinimumLevel.Information()
                            .Enrich.FromLogContext()
                            .WriteTo.LokiHttp(credentials)
                            .CreateLogger();
                        builder.AddSerilog(lokiLogger);
                    }
                })
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
}