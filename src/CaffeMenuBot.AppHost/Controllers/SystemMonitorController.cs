using System.Collections;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Threading;
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
            Tags = new[] { "System Monitor, Administration rights are required." })]
        [SwaggerResponse(401, "User unathorized.")]
        [SwaggerResponse(403, "Role not allowed.")]
        [SwaggerResponse(200, "Successfully got system load info", typeof(Queue))]
        [SwaggerResponse(200, "Statistics not collected yet, empty array", typeof(Queue))]
        [SwaggerResponse(500, "Internal server error.")]
        public ActionResult<Queue> GetLoadInfo(CancellationToken ct)
        {
            try
            {
                Queue? loadAvgInfo = _loadService.GetLoadAvg();
                if(loadAvgInfoeyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJJZCI6ImExOGJlOWMwLWFhNjUtNGFmOC1iZDE3LTAwYmQ5MzQ0ZTU3NSIsIlVzZXJuYW1lIjoiYWRtaW4iLCJFbWFpbCI6ImFkbWluQGNhZmZlbWVudWJvdC5jb20iLCJqdGkiOiIzYTQwMGRkMC0xNmE3LTRkMGQtYjgzMy05MTgwMTdhMjE1YTgiLCJSb2xlcyI6ImFkbWluIiwibmJmIjoxNjIyMjE2NTg2LCJleHAiOjE2MjIyMjczODYsImlhdCI6MTYyMjIxNjU4Nn0.yBUKWGWYQj6nBGFLiWPhJ4Fl-4XBYqbCZ0jBWtQaD8_uVsZ_J5BzjtqaJos05ZrR0ijFNQwH36IJmbDhBYWmVA == null)
                    return Ok(new Queue());
                
                return Ok(loadAvgInfo);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}