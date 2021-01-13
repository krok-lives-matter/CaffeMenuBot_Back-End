using System.Threading.Tasks;
using CaffeMenuBot.AppHost;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace CaffeMenuBot.IntegrationTests
{
    public sealed class TestControllerTests
        : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;

        public TestControllerTests(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Get_TestEndpoint()
        {
            var client = _factory.CreateClient();
            var response = await client.GetStringAsync("api/test");
            Assert.Equal("OK", response);
        }
    }
}