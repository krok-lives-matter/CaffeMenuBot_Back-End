using CaffeMenuBot.Data.Models.Menu;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace CaffeMenuBot.AppHost.Models.DTO.Requests
{
    public sealed record CategoryRequest 
    {
        public int Id { get; init; } = 0;

        public string CategoryName { get; init; } = null!;

        public IFormFile? CoverPhoto { get; init; } = null;

        public bool IsVisible { get; set; } = true;

        public List<Dish>? Dishes { get; init; }
    }
}
