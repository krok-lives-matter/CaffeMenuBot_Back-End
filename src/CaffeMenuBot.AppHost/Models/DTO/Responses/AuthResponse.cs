using System.Collections.Generic;
using System.Text.Json.Serialization;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CaffeMenuBot.AppHost.Models.DTO.Responses
{
    [SwaggerSchemaFilter(typeof(AuthResponseSchema))]
    public record AuthResponse
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Token { get; init; } = null!;
        public bool Result { get; init; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<string> Errors { get; init; } = null!;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public UserResponse User { get; init; } = null!;
    }

    public sealed class AuthResponseSchema : ISchemaFilter
    {
        public IOpenApiAny SchemaExample => new OpenApiObject
        {
            ["token"] = new OpenApiString(
                "eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJJZCI6IjE0MWMxNjRkLTdkY2YtNDE0NS05NWU1LWFmZjQ2MjY2Y2FjOSIsInN1YiI6ImFkbWluQGNhZmZlbWVudWJvdC5jb20iLCJlbWFpbCI6ImFkbWluQGNhZmZlbWVudWJvdC5jb20iLCJqdGkiOiIwYzk2ZjNlNS1mNGU3LTQyMTEtYWU5Ny1jNjZjMjMzNWVlYTEiLCJuYmYiOjE2MjEwODAxNTEsImV4cCI6MTYyMTEwMTc1MSwiaWF0IjoxNjIxMDgwMTUxfQ.Yc1tRq4NoK2qxixXEquFXiYTsEVBLRrciPGRqqanYA3eY4JEu0waO8YmaHJwwhSmZSwwVIgnbbRX7jzdxQkwtw"),
            ["result"] = new OpenApiBoolean(true),
            ["user"] = new UserResponseSchema().SchemaExample
        };
        
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            schema.Example = SchemaExample;
        }
    }
}