using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CaffeMenuBot.Data.Models.Menu
{
    [Table("dishes", Schema = CaffeMenuBotContext.SchemaName)]
    public record Dish
    {
        [Key, Required, Column("dish_id", TypeName = "integer")]
        public int Id { get; init; }

        [Required, Column("dish_name", TypeName = "text")]
        public string DishName { get; set; } = null!;

        [Required, Column("description", TypeName = "text")]
        public string Description { get; set; } = null!;

        [Required, Column("serving", TypeName = "text")]
        public string Serving { get; set; } = null!;

        [Required, Range(1, 9999), Column("price", TypeName = "decimal(6, 2)")]
        public decimal Price { get; set; }    

        public int CategoryId { get; set; }
        [JsonIgnore]
        public Category? Category { get; init; } = null!;
    }
}