using Microsoft.AspNetCore.Identity;

namespace CaffeMenuBot.Data.Models.Dashboard
{
    public class DashboardUserRole : IdentityUserRole<string>
    {
        public DashboardUser User { get; set; } = null!;

        public IdentityRole Role { get; set; } = null!;
    }
}
