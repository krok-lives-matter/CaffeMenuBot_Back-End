using System.ComponentModel.DataAnnotations;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CaffeMenuBot.AppHost.Models.DTO.Requests
{
    [SwaggerSchemaFilter(typeof(UserLoginRequestSchema))]
    public sealed record UserLoginRequest
    {
        [Required, EmailAddress]
        public string Email { get; init; } = null!;
        [Required, StringLength(32, MinimumLength = 8)]
        public string Password { get; init; } = null!;
    }

    public sealed class UserLoginRequestSchema : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            schema.Example = new OpenApiObject
            {
                ["email"] = new OpenApiString("admin@caffemenubot.com"),
                ["password"] = new OpenApiString("password here")
            };
        }
    }
}