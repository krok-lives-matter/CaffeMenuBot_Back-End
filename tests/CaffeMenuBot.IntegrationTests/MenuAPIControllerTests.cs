using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using CaffeMenuBot.AppHost;
using Xunit;
using System.Net.Http.Json;
using System.Text;
using System.Net.Http.Headers;
using CaffeMenuBot.AppHost.Models.DTO.Responses;

namespace CaffeMenuBot.IntegrationTests
{
    public sealed class DashboardAPIControllerTests : IClassFixture<CaffeMenuBotWebApplicationFactory<Startup>>
    {
        private readonly CaffeMenuBotWebApplicationFactory<Startup> _factory;

        public DashboardAPIControllerTests(CaffeMenuBotWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        private async Task<string> GetTokenAsync()
        {
            const string email = "test@caffemenubot.com";
            const string password = "_Change$ThisPlease3";
            
            HttpClient client = _factory.CreateClient();

            using StringContent content = 
            new($"{{\"email\":\"{email}\",\"password\":\"{password}\"}}", Encoding.UTF8, "application/json");
            HttpResponseMessage result = await client.PostAsync("api/auth/login", content);
            
            RegistrationResponse resultBody = await result.Content.ReadFromJsonAsync<RegistrationResponse>();

            return resultBody.Token;
        }

        [Theory]
        [InlineData(Int32.MaxValue, "missing id is passed")]
        public async Task GetDishById_MissingIdSouldReturnNotFound(int id, string description)
        {
            HttpClient client = _factory.CreateClient();

            client.DefaultRequestHeaders.Authorization 
                         = new AuthenticationHeaderValue("Bearer", await this.GetTokenAsync());

            HttpResponseMessage result = await client.GetAsync($"api/dashboard/menu/dishes/{id}");

            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        }

        [Theory]
        [InlineData(Int32.MaxValue, "missing id is passed")]
        public async Task GetCategoryById_MissingIdSouldReturnNotFound(int id, string description)
        {
            HttpClient client = _factory.CreateClient();

            client.DefaultRequestHeaders.Authorization 
                         = new AuthenticationHeaderValue("Bearer", await this.GetTokenAsync());

            HttpResponseMessage result = await client.GetAsync($"api/dashboard/menu/categories/{id}");

            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        }
    }
}
