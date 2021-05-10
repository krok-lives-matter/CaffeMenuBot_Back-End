using System;
using System.Linq;
using CaffeMenuBot.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CaffeMenuBot.IntegrationTests
{
    public sealed class CaffeMenuBotWebApplicationFactory<TStartup>
        : WebApplicationFactory<TStartup> where TStartup: class
    {
        public CaffeMenuBotWebApplicationFactory()
        {
            ClientOptions.BaseAddress = new Uri("http://localhost:5001/");
            ClientOptions.AllowAutoRedirect = false;
        }
        
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType ==
                         typeof(DbContextOptions<CaffeMenuBotContext>));

                services.Remove(descriptor);

                services.AddDbContext<CaffeMenuBotContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryDbForTesting");
                });

                var sp = services.BuildServiceProvider();

                using var scope = sp.CreateScope();
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<CaffeMenuBotContext>();
                var logger = scopedServices
                    .GetRequiredService<ILogger<CaffeMenuBotWebApplicationFactory<TStartup>>>();

                db.Database.EnsureCreated();

                //try
                //{
                //    Utilities.InitializeDatabaseForTests(db, scopedServices);
                //}
                //catch (Exception ex)
                //{
                //   logger.LogError(ex, "An error occurred seeding the " +
                //                        "database with test data. Error: {Message}", ex.Message);
                //}
            });
            builder.UseUrls("http://localhost:5001/");
        }
    }
}