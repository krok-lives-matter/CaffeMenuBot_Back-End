using System.ComponentModel.DataAnnotations;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CaffeMenuBot.AppHost.Models.DTO.Requests
{
    [SwaggerSchemaFilter(typeof(UserRegisterRequestSchema))]
    public sealed record UserRegisterRequest
    {
        [Required, EmailAddress]
        public string Email { get; init; } = null!;
        [Required, StringLength(32, MinimumLength = 8)]
        public string Password { get; init; } = null!;
        [EmailAddress]
        public string? NormalizedEmail { get; init; }
        [Required(AllowEmptyStrings = false)]
        public string UserName { get; init; } = null!;
    }

    public sealed class UserRegisterRequestSchema : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            schema.Example = new OpenApiObject
            {
                ["email"] = new OpenApiString("admin@caffemenubot.com"),
                ["password"] = new OpenApiString("password here"),
                ["normalizedEmail"] = new OpenApiString("admin@ca...com"),
                ["userName"] = new OpenApiString("admin")
            };
        }
    }
}