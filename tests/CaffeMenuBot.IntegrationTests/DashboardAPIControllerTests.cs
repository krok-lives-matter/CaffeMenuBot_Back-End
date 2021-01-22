﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CaffeMenuBot.AppHost;
using Xunit;

namespace CaffeMenuBot.IntegrationTests
{
    public sealed class DashboardAPIControllerTests : IClassFixture<CaffeMenuBotWebApplicationFactory<Startup>>
    {
        private readonly CaffeMenuBotWebApplicationFactory<Startup> _factory;

        public DashboardAPIControllerTests(CaffeMenuBotWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Theory]
        [InlineData(Int32.MaxValue, "missing id is passed")]
        public async Task GetDishById_MissingIdSouldReturnNotFound(int id, string description)
        {
            HttpClient client = _factory.CreateClient();

            HttpResponseMessage result = await client.GetAsync($"api/dashboard/getDish/{id}");

            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        }

        [Theory]
        [InlineData(Int32.MaxValue, "missing id is passed")]
        public async Task GetCategoryById_MissingIdSouldReturnNotFound(int id, string description)
        {
            HttpClient client = _factory.CreateClient();

            HttpResponseMessage result = await client.GetAsync($"api/dashboard/getCategory/{id}");

            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        }
    }
}
