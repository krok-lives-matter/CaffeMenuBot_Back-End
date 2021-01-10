using Microsoft.AspNetCore.Mvc;

namespace CaffeMenuBot.AppHost.Controllers
{
    [Route("auth")]
    public class AuthController : Controller
    {
      
        [HttpGet("login")]
        public ActionResult LoginPage()
        {
            return View();
        }
    }
}
