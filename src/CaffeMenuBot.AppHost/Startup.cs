using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using CaffeMenuBot.Bot.Commands;
using CaffeMenuBot.Bot.Services;
using CaffeMenuBot.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using CaffeMenuBot.AppHost.Options;
using Microsoft.AspNetCore.Identity;

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
            services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
                });
            
            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddIdentity<IdentityUser, IdentityRole>(options =>
            {
                options.SignIn.RequireConfirmedAccount = true;
                options.ClaimsIdentity.UserIdClaimType = "Id";
            })
            .AddRoles<IdentityRole>()
            .AddRoleManager<RoleManager<IdentityRole>>()
            .AddEntityFrameworkStores<CaffeMenuBotContext>();
            

            services.AddDbContext<CaffeMenuBotContext>(options =>
                options.UseNpgsql(_configuration.GetConnectionString("CaffeMenuBotDb"), builder =>
                    builder.EnableRetryOnFailure()
                        .MigrationsAssembly("CaffeMenuBot.Data")
                        .MigrationsHistoryTable("__MigrationHistory", CaffeMenuBotContext.SchemaName)));            

            services.Configure<JwtOptions>(_configuration.GetSection("Jwt"));
            
            ConfigureBot(services);

            // within this section we are configuring the authentication and setting the default scheme
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(jwt =>
            {
                var key = Encoding.ASCII.GetBytes(_configuration["Jwt:SecretKey"]);

                jwt.SaveToken = true;
                jwt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true, // this will validate the 3rd part of the jwt token using the secret that we added in the appsettings and verify we have generated the jwt token
                    IssuerSigningKey = new SymmetricSecurityKey(key), // Add the secret key to our Jwt encryption
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    RequireExpirationTime = false,
                    ValidateLifetime = true
                };
            });
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
            }

            app.UseStaticFiles();
            
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}