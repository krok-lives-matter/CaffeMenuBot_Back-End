using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CaffeMenuBot.Data;
using CaffeMenuBot.Data.Models.Reviews;
using Microsoft.EntityFrameworkCore;
using CaffeMenuBot.AppHost.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace CaffeMenuBot.AppHost.Controllers
{
    [ApiController]
    [Route("api/dashboard/reviews")]
    [Authorize]
    public class ReviewApiController: ControllerBase
    {
        private readonly CaffeMenuBotContext _context;
        
        public ReviewApiController(CaffeMenuBotContext context)
        {
            _context = context;
        }

        //GET api/dashboard/reviews/
        [HttpGet]
        [SwaggerOperation("Gets all reviews", Tags = new[] { "Reviews" })]
        [SwaggerResponse(200, "Successfully found any reviews", typeof(IEnumerable<Review>))]
        public async Task<ActionResult<IEnumerable<Review>>> Get(CancellationToken cancellationToken)
        {
            var reviews = await _context
                .Reviews
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return Ok(reviews);
        }

        //GET api/dashboard/reviews/{id}
        [HttpGet("{id:int:min(1)}")]
        [SwaggerOperation("Gets review by id", Tags = new[] { "Reviews" })]
        [SwaggerResponse(200, "Successfully found review by specified id", typeof(Review))]
        [SwaggerResponse(404, "Review was not found by specified id")]
        public async Task<ActionResult<Review>> Get(int id, CancellationToken cancellationToken)
        {
            var review = await _context
                .Reviews
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
            
            if (review == null)
                return NotFound();
            
            return Ok(review);
        }

        //DELETE api/dashboard/reviews/{id}
        [HttpDelete("{id:int:min(1)}")]
        [SwaggerOperation("Deletes review by id", Tags = new[] { "Reviews" })]
        [SwaggerResponse(204, "Successfully review dish by specified id")]
        [SwaggerResponse(404, "Review was not found by specified id")]
        public async Task<ActionResult<Review>> Delete(int id, CancellationToken cancellationToken)
        {
            var reviewToDelete = await _context
                .Reviews
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);

            if (reviewToDelete == null)
                return NotFound();

            _context.Remove(reviewToDelete);

            await _context.SaveChangesAsync();

            return NoContent();
        }

        //POST api/dashboard/reviews/
        [HttpPost]
        [SwaggerOperation("Creates review", Tags = new[] { "Reviews" })]
        [SwaggerResponse(200, "Successfully created review with result of id of created review", typeof(Review))]
        [SwaggerResponse(402, "Bad request, bad data was specified")]
        public async Task<ActionResult<CreatedItemResult>> Post([FromForm] Review review,
            CancellationToken cancellationToken)
        {
            // ReSharper disable once MethodHasAsyncOverloadWithCancellation
            _context.Reviews.Add(review);
            await _context.SaveChangesAsync(cancellationToken);

            return Ok(review);
        }

        //PUT api/dashboard/reviews/
        [HttpPut]
        [SwaggerOperation("Updates review", Tags = new[] { "Reviews" })]
        [SwaggerResponse(200, "Successfully updated review with result of id of updated review", typeof(Review))]
        [SwaggerResponse(402, "Bad request, bad data was specified")]
        public async Task<ActionResult<CreatedItemResult>> Put([FromForm] Review review,
            CancellationToken cancellationToken)
        {
            // ReSharper disable once MethodHasAsyncOverloadWithCancellation
            _context.Reviews.Update(review);
            await _context.SaveChangesAsync(cancellationToken);

            return Ok(review);
        }

    }
}
