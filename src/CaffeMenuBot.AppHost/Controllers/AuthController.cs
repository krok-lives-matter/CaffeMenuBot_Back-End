using CaffeMenuBot.AppHost.Authentication;
using CaffeMenuBot.AppHost.Models;
using CaffeMenuBot.Data.Models.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;
using CaffeMenuBot.AppHost.Filters;
using Microsoft.AspNetCore.Http;
using System;

namespace CaffeMenuBot.AppHost.Controllers
{
    [Route("auth")]
    [AllowAnonymous]
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        [ValidateModel]
        public async Task<ActionResult> LoginPage([FromForm] LoginModel logInModel, CancellationToken ct)
        {
            ApplicationUser? user = await _authService.AuthenticateUserAsync(logInModel.Email, logInModel.Password, ct);

            if (user == null)
                return RedirectToAction("Auth");

            string token = _authService.GenerateJwtToken(user);
            Response.Cookies.Append("auth", token, new CookieOptions
            {
                MaxAge = TimeSpan.FromMinutes(30d),
                SameSite = SameSiteMode.Lax,
                IsEssential = true
            });

            return Ok("Login Successful");
        }

        [HttpGet]
        public ActionResult Auth()
        {
            return View();
        }
    }
}
