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
        private readonly HttpClient _client;

        public DashboardAPIControllerTests(CaffeMenuBotWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
            _client = _factory.CreateDefaultClient();
            _client.DefaultRequestHeaders.Authorization
                         = new AuthenticationHeaderValue("Bearer", GetTokenAsync().GetAwaiter().GetResult());

        }

        private async Task<string> GetTokenAsync()
        {
            const string email = "admin@caffemenubot.com";
            const string password = "_Change$ThisPlease3";
            
            using StringContent content = 
            new($"{{\"email\":\"{email}\",\"password\":\"{password}\"}}", Encoding.UTF8, "application/json");
            HttpResponseMessage result = await _client.PostAsync("api/auth/login", content);
            
            AuthResponse resultBody = await result.Content.ReadFromJsonAsync<AuthResponse>();

            return resultBody.Token;
        }

        [Theory]
        [InlineData(Int32.MaxValue, "missing id is passed")]
        public async Task GetDishById_MissingIdSouldReturnNotFound(int id, string description)
        {
            HttpResponseMessage result = await _client.GetAsync($"api/dashboard/menu/dishes/{id}");

            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        }

        [Theory]
        [InlineData(Int32.MaxValue, "missing id is passed")]
        public async Task GetCategoryById_MissingIdSouldReturnNotFound(int id, string description)
        {
            HttpResponseMessage result = await _client.GetAsync($"api/dashboard/menu/categories/{id}");

            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        }
    }
}
