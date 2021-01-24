using CaffeMenuBot.AppHost.Authentication;
using CaffeMenuBot.Data.Models.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;
using CaffeMenuBot.Shared.Models.Auth;

namespace CaffeMenuBot.AppHost.Controllers
{
    [Route("auth")]
    [AllowAnonymous]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseModel>> Login([FromBody] LoginRequestModel model, CancellationToken ct)
        {
            ApplicationUser? user = await _authService.AuthenticateUserAsync(model.Email, model.Password, ct);

            if (user == null)
                return NotFound();

            return new LoginResponseModel
            {
                Token = _authService.GenerateJwtToken(user)
            };
        }
    }
}
