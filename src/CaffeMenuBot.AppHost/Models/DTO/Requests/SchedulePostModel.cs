using System.ComponentModel.DataAnnotations;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CaffeMenuBot.AppHost.Models.DTO.Requests
{
    public sealed record SchedulePostModel
    {
        [Required]
        public int OrderIndex{get;init;} = 1;

        [Required(AllowEmptyStrings = false)]
        public string WeekdayName { get; init; } = null!;

        [Required(AllowEmptyStrings = false)]
        public string OpenTime {get;init;} = null!;

        [Required(AllowEmptyStrings = false)]
        public string CloseTime {get;init;} = null!;
    }

    public sealed class SchedulePostModelSchema : ISchemaFilter
    {
        public IOpenApiAny SchemaExample => new OpenApiObject
        {
            ["weekdayName"] = new OpenApiString("Tuesday"),
            ["openTime"] = new OpenApiString("9:00"),
            ["closeTime"] = new OpenApiString("17:00"),
            ["orderIndex"] = new OpenApiInteger(1)
        };
        
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            schema.Example = SchemaExample;
        }
    }
}