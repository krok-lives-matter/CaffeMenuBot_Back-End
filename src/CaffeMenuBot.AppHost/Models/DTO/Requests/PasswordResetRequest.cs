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
        [Required(AllowEmptyStrings = false)]
        public string UserId { get; init; } = null!;
        // validation fro default identity policy
        [Required, StringLength(64, MinimumLength = 4)]
        public string NewPassword { get; init; } = null!;
    }

    public sealed class PasswordResetRequestSchema : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            schema.Example = new OpenApiObject
            {
                ["userId"] = new OpenApiString("a18be9c0-aa65-4af8-bd17-00bd9344e575"),
                ["newPassword"] = new OpenApiString("MyN3wP@ssw0rd")
            };
        }
    }
}