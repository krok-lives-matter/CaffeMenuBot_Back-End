using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Threading;
using CaffeMenuBot.AppHost.Models.DTO.Responses;

namespace CaffeMenuBot.AppHost.Controllers
{
    [Route("api/system")]
    [Authorize(Roles = "admin")]
    [ApiController]
    public class SystemMonitorController : ControllerBase
    {
        [HttpGet]
        [Route("loadavg")]
        [SwaggerOperation("Load average figures giving the number of jobs in the run queue (state R) or waiting for disk I/O (state D) averaged over 1, 5, and 15 minutes.",
            Tags = new[] { "System Monitor, Administration rights are required." })]
        [SwaggerResponse(200, "Successfully got system load info", typeof(LoadAvgResponse))]
        [SwaggerResponse(500, "Internal server error.")]
        public ActionResult GetLoadInfo(CancellationToken ct)
        {
            try
            {
                // run shell command and split by space char
                string[] totalInfo = "cat /proc/loadavg".Bash().Split();

                // get loadavg 1, 5, 15 min and map to 0-100 range
                double oneMin = double.Parse(totalInfo[0]) * 100;
                double fiveMin = double.Parse(totalInfo[1]) * 100;
                double fiveTeenMin = double.Parse(totalInfo[2]) * 100;

                var loadAvgResponse = new LoadAvgResponse()
                {
                    LoadAvg1Min = oneMin,
                    LoadAvg5Min = fiveMin,
                    LoadAvg15Min = fiveTeenMin,
                };

                return Ok(loadAvgResponse);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}