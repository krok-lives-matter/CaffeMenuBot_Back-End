using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CaffeMenuBot.Data;
using CaffeMenuBot.Data.Models.Schedule;
using Microsoft.EntityFrameworkCore;
using CaffeMenuBot.AppHost.Models;

namespace CaffeMenuBot.AppHost.Controllers
{
    [ApiController]
    [Route("api/schedule")]
    [Authorize]
    public class ScheduleApiController : ControllerBase
    {
        private readonly CaffeMenuBotContext _context;
        
        public ScheduleApiController(CaffeMenuBotContext context)
        {
            _context = context;
        }

        //GET api/schedule/
        [HttpGet]
        [AllowAnonymous] // for bot to be able to request schedule
        public async Task<ActionResult<IEnumerable<Schedule>>> Get(CancellationToken cancellationToken)
        {
            var schedule = await _context
                .Schedule
                .AsNoTracking()
                .ToListAsync(cancellationToken);
            return schedule;
        }

        //DELETE api/schedule/{id}
        [HttpDelete("{id:int:min(1)}")]
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

        //POST api/schedule/
        [HttpPost]
        public async Task<ActionResult<CreatedItemResult>> Post([FromBody] Schedule schedule,
            CancellationToken cancellationToken)
        {
            // ReSharper disable once MethodHasAsyncOverloadWithCancellation
            _context.Schedule.Add(schedule);
            await _context.SaveChangesAsync(cancellationToken);

            return new CreatedItemResult {CreatedItemId = schedule.Id};
        }

        //PUT api/schedule/
        [HttpPut]
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
