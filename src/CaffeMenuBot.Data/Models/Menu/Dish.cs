using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CaffeMenuBot.Data.Models.Menu
{
    [Table("dishes", Schema = CaffeMenuBotContext.SchemaName)]
    public sealed record Dish
    {
        [Key, Required, Column("dish_id", TypeName = "integer")]
        public int Id { get; init; }

        [Required, Column("dish_name", TypeName = "text")]
        public string DishName { get; init; } = null!;

        [Required, Column("description", TypeName = "text")]
        public string Description { get; init; } = null!;

        [Required, Column("serving", TypeName = "text")]
        public string Serving { get; init; } = null!;

        [Required, Column("price", TypeName = "decimal(5, 2)")]
        public decimal Price { get; init; }

        [Required, Column("photo_url", TypeName = "text")]
        public string PhotoUrl {get;set;} = "https://thumbs.dreamstime.com/b/no-image-available-icon-flat-vector-no-image-available-icon-flat-vector-illustration-132484032.jpg";

        public int CategoryId { get; init; }
        [JsonIgnore]
        public Category? Category { get; init; } = null!;
    }
}