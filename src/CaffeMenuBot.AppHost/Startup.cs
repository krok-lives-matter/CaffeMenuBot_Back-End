using System.Text.Json.Serialization;
using CaffeMenuBot.AppHost.Authentication;
using CaffeMenuBot.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
            
            services.AddDefaultIdentity<ApplicationUser>()
                .AddEntityFrameworkStores<AuthorizationDbContext>();

            services.AddIdentityServer()
                .AddApiAuthorization<ApplicationUser, AuthorizationDbContext>();
            
            services.AddAuthentication()
                .AddIdentityServerJwt();
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