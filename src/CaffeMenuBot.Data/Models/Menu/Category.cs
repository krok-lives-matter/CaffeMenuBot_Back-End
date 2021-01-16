using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CaffeMenuBot.Data.Models.Menu
{
    [Table("categories", Schema = CaffeMenuBotContext.SchemaName)]
    public sealed record Category
    {
        [Key, Required, Column("category_id", TypeName = "integer")]
        public int Id { get; init; }

        [Required, Column("category_name", TypeName = "text")]
        public string CategoryName { get; init; } = null!;

        public int? ParentCategoryId { get; init; }
        public Category? ParentCategory { get; init; }
        public IEnumerable<Category>? SubCategories { get; set; }

        public List<Dish>? Dishes { get; init; } = null!;
    }
}