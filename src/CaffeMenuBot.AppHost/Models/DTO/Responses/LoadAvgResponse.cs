using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CaffeMenuBot.AppHost.Models.DTO.Responses
{
    [SwaggerSchemaFilter(typeof(LoadAvgResponseSchema))]
    public record LoadAvgResponse
    {
        public double LoadAvg1Min {get;init;} 
        public double LoadAvg5Min {get;init;} 
        public double LoadAvg15Min {get;init;} 
    }
    public sealed class LoadAvgResponseSchema : ISchemaFilter
    {
        public IOpenApiAny SchemaExample => new OpenApiObject
        {
            ["loadAvg1Min"] = new OpenApiFloat(20.5f),
            ["loadAvg5Min"] = new OpenApiFloat(105.5f),
            ["loadAvg15Min"] = new OpenApiFloat(93.5f)
        };
        
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            schema.Example = SchemaExample;
        }
    }
}