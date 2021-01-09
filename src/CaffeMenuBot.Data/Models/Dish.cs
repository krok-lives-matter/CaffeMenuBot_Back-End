using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CaffeMenuBot.Data.Models
{
    [Table("dishes", Schema = "public")]
    public sealed record Dish
    {
        [Key, Required, Column("dish_id", TypeName = "integer")]
        public int Id { get; init; }

        [Required, Column("dish_name", TypeName = "text")]
        public string DishName { get; init; }

        [Required, Column("description", TypeName = "text")]
        public string Description { get; init; }

        [Required, Column("serving", TypeName = "text")]
        public string Serving { get; init; }

        [Required, Column("price", TypeName = "decimal(5, 2)")]
        public decimal Price { get; init; }

        public int CategoryId { get; init; }
        public Category Category { get; init; }
    }
}