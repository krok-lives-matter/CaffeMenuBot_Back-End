using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CaffeMenuBot.AppHost.Models
{
    public sealed record CreateItemLinkResult
    {
        public string ImageLink { get; init; } = null!;
    }
    
    public sealed class CreateItemLinkResultSchema : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            schema.Example = new OpenApiObject
            {
                ["imageLink"] = new OpenApiString("/media/category_covers/fileName.png")
            };
        }
    }
}