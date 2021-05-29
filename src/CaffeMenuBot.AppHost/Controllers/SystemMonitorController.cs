using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Threading;
using CaffeMenuBot.AppHost.Models.DTO.Responses;
using CaffeMenuBot.AppHost.Services;

namespace CaffeMenuBot.AppHost.Controllers
{
    [Route("api/system")]
    [Authorize(Roles = "root,admin")]
    [ApiController]
    public class SystemMonitorController : ControllerBase
    {
        private readonly LoadAvgBackgroundService _loadService;
        public SystemMonitorController(LoadAvgBackgroundService loadService)
        {
            _loadService = loadService;
        }
        [HttpGet]
        [Route("loadavg")]
        [SwaggerOperation("Load average figures giving the number of jobs in the run queue (state R) or waiting for disk I/O (state D) averaged over 1, 5, and 15 minutes.",
            Tags = new[] { "System Monitor (root or admin required)." })]
        [SwaggerResponse(401, "User unathorized.")]
        [SwaggerResponse(403, "Role not allowed.")]
        [SwaggerResponse(200, "Successfully got system load info", typeof(LoadInfoResponse))]
        [SwaggerResponse(200, "Statistics not collected yet, empty response")]
        [SwaggerResponse(500, "Internal server error.")]
        public ActionResult<LoadInfoResponse> GetLoadInfo(CancellationToken ct)
        {
            try
            {
                var loadAvgInfo = _loadService.GetLoadAvg()?.ToArray();
                if(loadAvgInfo is null)
                    return Ok();
                
                return Ok(new LoadInfoResponse
                {
                    Avg1 = loadAvgInfo[0],
                    Avg2 = loadAvgInfo[1],
                    Avg3 = loadAvgInfo[2],
                    Avg4 = loadAvgInfo[3],
                    Avg5 = loadAvgInfo[4]
                });
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}