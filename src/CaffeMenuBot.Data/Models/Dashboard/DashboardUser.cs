using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CaffeMenuBot.Data.Models.Dashboard
{
    public class DashboardUser : IdentityUser
    {
        [Required, Column("profile_photo_filename", TypeName = "text")]
        public string ProfilePhotoFileName { get; set; } = "blank.jpg";
    }
}
