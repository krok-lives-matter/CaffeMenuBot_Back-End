using CaffeMenuBot.Data.Models.Authentication;
using System.Threading;
using System.Threading.Tasks;

namespace CaffeMenuBot.AppHost.Authentication
{
    public interface IAuthService
    {
        ValueTask<ApplicationUser?> AuthenticateUserAsync(string email, string password, CancellationToken ct);

        string GenerateJwtToken(ApplicationUser userInfo);
    }
}
