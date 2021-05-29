using System;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace CaffeMenuBot.AppHost.Controllers
{
    [ApiController]
    public class ErrorController : ControllerBase
    {
        [Route("/error-local-development")]
        [HttpGet]
        public IActionResult ErrorLocalDevelopment(
            [FromServices] IWebHostEnvironment webHostEnvironment)
        {
            if (webHostEnvironment.EnvironmentName != "Development")
            {
                throw new InvalidOperationException(
                    "This shouldn't be invoked in non-development environments.");
            }

            var context = HttpContext.Features.Get<IExceptionHandlerFeature>();

            return Problem(
                detail: context.Error.StackTrace,
                title: context.Error.Message);
        }

        [Route("/error")]
        [HttpGet]
        public IActionResult Error() => Problem();
    }
}