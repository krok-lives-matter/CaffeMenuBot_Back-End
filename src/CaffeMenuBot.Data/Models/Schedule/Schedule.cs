using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CaffeMenuBot.Data.Models.Schedule
{
    [Table("schedule", Schema = CaffeMenuBotContext.SchemaName)]
    public record Schedule
    {
        [Key, Required, Column("schedule_id", TypeName = "integer")]
        public int Id { get; init; }

        [Required, Column("order_index", TypeName = "integer")]
        public int OrderIndex{get;set;} = 1;

        [Required, Column("weekday_name", TypeName = "text")]
        public string WeekdayName { get; set; } = null!;

        [Required, Column("open_time", TypeName = "text")]
        public string OpenTime {get;set;} = null!;

        [Required, Column("close_time", TypeName = "text")]
        public string CloseTime {get;set;} = null!;
    }
}