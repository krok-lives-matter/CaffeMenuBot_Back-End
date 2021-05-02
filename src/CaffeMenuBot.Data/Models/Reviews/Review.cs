using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CaffeMenuBot.Data.Models.Bot;

namespace CaffeMenuBot.Data.Models.Reviews
{
    [Table("reviews", Schema = CaffeMenuBotContext.SchemaName)]
    public sealed record Review
    {
        [Key, Required, Column("review_id", TypeName = "integer")]
        public int Id { get; init; }

        // for 5-start-like rating, can be changed later by user
        [Required, Column("rating", TypeName = "smallint")]
        public byte Rating {get;set;}

        [Column("review_comment", TypeName = "text")]
        public string ReviewComment { get; init; } = null!;

        [Required, Column("bot_user")]
        public BotUser User {get;init;} = null!;
    }
}