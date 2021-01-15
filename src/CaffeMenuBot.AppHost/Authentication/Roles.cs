using Microsoft.AspNetCore.Authorization;

namespace CaffeMenuBot.AppHost.Authentication
{
    internal static class Roles
    {

        public const string Admin = "admin";

        public static AuthorizationPolicy AdminPolicy() =>
            new AuthorizationPolicyBuilder().RequireAuthenticatedUser().RequireRole(Admin).Build();
    }
}
