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
using CaffeMenuBot.AppHost.Models.DTO.Requests;
using Swashbuckle.AspNetCore.Annotations;

namespace CaffeMenuBot.AppHost.Controllers
{
    [ApiController]
    [Route("api/dashboard/schedule")]
    [Authorize]
    public sealed class ScheduleApiController : ControllerBase
    {
        private readonly CaffeMenuBotContext _context;
        
        public ScheduleApiController(CaffeMenuBotContext context)
        {
            _context = context;
        }

        //GET api/dashboard/schedule/
        [HttpGet]
        [SwaggerOperation("Gets all schedules ordered by order index ascending", Tags = new[] { "Schedule" })]
        [SwaggerResponse(200, "Successfully fund schedule by specified id", typeof(List<Schedule>))]
        [SwaggerResponse(500, "Internal server error")]
        public async Task<ActionResult<List<Schedule>>> Get(CancellationToken cancellationToken)
        {
            var schedules = await _context.Schedule
                .AsNoTracking()
                .OrderBy(s => s.OrderIndex)
                .ToListAsync(cancellationToken);
            return schedules;
        }

        //GET api/dashboard/schedule/{id}
        [HttpGet("{id:int:min(1)}")]
        [SwaggerOperation("Gets review by id", Tags = new[] { "Schedule" })]
        [SwaggerResponse(200, "Successfully found schedule by specified id", typeof(Schedule))]
        [SwaggerResponse(404, "Schedule was not found by specified id")]
        [SwaggerResponse(500, "Internal server error")]
        public async Task<ActionResult<Schedule>> Get(int id, CancellationToken cancellationToken)
        {
            var schedule = await _context.Schedule
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);

            if (schedule == null)
                return NotFound();

            return schedule;
        }

        //DELETE api/dashboard/schedule/{id}
        [HttpDelete("{id:int:min(1)}")]
        [SwaggerOperation("Deletes schedule by id", Tags = new[] { "Schedule" })]
        [SwaggerResponse(204, "Successfully deleted schedule by specified id")]
        [SwaggerResponse(404, "Schedule was not found by specified id")]
        [SwaggerResponse(500, "Internal server error")]
        public async Task<ActionResult<Schedule>> Delete(int id, CancellationToken cancellationToken)
        {
            var scheduleToDelete = await _context.Schedule
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);

            if (scheduleToDelete == null)
                return NotFound();

            _context.Remove(scheduleToDelete);

            await _context.SaveChangesAsync(cancellationToken);

            return NoContent();
        }

        //POST api/dashboard/schedule/
        [HttpPost]
        [SwaggerOperation("Creates schedule", Tags = new[] { "Schedule" })]
        [SwaggerResponse(200, "Successfully created schedule",
            typeof(CreatedItemResult))]
        [SwaggerResponse(400, "Bad request, bad data was specified")]
        [SwaggerResponse(500, "Internal server error")]
        public async Task<ActionResult<CreatedItemResult>> Post([FromBody] SchedulePostModel schedule,
            CancellationToken cancellationToken)
        {
            var addedEntry = _context.Schedule.Add(new Schedule
            {
                CloseTime = schedule.CloseTime,
                OpenTime = schedule.OpenTime,
                OrderIndex = schedule.OrderIndex,
                WeekdayName = schedule.WeekdayName
            });
            await _context.SaveChangesAsync(cancellationToken);

            return Ok(new CreatedItemResult
            {
                CreatedItemId = addedEntry.Entity.Id
            });
        }

        //PUT api/dashboard/schedule/
        [HttpPut]
        [SwaggerOperation("Updates schedule", Tags = new[] { "Schedule" })]
        [SwaggerResponse(200, "Successfully updated schedule with result of id of updated schedule")]
        [SwaggerResponse(400, "Bad request, bad data was specified")]
        [SwaggerResponse(500, "Internal server error")]
        public async Task<ActionResult> Put([FromBody] SchedulePutModel schedule,
            CancellationToken cancellationToken)
        {
            if (!await _context.Schedule.AnyAsync(s => s.Id == schedule.Id, cancellationToken))
            {
                ModelState.AddModelError(nameof(schedule.Id), "Schedule with specified id was not found");
                return BadRequest(ModelState);
            }
            
            _context.Schedule.Update(new Schedule
            {
                Id = schedule.Id,
                CloseTime = schedule.CloseTime,
                OpenTime = schedule.OpenTime,
                OrderIndex = schedule.OrderIndex,
                WeekdayName = schedule.WeekdayName
            });
            await _context.SaveChangesAsync(cancellationToken);

            return Ok();
        }
    }
}
