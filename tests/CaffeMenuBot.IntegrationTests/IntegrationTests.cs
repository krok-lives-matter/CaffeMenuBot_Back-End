using System;
using System.Collections.Generic;
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
        public async Task ListAllCategoriesAndDishes()
        {
            HttpResponseMessage result = await _authorizedClient.GetAsync($"api/dashboard/menu/categories");
            List<Category> categoriesResultEncapsulated = await result.Content.ReadFromJsonAsync<List<Category>>();

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.True(categoriesResultEncapsulated.Count() != 0, "Categories are not 0 count");
            Assert.True(categoriesResultEncapsulated[0].Dishes.Count() != 0, "Categories index 0 dishes are not 0 count");
        }

        [Theory]
        [InlineData("Updated name", "name to update")]
        public async Task UpdateDish(string name, string description)
        {
            HttpResponseMessage result = await _authorizedClient.GetAsync($"api/dashboard/menu/dishes");
            List<Dish> dishesResultEncapsulated = await result.Content.ReadFromJsonAsync<List<Dish>>();
            dishesResultEncapsulated[0].DishName = name;

            using StringContent dishUpdateContent = new(JsonSerializer.Serialize(dishesResultEncapsulated[0]), Encoding.UTF8, "application/json");
            HttpResponseMessage dishUpdateResult = await _authorizedClient.PutAsync($"api/dashboard/menu/dishes", dishUpdateContent);
            Dish dishResultEncapsulated = await dishUpdateResult.Content.ReadFromJsonAsync<Dish>();

            Assert.Equal(dishResultEncapsulated.DishName, name);
        }

        [Fact]
        public async Task DeleteDishById_SuccessStatusCode()
        {
            HttpResponseMessage deleteResult = await _authorizedClient.DeleteAsync($"api/dashboard/menu/dishes/1");
            Assert.True(deleteResult.IsSuccessStatusCode);
        }


        [Theory]
        [InlineData("Updated name", "name to update")]
        public async Task UpdateCategory(string name, string description)
        {
            HttpResponseMessage result = await _authorizedClient.GetAsync($"api/dashboard/menu/categories");
            List<Category> categoriesResultEncapsulated = await result.Content.ReadFromJsonAsync<List<Category>>();
            categoriesResultEncapsulated[0].CategoryName = name;

            using StringContent categoryUpdateContent = new(JsonSerializer.Serialize(categoriesResultEncapsulated[0]), Encoding.UTF8, "application/json");
            HttpResponseMessage categoryUpdateResult = await _authorizedClient.PutAsync($"api/dashboard/menu/categories", categoryUpdateContent);
            Category categoryResultEncapsulated = await categoryUpdateResult.Content.ReadFromJsonAsync<Category>();

            Assert.Equal(categoryResultEncapsulated.CategoryName, name);
        }


        [Fact]
        public async Task AddCategory_AddDishToCategoryById_SuccessStatusCode_CategoryIncludesNewDish()
        {
            Category category = new Category()
            {
                CategoryName = "Test Category"
            };
            using StringContent categoryAddContent = new(JsonSerializer.Serialize(category), Encoding.UTF8, "application/json");
            HttpResponseMessage categoryAddResult = await _authorizedClient.PostAsync($"api/dashboard/menu/categories", categoryAddContent);
            Category categoryResultEncapsulated = await categoryAddResult.Content.ReadFromJsonAsync<Category>();

            Assert.True(categoryAddResult.IsSuccessStatusCode, "Category added");

            Dish dish = new Dish()
            {
                DishName = "test dish",
                Description = "hello world!",
                CategoryId = categoryResultEncapsulated.Id,
                Price = 50.5m,
                Serving = "200g"
            };

            string json = JsonSerializer.Serialize(dish);
            using StringContent dishContent = new(json, Encoding.UTF8, "application/json");
            HttpResponseMessage dishResult = await _authorizedClient.PostAsync($"api/dashboard/menu/dishes", dishContent);
            dish = await dishResult.Content.ReadFromJsonAsync<Dish>();

            Assert.True(dishResult.IsSuccessStatusCode, "dish added");


            // update category object
            categoryAddResult = await _authorizedClient.GetAsync($"api/dashboard/menu/categories/{categoryResultEncapsulated.Id}");
            categoryResultEncapsulated = await categoryAddResult.Content.ReadFromJsonAsync<Category>();

            Assert.True(categoryResultEncapsulated.Dishes.Any(d => d.Id == dish.Id), "category includes new dish");
        }

        [Fact]
        public async Task DeleteCategoryById_SuccessStatusCode()
        {
            HttpResponseMessage deleteResult = await _authorizedClient.DeleteAsync($"api/dashboard/menu/categories/1");
            Assert.True(deleteResult.IsSuccessStatusCode);
        }

        #endregion
    }
}