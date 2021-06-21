using System.ComponentModel.DataAnnotations;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CaffeMenuBot.AppHost.Models.DTO.Requests
{
    public sealed record SchedulePutModel
    {
        [Required, Range(1, int.MaxValue)]
        public int Id { get; init; }

        [Required]
        public int OrderIndex{get;init;} = 1;

        [Required(AllowEmptyStrings = false)]
        public string WeekdayName { get; init; } = null!;

        [Required(AllowEmptyStrings = false)]
        public string OpenTime {get;init;} = null!;

        [Required(AllowEmptyStrings = false)]
        public string CloseTime {get;init;} = null!;
    }

    public sealed class SchedulePutModelSchema : ISchemaFilter
    {
        public IOpenApiAny SchemaExample
        {
            get
            {
                var schema = (OpenApiObject) new SchedulePostModelSchema().SchemaExample;
                schema["id"] = new OpenApiInteger(1);
                return schema;
            }
        }
        
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            schema.Example = SchemaExample;
        }
    }
}