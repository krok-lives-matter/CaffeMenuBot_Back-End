using CaffeMenuBot.Bot.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Threading;
using System.Threading.Tasks;
using CaffeMenuBot.AppHost.Models.DTO.Responses;

namespace CaffeMenuBot.AppHost.Controllers
{
    [Route("api/bot")]
    [Authorize(Roles = "root,admin")]
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
        [SwaggerOperation("Gets bot status (root or admin required)",
            Tags = new[] { "Bot"})]
        [SwaggerResponse(401, "User unauthorized.")]
        [SwaggerResponse(403,"Role not allowed.")]
        [SwaggerResponse(200, "Status of bot.", typeof(StatusResponse))]
        [SwaggerResponse(500, "Internal server error.")]
        public ActionResult<StatusResponse> Status()
        {
            return new StatusResponse
            {
                 IsRunning = _botService.IsBotRunning()
            };
        }

        [HttpPost]
        [Route("stop")]
        [SwaggerOperation("Restarts the bot service (root or admin required)",
            Tags = new[] { "Bot" })]
        [SwaggerResponse(401, "User unauthorized.")]
        [SwaggerResponse(403, "Role not allowed.")]
        [SwaggerResponse(200, "Successfully stopped the bot.")]
        [SwaggerResponse(400, "Bot already stopped.")]
        [SwaggerResponse(500, "Internal server error.")]
        public async Task<ActionResult> Stop(CancellationToken ct)
        {
            try
            {
                if(_botService.IsBotRunning() == false)
                    return BadRequest("Bot already stopped");

                await _botService.StopAsync(ct);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok();
        }

        [HttpPost]
        [Route("start")]
        [SwaggerOperation("Starts the bot service (root or admin required)",
            Tags = new[] { "Bot" })]
        [SwaggerResponse(401, "User unauthorized.")]
        [SwaggerResponse(403, "Role not allowed.")]
        [SwaggerResponse(200, "Successfully started the bot.")]
        [SwaggerResponse(400, "Bot already started.")]
        [SwaggerResponse(500, "Internal server error.")]
        public async Task<ActionResult> Start(CancellationToken ct)
        {
            var user = HttpContext.User;
            try
            {
                if(_botService.IsBotRunning())
                    return BadRequest("Bot already runing");

                await _botService.StartAsync(ct);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok();
        }

        [HttpPost]
        [Route("restart")]
        [SwaggerOperation("Restarts the bot service (root or admin required)",
            Tags = new[] { "Bot" })]
        [SwaggerResponse(401, "User unauthorized.")]
        [SwaggerResponse(403, "Role not allowed.")]
        [SwaggerResponse(200, "Successfully restarted.")]
        [SwaggerResponse(500, "Internal server error.")]
        public async Task<ActionResult> Restart(CancellationToken ct)
        {
            try
            {
                if(_botService.IsBotRunning())
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
