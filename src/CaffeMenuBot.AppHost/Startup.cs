using System.Linq;
using System.Text.Json.Serialization;
using CaffeMenuBot.AppHost.Authentication;
using CaffeMenuBot.Bot.Commands;
using CaffeMenuBot.Bot.Services;
using CaffeMenuBot.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;

namespace CaffeMenuBot.AppHost
{
    public sealed class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
                });
            services.AddRazorPages();
            
            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddDbContext<CaffeMenuBotContext>(options =>
                options.UseNpgsql(_configuration.GetConnectionString("CaffeMenuBotDb"), builder =>
                    builder.EnableRetryOnFailure()
                        .MigrationsAssembly("CaffeMenuBot.Data")
                        .MigrationsHistoryTable("__MigrationHistory", CaffeMenuBotContext.SchemaName)));

            services.AddDbContext<AuthorizationDbContext>(options =>
                options.UseNpgsql(_configuration.GetConnectionString("CaffeMenuBotDb"), builder =>
                    builder.EnableRetryOnFailure()));
            
            this.ConfigureBot(services);
            
            services.AddDefaultIdentity<ApplicationUser>()
                .AddRoles<IdentityRole>()
                .AddRoleManager<RoleManager<IdentityRole>>()
                .AddEntityFrameworkStores<AuthorizationDbContext>();

            services.AddIdentityServer()
                .AddApiAuthorization<ApplicationUser, AuthorizationDbContext>();
            
            services.AddAuthentication()
                .AddIdentityServerJwt();
        }

        private void ConfigureBot(IServiceCollection services)
        {
            services.AddLogging();
            services.AddSingleton<CommandPatternManager>();
            services.AddSingleton<ITelegramBotClient>(
                new TelegramBotClient(_configuration["BOT_TOKEN"]));
            services.AddSingleton<IUpdateHandler, BotHandler>();
            services.AddHostedService<BotHandlerService>();

            var baseType = typeof(IChatAction);
            
            foreach (var commandType in baseType.Assembly.GetTypes().Where(t => baseType.IsAssignableFrom(t) && t.IsClass && t.IsPublic && !t.IsAbstract))
            {
                services.AddScoped(baseType, commandType);
            }
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //app.UseMigrationsEndPoint();
                app.UseWebAssemblyDebugging();
            }

            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();
            
            app.UseRouting();

            app.UseIdentityServer();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
                endpoints.MapFallbackToFile("index.html");
            });
        }
    }
}