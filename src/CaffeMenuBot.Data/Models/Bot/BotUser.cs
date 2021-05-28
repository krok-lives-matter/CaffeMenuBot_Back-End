using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace CaffeMenuBot.Data.Models.Bot
{
    [Table("bot_users", Schema = CaffeMenuBotContext.SchemaName)]
    public record BotUser
    {
        [Key, Required, Column("user_id", TypeName = "bigint")]
        public long Id { get; init; }
        [Column("user_chat_state")]
        public ChatState State { get; set; } = ChatState.default_state;
    }
}
