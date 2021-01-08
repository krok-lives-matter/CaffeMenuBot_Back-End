using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace CaffeMenuBot.Data.Models
{
    [Table("categories", Schema = "public")]
    public sealed class Category{

        [Key, Required, Column("category_id", TypeName = "integer")]
        public int Id { get; set; }

        [Required, Column("category_name", TypeName = "text")]
        public string CategoryName { get; set; }

        public List<Dish> Dishes {get;set;}   
    }
}