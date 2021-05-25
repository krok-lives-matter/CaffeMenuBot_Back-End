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
using CaffeMenuBot.Data.Models.Reviews;
using CaffeMenuBot.Data.Models.Schedule;
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
        }

        [Theory]
        [InlineData("Updated name", "name to update")]
        public async Task UpdateDish(string name, string description)
        {
            HttpResponseMessage result = await _authorizedClient.GetAsync($"api/dashboard/menu/dishes?category_id=1");
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
            HttpResponseMessage deleteResult = await _authorizedClient.DeleteAsync($"api/dashboard/menu/dishes?dish_id=1");
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
        }

        [Fact]
        public async Task DeleteCategoryById_SuccessStatusCode()
        {
            HttpResponseMessage deleteResult = await _authorizedClient.DeleteAsync($"api/dashboard/menu/categories/1");
            Assert.True(deleteResult.IsSuccessStatusCode);
        }

        #endregion

        #region ScheduleApiTests
        [Fact]
        public async Task GetAllSchedules_Success()
        {
            HttpResponseMessage getResult = await _authorizedClient.GetAsync($"api/dashboard/schedule/");
            List<Schedule> scheduleEncapsulated = await getResult.Content.ReadFromJsonAsync<List<Schedule>>();

            Assert.True(getResult.IsSuccessStatusCode);
            Assert.True(scheduleEncapsulated.Count() != 0);
        }

        [Theory]
        [InlineData(Int32.MaxValue, "missing ID has passed")]
        public async Task GetScheduleById_MissingIdShouldReturnNotFound(int id, string description)
        {
            HttpResponseMessage getResult = await _authorizedClient.GetAsync($"api/dashboard/schedule/{id}");

            Assert.Equal(HttpStatusCode.NotFound, getResult.StatusCode);
        }

        [Fact]
        public async Task GetScheduleById_Success()
        {
            HttpResponseMessage getResult = await _authorizedClient.GetAsync($"api/dashboard/schedule/2");
            var scheduleEncapsulated = await getResult.Content.ReadFromJsonAsync<Schedule>();

            Assert.NotNull(scheduleEncapsulated);
            Assert.True(getResult.IsSuccessStatusCode);
        }

        [Fact]
        public async Task DeleteScheduleById_Success()
        {
            HttpResponseMessage deleteResult = await _authorizedClient.DeleteAsync($"api/dashboard/schedule/1");
            Assert.True(deleteResult.IsSuccessStatusCode);
        }

        [Theory]
        [InlineData("Updated weekday name", "name to update")]
        public async Task UpdateSchedule(string wdName, string description)
        {
            HttpResponseMessage result = await _authorizedClient.GetAsync($"api/dashboard/schedule/");
            List<Schedule> schedulesResultEncapsulated = await result.Content.ReadFromJsonAsync<List<Schedule>>();
            schedulesResultEncapsulated[0].WeekdayName = wdName;

            using StringContent scheduleUpdateContent = new(JsonSerializer.Serialize(schedulesResultEncapsulated[0]), Encoding.UTF8, "application/json");
            HttpResponseMessage scheduleUpdateResult = await _authorizedClient.PutAsync($"api/dashboard/schedule/", scheduleUpdateContent);
            Schedule scheduleResultEncapsulated = await scheduleUpdateResult.Content.ReadFromJsonAsync<Schedule>();

            Assert.Equal(scheduleResultEncapsulated.WeekdayName, wdName);
        }

        [Fact]
        public async Task CreateSchedule_Success()
        {
            Schedule schedule = new Schedule()
            {
                OrderIndex = 8,
                WeekdayName = "Wednesday",
                OpenTime = "00:00",
                CloseTime = "00:01"

            };
            using StringContent scheduleAddContent = new(JsonSerializer.Serialize(schedule), Encoding.UTF8, "application/json");
            HttpResponseMessage scheduleAddResult = await _authorizedClient.PostAsync($"api/dashboard/schedule", scheduleAddContent);
            Schedule scheduleResultEncapsulated = await scheduleAddResult.Content.ReadFromJsonAsync<Schedule>();

            Assert.True(scheduleAddResult.IsSuccessStatusCode, "Schedule created");
        }
        #endregion

        #region ReviewApiTest
        [Fact]
        public async Task ListAllReviews_SuccessStatusCode()
        {
            HttpResponseMessage result = await _authorizedClient.GetAsync($"api/dashboard/reviews");
            List<Review> reviewsEncapsulated = await result.Content.ReadFromJsonAsync<List<Review>>();

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        }

        [Theory]
        [InlineData("My updated comment")]
        public async Task CrudReview_SucessStatusCode_NotNull_UpdatedCommentEquals(string comment)
        {
            await PostReview();
            int review_id = await UpdateReview(comment);
            await GetReviewById(review_id);
            await DeleteReviewById(review_id);
        }

        private async Task PostReview()
        {
            // create new review

            Review review = new Review()
            {
                Rating = Data.Models.Bot.Rating.rating_excellent,
                ReviewComment = "This is test review",
                User = new Data.Models.Bot.BotUser()
                {
                    Id = 67123616,
                    State = Data.Models.Bot.ChatState.default_state
                }
            };

            // post new review
            string json = JsonSerializer.Serialize(review);
            using StringContent reviewContent = new(json, Encoding.UTF8, "application/json");
            HttpResponseMessage postResult = await _authorizedClient.PostAsync($"api/dashboard/reviews", reviewContent);

            // make sure posted
            Assert.Equal(HttpStatusCode.OK, postResult.StatusCode);
        }

        private async Task<int> UpdateReview(string comment)
        {
            // get all reviews
            HttpResponseMessage result = await _authorizedClient.GetAsync($"api/dashboard/reviews");
            List<Review> reviewsEncapsulated = await result.Content.ReadFromJsonAsync<List<Review>>();

            // update review
            reviewsEncapsulated[0].ReviewComment = comment;

            // put updated review
            string json = JsonSerializer.Serialize(reviewsEncapsulated[0]);
            using StringContent updateReviewContent = new(json, Encoding.UTF8, "application/json");
            HttpResponseMessage updatedResult = await _authorizedClient.PutAsync($"api/dashboard/reviews", updateReviewContent);
            // get updated result
            Review reviewEncapsulated = await updatedResult.Content.ReadFromJsonAsync<Review>();

            // make sure comment was updated
            Assert.Equal(reviewEncapsulated.ReviewComment, comment);

            return reviewEncapsulated.Id;
        }

        private async Task GetReviewById(int id)
        {
            // get review
            HttpResponseMessage result = await _authorizedClient.GetAsync($"api/dashboard/reviews/{id}");
            Review reviewEncapsulated = await result.Content.ReadFromJsonAsync<Review>();

            Assert.True(result.IsSuccessStatusCode, "Success get review by id");
            Assert.NotNull(reviewEncapsulated);
        }

        private async Task DeleteReviewById(int id)
        {
            // delete review
            HttpResponseMessage result = await _authorizedClient.DeleteAsync($"api/dashboard/reviews/{id}");

            Assert.True(result.IsSuccessStatusCode, "Success delete review by id");
        }


        #endregion
    }
}