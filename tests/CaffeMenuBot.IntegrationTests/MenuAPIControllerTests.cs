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
using CaffeMenuBot.Data.Models.Menu;
using System.Text.Json;
using System.Linq;

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

        //[Fact]
        //public async Task AddCategory_ShouldReturnSuccessStatusCode()
        //{
        //    Category category = new Category()
        //    {
        //        CategoryName = "Test Category"
        //    };
        //    using StringContent content = new(JsonSerializer.Serialize(category), Encoding.UTF8, "application/json");
        //    HttpResponseMessage result = await _client.PostAsync($"api/dashboard/menu/categories", content);

        //    Assert.True(result.IsSuccessStatusCode);
        //}

        //[Fact]
        //public async Task AddSubCategory_SuccessStatusCode_CountNotZero()
        //{   
        //    Category subcategory = new Category()
        //    {
        //        CategoryName = "test subcategory",
        //        ParentCategoryId = 1
        //    };

        //    string json = JsonSerializer.Serialize(subcategory);
        //    using StringContent content = new(json, Encoding.UTF8, "application/json");
        //    HttpResponseMessage result = await _client.PostAsync($"api/dashboard/menu/categories", content);

        //    Assert.True(result.IsSuccessStatusCode);

        //    HttpResponseMessage categoryResult = await _client.GetAsync($"api/dashboard/menu/categories/1");
        //    Category category = await categoryResult.Content.ReadFromJsonAsync<Category>();

        //    Assert.True(category.SubCategories.ToList().Count() != 0);
        //}

        //[Fact]
        //public async Task AddDishToCategory_SuccessStatusCode_CountNotZero()
        //{

        //    Dish dish = new Dish()
        //    {
        //        DishName = "test dish",
        //        CategoryId = 2,
        //        Price = 50.5m,
        //        Serving = "200g"
        //    };

        //    string json = JsonSerializer.Serialize(dish);
        //    using StringContent content = new(json, Encoding.UTF8, "application/json");
        //    HttpResponseMessage result = await _client.PostAsync($"api/dashboard/menu/dishes", content);

        //    Assert.True(result.IsSuccessStatusCode);

        //    HttpResponseMessage categoryResult = await _client.GetAsync($"api/dashboard/menu/categories/2");
        //    Category category = await categoryResult.Content.ReadFromJsonAsync<Category>();

        //    Assert.True(category.Dishes.Count() > 1);
        //}
    }
}
