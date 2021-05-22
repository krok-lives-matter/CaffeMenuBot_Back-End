using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CaffeMenuBot.AppHost;
using CaffeMenuBot.AppHost.Models.DTO.Responses;
using CaffeMenuBot.Data.Models.Menu;
using Xunit;

namespace CaffeMenuBot.IntegrationTests
{
    public sealed class IntegrationTests : IClassFixture<CaffeMenuBotWebApplicationFactory<Startup>>
    {
        private readonly CaffeMenuBotWebApplicationFactory<Startup> _factory;
        private readonly HttpClient _client;
        private readonly HttpClient _authorizedClient;

        public IntegrationTests(CaffeMenuBotWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
            _client = _factory.CreateDefaultClient();
            _authorizedClient = _factory.CreateDefaultClient();
            _authorizedClient.DefaultRequestHeaders.Authorization
                         = new AuthenticationHeaderValue("Bearer", GetTokenAsync().GetAwaiter().GetResult());
        }

        #region auth tests

        // Requirements:
        // Email: at least 4 characters; validation the same as for <input type="email">; max - 254 characters.
        // Password: at least 8 characters; max - 64 characters.
        [Theory]
        [InlineData(@"{""email"":"""",""password"":""""}",
            "Email and password fields are passed, but they have no values.")]
        [InlineData(@"""email"""""":""test@gmail.com""",
            "Email is passed and has a value, but there is no password parameter.")]
        [InlineData(@"""password"":""Passw0rd""",
            "Password is passed and has a value, but there is no email parameter.")]
        [InlineData(@"""email"":""""a@b,""password"":""Passw0rd""",
            "Email consists of 3 characters, but it has to be at least 4 characters.")]
        [InlineData(@"""email"":""test@gmail.com"",""password"":""Passw0r""",
            "Password consists of 7 characters, but it has to be at least 8 characters.")]
        [InlineData(@"""email"":""test@gmail.com"",""password"":""n67AsCQhodZoH8wSzP7TXFX8YpUS8R6GUehNyFsYuoeXE8AvMWwjFCvycpgA63JmH""",
            "Password is 65 characters in length, but the max allowed length is 64 characters.")]
        [InlineData(@"""email"":""test@gmail@com"",""password"":""Passw0rd""",
            "Invalid email value.")]
        [InlineData(@"""email"":""hello.world@"",""password"":""Passw0rd""",
            "Invalid email value.")]
        [InlineData(@"""email"":""@hello/world"",""password"":""Passw0rd""",
            "Invalid email value.")]
        [InlineData(@"""email"":""gmail.com"",""password"":""Passw0rd""",
            "Invalid email value.")]
        [InlineData(@"""email"":""hello—world_gmail.com"",""password"":""Passw0rd""",
            "Invalid email value.")]
        [InlineData("",
            "Empty data string is passed.")]
        public async Task Login_BadRequestData_ReturnsHttpCode400(string data, string description)
        {
            using StringContent content = new(data, Encoding.UTF8, "application/json");
            HttpResponseMessage result = await _client.PostAsync("api/auth/login", content);
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Fact]
        public async Task Login_Ok()
        {
            const string email = "admin@caffemenubot.com";
            const string password = "_Change$ThisPlease3";
            
            using StringContent content = 
            new($"{{\"email\":\"{email}\",\"password\":\"{password}\"}}", Encoding.UTF8, "application/json");
            HttpResponseMessage result = await _client.PostAsync("api/auth/login", content);
            
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        }

        #endregion

        #region MenuApiTests
        private async Task<string> GetTokenAsync()
        {
            const string email = "admin@caffemenubot.com";
            const string password = "_Change$ThisPlease3";

            using StringContent content =
            new($"{{\"email\":\"{email}\",\"password\":\"{password}\"}}", Encoding.UTF8, "application/json");
            HttpResponseMessage result = await _authorizedClient.PostAsync("api/auth/login", content);

            AuthResponse resultBody = await result.Content.ReadFromJsonAsync<AuthResponse>();

            return resultBody.Token;
        }

        [Theory]
        [InlineData(Int32.MaxValue, "missing id is passed")]
        public async Task GetDishById_MissingIdSouldReturnNotFound(int id, string description)
        {
            HttpResponseMessage result = await _authorizedClient.GetAsync($"api/dashboard/menu/dishes/{id}");

            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        }

        [Theory]
        [InlineData(Int32.MaxValue, "missing id is passed")]
        public async Task GetCategoryById_MissingIdSouldReturnNotFound(int id, string description)
        {
            HttpResponseMessage result = await _authorizedClient.GetAsync($"api/dashboard/menu/categories/{id}");

            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        }

        [Fact]
        public async Task AddCategory_ShouldReturnSuccessStatusCode()
        {
            Category category = new Category()
            {
                CategoryName = "Test Category"
            };
            using StringContent content = new(JsonSerializer.Serialize(category), Encoding.UTF8, "application/json");
            HttpResponseMessage result = await _authorizedClient.PostAsync($"api/dashboard/menu/categories", content);

            Assert.True(result.IsSuccessStatusCode);
        }

        //[Fact]
        //public async Task AddDishToCategory_SuccessStatusCode_CountNotZero()
        //{

        //    Dish dish = new Dish()
        //    {
        //        DishName = "test dish",
        //        Description = "hello world!",
        //        CategoryId = 1,
        //        Price = 50.5m,
        //        Serving = "200g"
        //    };

        //    string json = JsonSerializer.Serialize(dish);
        //    using StringContent content = new(json, Encoding.UTF8, "application/json");
        //    HttpResponseMessage result = await _authorizedClient.PostAsync($"api/dashboard/menu/dishes", content);

        //    Assert.True(result.IsSuccessStatusCode);

        //    HttpResponseMessage categoryResult = await _authorizedClient.GetAsync($"api/dashboard/menu/categories/1");
        //    Category category = await categoryResult.Content.ReadFromJsonAsync<Category>();

        //    Assert.True(category.Dishes.Count() > 1);
        //}

        #endregion
    }
}