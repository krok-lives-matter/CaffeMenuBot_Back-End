using Microsoft.AspNetCore.Mvc;

namespace CaffeMenuBot.AppHost.Controllers
{
    [ApiController]
    [Route("api/test")]
    [AllowAnonymous]
    public sealed class TestController : ControllerBase
    {
        [HttpGet]
        public ActionResult Test()
        {
            return Content("OK");
        }
    }
}