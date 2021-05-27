using CaffeMenuBot.Bot.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CaffeMenuBot.AppHost.Controllers
{
    [Route("api/bot")]
    [Authorize(Roles = "admin")]
    [ApiController]
    public class BotController : ControllerBase
    {
        private readonly BotHandlerService _botService;
        public BotController(BotHandlerService botService)
        {
            _botService = botService;
        }

        [HttpGet]
        [Route("stop")]
        [SwaggerOperation("Restarts the bot service",
            Tags = new[] { "Bot, Administration rights are required." })]
        [SwaggerResponse(200, "Restart bot started")]
        [SwaggerResponse(500, "Internal server error.")]
        public async Task<ActionResult> Stop(CancellationToken ct)
        {
            var user = HttpContext.User;
            try
            {
                await _botService.StopAsync(ct);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok();
        }

        [HttpGet]
        [Route("start")]
        [SwaggerOperation("Starts the bot service",
            Tags = new[] { "Bot, Administration rights are required." })]
        [SwaggerResponse(200, "Started bot")]
        [SwaggerResponse(500, "Internal server error.")]
        public async Task<ActionResult> Start(CancellationToken ct)
        {
            var user = HttpContext.User;
            try
            {
                await _botService.StartAsync(ct);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok();
        }

        [HttpGet]
        [Route("restart")]
        [SwaggerOperation("Restarts the bot service",
            Tags = new[] { "Bot, Administration rights are required." })]
        [SwaggerResponse(200, "Restart bot started")]
        [SwaggerResponse(500, "Internal server error.")]
        public async Task<ActionResult> Restart(CancellationToken ct)
        {
            try
            {
                await _botService.StopAsync(ct);
                await _botService.StartAsync(ct);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok();
        }

    }
}
