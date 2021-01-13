using CaffeMenuBot.Data.Models;
using CaffeMenuBot.Data.Models.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CaffeMenuBot.AppHost.Authentication
{
    public interface IAuthService
    {
        ValueTask<ApplicationUser?> AuthenticateUserAsync(string username, string password, CancellationToken ct);

        string GenerateJwtToken(ApplicationUser userInfo);
    }
}
