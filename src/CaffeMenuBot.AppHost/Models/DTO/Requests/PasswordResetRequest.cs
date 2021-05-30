using System.Text.RegularExpressions;
using System.ComponentModel.DataAnnotations;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CaffeMenuBot.AppHost.Models.DTO.Requests
{
    [SwaggerSchemaFilter(typeof(PasswordResetRequestSchema))]
    public sealed record PasswordResetRequest
    {
        [Required]
        public string UserId { get; init; } = null!;
        // validation fro default identity policy
        [
            Required,
            RegularExpression
                (
                    @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[#$^+=!*()@%&]).{8,}$",
                    ErrorMessage = "password should contain an uppercase character, lowercase character, a digit, and a non-alphanumeric character. Password must be at least six characters long."
                )
        ]
        public string NewPassword { get; init; } = null!;
    }

    public sealed class PasswordResetRequestSchema : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            schema.Example = new OpenApiObject
            {
                ["UserId"] = new OpenApiString("a18be9c0-aa65-4af8-bd17-00bd9344e575"),
                ["Password"] = new OpenApiString("MyN3wP@ssw0rd")
            };
        }
    }
}