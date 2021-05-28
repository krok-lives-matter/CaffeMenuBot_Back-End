using System;
using System.Linq;
using System.Text;
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
using Microsoft.OpenApi.Models;
using CaffeMenuBot.Bot.Actions.Interface;
using CaffeMenuBot.Data.Models.Dashboard;
using CaffeMenuBot.AppHost.Services;
using CaffeMenuBot.AppHost.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace CaffeMenuBot.AppHost
{
    public sealed class Startup
    {
        public const string BaseImageUrl = "https://cmb-api.vova-lantsov.dev/media";
        
        const string publicCorsPolicyName = "publicCORS";
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(name: publicCorsPolicyName,
                    builder =>
                    {
                        builder.AllowAnyOrigin()
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    });
            });

            services.AddControllers().ConfigureApiBehaviorOptions(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var result = new BadRequestObjectResult(context.ModelState);

                    result.ContentTypes.Add(MediaTypeNames.Application.Json);
                    result.ContentTypes.Add(MediaTypeNames.Application.Xml);
        
                    return result;
                };
            });

            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddIdentity<DashboardUser, IdentityRole>(options =>
                {
                    options.SignIn.RequireConfirmedAccount = true;
                    options.ClaimsIdentity.UserIdClaimType = "Id";
                    options.ClaimsIdentity.UserNameClaimType = "Username";
                    options.ClaimsIdentity.RoleClaimType = "Roles";
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
            services.AddScoped<JwtHelper>();

            ConfigureBot(services);
            ConfigureLoadAvgMonitor(services);

            // within this section we are configuring the authentication and setting the default scheme
            services
                .AddAuthentication(options =>
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
                        ValidateIssuerSigningKey =
                            true, // this will validate the 3rd part of the jwt token using the secret that we added in the appsettings and verify we have generated the jwt token
                        IssuerSigningKey = new SymmetricSecurityKey(key), // Add the secret key to our Jwt encryption
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        RequireExpirationTime = false,
                        ValidateLifetime = true,
                        RoleClaimType = "Roles",
                        NameClaimType = "Username"
                    };
                });

            services.AddSwaggerGen(c =>
            {           
                var securityScheme = new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    Description = "JWT Authorization header using the Bearer scheme. " +
                                  "You should request JWT token using login endpoints.",
                    Name = "Authorize"
                };
                c.AddSecurityDefinition("jwt", securityScheme);

                var securityRequirement = new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference {Type = ReferenceType.SecurityScheme, Id = "jwt"}
                        },
                        Array.Empty<string>()
                    }
                };
                c.AddSecurityRequirement(securityRequirement);

                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Dashboard API",
                    Description = "An API for CaffeMenuBot Dashboard",
                    Contact = new OpenApiContact
                    {
                        Name = "Vova Lantsov, Edvard Potapenko, Dmitriy Gurskiy",
                        Email = "contact@vova-lantsov.dev, potapenkoes@krok.edu.ua, gurskiydv@krok.edu.ua",
                        Url = new Uri("https://github.com/krok-lives-matter/CaffeMenuBot_Back-End"),
                    }
                });

                c.EnableAnnotations();
                c.SupportNonNullableReferenceTypes();
                c.DescribeAllParametersInCamelCase();
            });
        }

        private void ConfigureLoadAvgMonitor(IServiceCollection services)
        {
            services.AddSingleton<LoadAvgBackgroundService>();
            services.AddHostedService(sp => sp.GetRequiredService<LoadAvgBackgroundService>());
        }

        private void ConfigureBot(IServiceCollection services)
        {
            try
            {
                services.AddSingleton<ITelegramBotClient>(
                new TelegramBotClient(_configuration["BOT_TOKEN"]));
            }
            catch (ArgumentException)
            {
                Console.WriteLine("Skipping bot initialization, no bot token is provided");
                return;
            }
            services.AddLogging();
            services.AddSingleton<PatternManager<IChatAction>>();
            services.AddSingleton<PatternManager<IStateAction>>();
            services.AddSingleton<IUpdateHandler, BotHandler>();
            services.AddSingleton<BotHandlerService>();
            services.AddHostedService(sp => sp.GetRequiredService<BotHandlerService>());

            var baseType = typeof(IChatAction);
            
            foreach (var commandType in baseType.Assembly.GetTypes().Where(t => baseType.IsAssignableFrom(t) && t.IsClass && t.IsPublic && !t.IsAbstract))
            {
                services.AddScoped(baseType, commandType);
            }

            baseType = typeof(IStateAction);

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
                app.UseExceptionHandler("/error-local-development");
            }
            else
            {
                app.UseExceptionHandler("/error");
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "DashboardAPI v1");
            });

            app.UseStaticFiles();

            app.UseCors(publicCorsPolicyName);

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