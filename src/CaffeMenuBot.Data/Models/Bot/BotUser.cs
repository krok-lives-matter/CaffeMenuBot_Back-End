using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace CaffeMenuBot.Data.Models.Bot
{
    [Table("bot_users", Schema = CaffeMenuBotContext.SchemaName)]
    public sealed record BotUser
    {
        [Key, Required, Column("user_id", TypeName = "integer")]
        public int Id { get; init; }
        [Column("user_chat_state")]
        public ChatState State { get; set; } = ChatState.default_state;
    }
}
