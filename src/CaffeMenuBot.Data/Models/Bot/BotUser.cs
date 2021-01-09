using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace CaffeMenuBot.Data.Models.Bot
{
    [Table("bot_users", Schema = "public")]
    public sealed record BotUser
    {
        [Key, Required, Column("user_id", TypeName = "integer")]
        public int Id { get; init; }
        [Column("user_phone", TypeName = "text")]
        #nullable enable
        public string? PhoneNumber { get; init; }
    }
}
