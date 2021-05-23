using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;

namespace CaffeMenuBot.Data.Models.Dashboard
{
    public sealed class DashboardUser : IdentityUser
    {
        // used to save profile photos of user while adding new user or updating him
        private const string MEDIA_SUBFOLDER = "/media/profile_photos";

        private string profilePhotoRelativeUrl = Path.Combine(MEDIA_SUBFOLDER, "blank.jpg");

        [Required, Column("profile_photo_relative_url", TypeName = "text")]
        public string ProfilePhotoRelativeUrl
        {
            get { return profilePhotoRelativeUrl; }
            set { profilePhotoRelativeUrl = Path.Combine(MEDIA_SUBFOLDER, value); }
        }
    }
}
