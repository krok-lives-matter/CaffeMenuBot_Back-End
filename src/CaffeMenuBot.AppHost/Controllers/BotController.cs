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
        [Route("status")]
        [SwaggerOperation("Gets bot status",
            Tags = new[] { "Bot, Administration rights are required." })]
        [SwaggerResponse(200, "Status of bot (is bot receiving)", typeof(bool))]
        [SwaggerResponse(500, "Internal server error.")]
        public ActionResult<Boolean> Status(CancellationToken ct)
        {
            return Ok(_botService.IsBotRunning());
        }

        [HttpGet]
        [Route("stop")]
        [SwaggerOperation("Restarts the bot service",
            Tags = new[] { "Bot, Administration rights are required." })]
        [SwaggerResponse(200, "Status of bot (is bot receiving)", typeof(bool))]
        [SwaggerResponse(500, "Internal server error.")]
        public async Task<ActionResult<Boolean>> Stop(CancellationToken ct)
        {
            try
            {
                await _botService.StopAsync(ct);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok(_botService.IsBotRunning());
        }

        [HttpGet]
        [Route("start")]
        [SwaggerOperation("Starts the bot service",
            Tags = new[] { "Bot, Administration rights are required." })]
        [SwaggerResponse(200, "Status of bot (is bot receiving)", typeof(bool))]
        [SwaggerResponse(500, "Internal server error.")]
        public async Task<ActionResult<Boolean>> Start(CancellationToken ct)
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
            return Ok(_botService.IsBotRunning());
        }

        [HttpGet]
        [Route("restart")]
        [SwaggerOperation("Restarts the bot service",
            Tags = new[] { "Bot, Administration rights are required." })]
        [SwaggerResponse(200, "Status of bot (is bot receiving)", typeof(bool))]
        [SwaggerResponse(500, "Internal server error.")]
        public async Task<ActionResult<Boolean>> Restart(CancellationToken ct)
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
            return Ok(_botService.IsBotRunning());
        }

    }
}
