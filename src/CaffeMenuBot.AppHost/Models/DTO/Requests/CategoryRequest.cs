using System.ComponentModel.DataAnnotations;
using CaffeMenuBot.Data.Models.Menu;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;

namespace CaffeMenuBot.AppHost.Models.DTO.Requests
{
    [SwaggerSchemaFilter(typeof(CategoryRequestSchema))]
    public sealed record CategoryRequest 
    {
        public int Id { get; init; } = 0;

        [Required]
        public string CategoryName { get; init; } = null!;

        public bool IsVisible { get; init; } = true;

        public List<Dish>? Dishes { get; init; }
    }

    public sealed class CategoryRequestSchema : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            schema.Example = new OpenApiObject
            {
                ["id"] = new OpenApiInteger(3),
                ["categoryName"] = new OpenApiString("Test category"),
                ["isVisible"] = new OpenApiBoolean(true),
                ["dishes"] = new OpenApiArray() 
                {
                    new OpenApiObject() 
                    {
                        ["categoryId"] = new OpenApiInteger(3),
                        ["dishName"] = new OpenApiString("Test"),
                        ["description"] = new OpenApiString("Desc"),
                        ["price"] = new OpenApiFloat(500.5f),
                        ["serving"] = new OpenApiString("200гр."),
                    },
                    new OpenApiObject()
                    {
                        ["categoryId"] = new OpenApiInteger(3),
                        ["dishName"] = new OpenApiString("Test 2"),
                        ["description"] = new OpenApiString("Desc 2"),
                        ["price"] = new OpenApiFloat(500.7f),
                        ["serving"] = new OpenApiString("150гр."),
                    },
                }
            };
        }
    }
}
