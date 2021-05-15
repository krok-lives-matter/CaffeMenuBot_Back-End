using CaffeMenuBot.AppHost.Models.DTO.Responses;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CaffeMenuBot.AppHost.Models
{
    [SwaggerSchemaFilter(typeof(ErrorResponseSchema))]
    public sealed record ErrorResponse : AuthResponse
    {
    }

    public sealed class ErrorResponseSchema : ISchemaFilter
    {
        public IOpenApiAny SchemaExample => new OpenApiObject
        {
            ["result"] = new OpenApiBoolean(false),
            ["errors"] = new OpenApiArray
            {
                new OpenApiString("Description of any error that has occurred")
            }
        };
        
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            schema.Example = SchemaExample;
        }
    }
}