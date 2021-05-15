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

        };
        
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            schema.Example = SchemaExample;
        }
    }
}