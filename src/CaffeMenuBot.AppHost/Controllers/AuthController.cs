using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
