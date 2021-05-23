using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;

namespace CaffeMenuBot.Data.Models.Menu
{
    [Table("categories", Schema = CaffeMenuBotContext.SchemaName)]
    public sealed record Category
    {
        // used to save images of category covers while adding new category or updating it
        private const string MEDIA_SUBFOLDER = "/media/category_covers";

        [Key, Required, Column("category_id", TypeName = "integer")]
        public int Id { get; init; }

        [Required, Column("category_name", TypeName = "text")]
        public string CategoryName { get; set; } = null!;


        private string coverPhotoRelativeUrl = Path.Combine(MEDIA_SUBFOLDER, "blank.jpg");

        [Required, Column("cover_photo_relative_url", TypeName = "text")]
        public string CoverPhotoRelativeUrl
        {
            get { return coverPhotoRelativeUrl; }
            set { coverPhotoRelativeUrl = Path.Combine(MEDIA_SUBFOLDER, value); }
        }

        [Required, Column("is_visible", TypeName = "boolean")]
        public bool IsVisible { get; set; } = true;

        public List<Dish>? Dishes { get; set; }
    }
}