using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CaffeMenuBot.AppHost.Controllers
{
    [Route("auth")]
    [AllowAnonymous]
    public class AuthController : Controller
    {
      
        [HttpGet("login")]
        public ActionResult LoginPage()
        {
            return View();
        }
    }
}
