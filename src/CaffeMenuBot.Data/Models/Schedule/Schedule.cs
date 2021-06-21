using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CaffeMenuBot.Data.Models.Schedule
{
    [Table("schedule", Schema = CaffeMenuBotContext.SchemaName)]
    public sealed record Schedule
    {
        [Key, Required, Column("schedule_id", TypeName = "integer")]
        public int Id { get; init; }

        [Required, Column("order_index", TypeName = "integer")]
        public int OrderIndex{get;init;} = 1;

        [Required, Column("weekday_name", TypeName = "text")]
        public string WeekdayName { get; init; } = null!;

        [Required, Column("open_time", TypeName = "text")]
        public string OpenTime {get;init;} = null!;

        [Required, Column("close_time", TypeName = "text")]
        public string CloseTime {get;init;} = null!;
    }
}