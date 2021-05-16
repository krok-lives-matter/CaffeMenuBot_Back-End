using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CaffeMenuBot.Data;
using CaffeMenuBot.Data.Models.Schedule;
using Microsoft.EntityFrameworkCore;
using CaffeMenuBot.AppHost.Models;
using System.Linq;
using Swashbuckle.AspNetCore.Annotations;

namespace CaffeMenuBot.AppHost.Controllers
{
    [ApiController]
    [Route("api/dashboard/schedule")]
    [Authorize]
    public class ScheduleApiController : ControllerBase
    {
        private readonly CaffeMenuBotContext _context;
        
        public ScheduleApiController(CaffeMenuBotContext context)
        {
            _context = context;
        }

        //GET api/dashboard/schedule/
        [HttpGet]
        [SwaggerOperation("Gets all schedule")]
        public async Task<ActionResult<IEnumerable<Schedule>>> Get(CancellationToken cancellationToken)
        {
            var schedule = await _context
                .Schedule
                .AsNoTracking()
                .OrderBy(s => s.OrderIndex)
                .ToListAsync(cancellationToken);
            return schedule;
        }

        //DELETE api/dashboard/schedule/{id}
        [HttpDelete("{id:int:min(1)}")]
        [SwaggerOperation("Deletes schedule by id")]
        [SwaggerResponse(204, "Successfully schedule dish by specified id")]
        [SwaggerResponse(404, "Schedule was not found by specified id")]
        public async Task<ActionResult<Schedule>> Delete(int id, CancellationToken cancellationToken)
        {
            var scheduleToDelete = await _context
                .Schedule
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);

            if (scheduleToDelete == null)
                return NotFound();

            _context.Remove(scheduleToDelete);

            await _context.SaveChangesAsync();

            return NoContent();
        }

        //POST api/dashboard/schedule/
        [HttpPost]
        [SwaggerOperation("Creates schedule")]
        [SwaggerResponse(200, "Successfully created schedule with result of id of created schedule", typeof(Schedule))]
        [SwaggerResponse(402, "Bad request, bad data was specified")]
        public async Task<ActionResult<CreatedItemResult>> Post([FromBody] Schedule schedule,
            CancellationToken cancellationToken)
        {
            // ReSharper disable once MethodHasAsyncOverloadWithCancellation
            _context.Schedule.Add(schedule);
            await _context.SaveChangesAsync(cancellationToken);

            return new CreatedItemResult {CreatedItemId = schedule.Id};
        }

        //PUT api/dashboard/schedule/
        [HttpPut]
        [SwaggerOperation("Updates schedule")]
        [SwaggerResponse(200, "Successfully updated schedule with result of id of updated schedule", typeof(Schedule))]
        [SwaggerResponse(402, "Bad request, bad data was specified")]
        public async Task<ActionResult<CreatedItemResult>> Put([FromBody] Schedule schedule,
            CancellationToken cancellationToken)
        {
            // ReSharper disable once MethodHasAsyncOverloadWithCancellation
            _context.Schedule.Update(schedule);
            await _context.SaveChangesAsync(cancellationToken);

            return new CreatedItemResult { CreatedItemId = schedule.Id };
        }

    }
}
