using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CaffeMenuBot.AppHost.Models.DTO.Responses
{
    public sealed record UserResponse
    {
        public string Id { get; init; } = null!;
        public string Email { get; init; } = null!;
        public string UserName { get; init; } = null!;
        public string ProfilePhotoUrl { get; init; } = null!;
    }

    public sealed class UserResponseSchema : ISchemaFilter
    {
        public IOpenApiAny SchemaExample => new OpenApiObject
        {
            ["id"] = new OpenApiString("141c164d-7dcf-4145-95e5-aff46266cac9"),
            ["email"] = new OpenApiString("admin@ca...com"),
            ["userName"] = new OpenApiString("admin"),
            ["profilePhotoUrl"] = new OpenApiString("/media/profile_photos/blank.jpg")
        };
        
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            schema.Example = SchemaExample;
        }
    }
}