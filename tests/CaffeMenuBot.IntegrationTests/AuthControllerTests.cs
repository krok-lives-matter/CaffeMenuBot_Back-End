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
        // Username: at least 4 characters; alphanumeric, underscore and hyphen only; max - 32 characters.
        // Password: at least 8 characters; max - 64 characters.
        [Theory]
        [InlineData("username=&password=",
            "Username and password fields are passed, but they have no values.")]
        [InlineData("username=test",
            "Username is passed and has a value, but there is no password parameter.")]
        [InlineData("password=Passw0rd",
            "Password is passed and has a value, but there is no username parameter.")]
        [InlineData("username=te&password=Passw0rd",
            "Username consists of 2 characters, but it has to be at least 4 characters.")]
        [InlineData("username=test&password=Passw0",
            "Password consists of 6 characters, but it has to be at least 8 characters.")]
        [InlineData("username=my_super_puper_duper_long_usernam&password=Passw0rd",
            "Username is 33 characters in length, but the max allowed length is 32 characters.")]
        [InlineData("username=test&password=n67AsCQhodZoH8wSzP7TXFX8YpUS8R6GUehNyFsYuoeXE8AvMWwjFCvycpgA63JmH",
            "Password is 65 characters in length, but the max allowed length is 64 characters.")]
        [InlineData("Username=test&Password=Passw0rd",
            "Upper-case parameter names are passed, but they must be lower-case.")]
        [InlineData("username=hello.world&password=Passw0rd",
            "Period character is forbidden to be used in the usernames.")]
        [InlineData("username=hello/world&password=Passw0rd",
            "Slash character is forbidden to be used in the usernames.")]
        [InlineData("username=hello,world&password=Passw0rd",
            "Comma character is forbidden to be used in the usernames.")]
        [InlineData("username=hello—world&password=Passw0rd",
            "Dash character is forbidden to be used in the usernames.")]
        [InlineData("",
            "Empty data string is passed.")]
        public async Task Login_BadRequestData_ReturnsHttpCode400(string data, string description)
        {
            HttpClient client = _factory.CreateClient();

            using StringContent content = new(data, Encoding.UTF8, "application/x-www-form-urlencoded");
            HttpResponseMessage result = await client.PostAsync("auth/login", content);
            
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        }
    }
}