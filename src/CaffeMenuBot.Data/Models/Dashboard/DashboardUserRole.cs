using Microsoft.AspNetCore.Identity;

namespace CaffeMenuBot.Data.Models.Dashboard
{
    public class DashboardUserRole : IdentityUserRole<string>
    {
        public virtual DashboardUser User { get; set; } = null!;

        public virtual IdentityRole Role { get; set; } = null!;
    }
}
