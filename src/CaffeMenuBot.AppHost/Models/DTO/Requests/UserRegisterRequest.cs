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
        // validation fro default identity policy
        [
            Required,
            RegularExpression
                (
                    @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[#$^+=!*()@%&]).{8,}$",
                    ErrorMessage = "password should contain an uppercase character, lowercase character, a digit, and a non-alphanumeric character. Password must be at least six characters long."
                )
        ]
        public string Password { get; init; } = null!;
        [EmailAddress]
        public string? NormalizedEmail { get; init; }
        [Required(AllowEmptyStrings = false)]
        public string UserName { get; init; } = null!;
        public string? Roles {get;init;} = null!;
    }

    public sealed class UserRegisterRequestSchema : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            schema.Example = new OpenApiObject
            {
                ["email"] = new OpenApiString("root@caffemenubot.com"),
                ["password"] = new OpenApiString("password here"),
                ["normalizedEmail"] = new OpenApiString("root@ca...com"),
                ["userName"] = new OpenApiString("root"),
                ["roles"] = new OpenApiString("root,admin"),
            };
        }
    }
}