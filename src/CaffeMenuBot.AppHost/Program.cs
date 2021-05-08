using System;
using System.Linq;
using System.Threading.Tasks;
using CaffeMenuBot.AppHost.Authentication;
using CaffeMenuBot.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CaffeMenuBot.AppHost
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            using (IServiceScope scope = host.Services.CreateScope())
            {
                IServiceProvider             provider    = scope.ServiceProvider;
                CaffeMenuBotContext          context     = provider.GetRequiredService<CaffeMenuBotContext>();
                AuthorizationDbContext       authContext = provider.GetRequiredService<AuthorizationDbContext>();
                UserManager<ApplicationUser> userManager = provider.GetRequiredService<UserManager<ApplicationUser>>();
                RoleManager<IdentityRole>    roleManager = provider.GetRequiredService<RoleManager<IdentityRole>>();
                

                if (context.Database.GetPendingMigrations().Any())
                    context.Database.Migrate();
                if (authContext.Database.GetPendingMigrations().Any())
                    authContext.Database.Migrate();

                AuthenticationSeeder.SeedAdminUser
                (
                    authContext,
                    userManager,
                    roleManager
                );
                
            }
            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                    logging.AddDebug();
                })
                .ConfigureAppConfiguration((context, builder) =>
                {
                    var env = context.HostingEnvironment.EnvironmentName;
                    builder.AddJsonFile("dbsettings.json", false, true)
                        .AddJsonFile($"dbsettings.{env}.json", true, true)
                        .AddJsonFile("botsettings.json", false, true)
                        .AddJsonFile($"botsettings.{env}.json", true, true);
                })
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
}