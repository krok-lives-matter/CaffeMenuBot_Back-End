using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CaffeMenuBot.Data.Models.Dashboard
{
    public class DashboardUser : IdentityUser
    {
        [JsonIgnore]
        [Required, Column("profile_photo_filename", TypeName = "text")]
        public string ProfilePhotoFileName { get; set; } = "blank.jpg";

        [NotMapped]
        public string? ProfilePhotoUrl { get; set; } 

        public virtual ICollection<DashboardUserRole> Roles { get; set; }  = new List<DashboardUserRole>();
    }
}
