using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace CaffeMenuBot.Data.Models
{
    [Table("dishes", Schema = "public")]
    public sealed class Dish{

        [Key, Required, Column("dish_id", TypeName = "integer")]
        public int Id { get; set; }

        [Required, Column("dish_name", TypeName = "text")]
        public string DishName { get; set; }

        [Required, Column("description", TypeName = "text")]
        public string Description { get; set; }

        [Required, Column("serving", TypeName = "text")]
        public string Serving { get; set; }

        [Required, Column("price", TypeName = "decimal(5, 2)")]
        public decimal Price { get; set; }

        public int CategoryId {get;set;}   
        public Category Category{get;set;}

    }
}