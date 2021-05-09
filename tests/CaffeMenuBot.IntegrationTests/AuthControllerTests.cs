using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CaffeMenuBot.AppHost;
using Xunit;

namespace CaffeMenuBot.IntegrationTests
{
    public sealed class AuthControllerTests : IClassFixture<CaffeMenuBotWebApplicationFactory<Startup>>
    {
        private readonly CaffeMenuBotWebApplicationFactory<Startup> _factory;

        public AuthControllerTests(CaffeMenuBotWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

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
            HttpClient client = _factory.CreateClient();

            using StringContent content = new(data, Encoding.UTF8, "application/json");
            HttpResponseMessage result = await client.PostAsync("api/auth/login", content);
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Fact]
        public async Task Login_Ok()
        {
            const string email = "admin@caffemenubot.com";
            const string password = "_Change$ThisPlease3";
            
            HttpClient client = _factory.CreateClient();

            using StringContent content = 
            new($"{{\"email\":\"{email}\",\"password\":\"{password}\"}}", Encoding.UTF8, "application/json");
            HttpResponseMessage result = await client.PostAsync("api/auth/login", content);
            
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        }
    }
}