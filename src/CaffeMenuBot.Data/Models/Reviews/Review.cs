using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CaffeMenuBot.Data.Models.Bot;

namespace CaffeMenuBot.Data.Models.Reviews
{
    [Table("reviews", Schema = CaffeMenuBotContext.SchemaName)]
    public record Review
    {
        [Key, Required, Column("review_id", TypeName = "integer")]
        public int Id { get; init; }

        [Column("rating")]
        public Rating Rating { get; set; } = Rating.rating_unrated;

        [Column("review_comment", TypeName = "text")]
        public string ReviewComment { get; set; } = null!;

        [Column("bot_user")]
        public BotUser? User {get;init;}

        [Required, Column("bot_user_id")]
        public long UserId {get;init;}
    }
}