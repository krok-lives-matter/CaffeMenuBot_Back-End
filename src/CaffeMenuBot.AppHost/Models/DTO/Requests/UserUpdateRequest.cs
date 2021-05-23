using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CaffeMenuBot.AppHost.Models.DTO.Responses
{
    public sealed record UserUpdateRequest
    {
        public string Id { get; init; } = null!;
        public string? UserName { get; init; } = null;
        public IFormFile? ProfilePhoto { get; init; } = null;
    }

    public sealed class UserUpdateRequestSchema : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            schema.Example = new OpenApiObject
            {
                ["id"] = new OpenApiString("a18be9c0-aa65-4af8-bd17-00bd9344e575"),
                ["userName"] = new OpenApiString("admin"),
                ["ProfilePhoto"] = new OpenApiString("profile photo")
            };
        }
    }
}