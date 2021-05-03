using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CaffeMenuBot.Data;
using CaffeMenuBot.Data.Models.Reviews;
using Microsoft.EntityFrameworkCore;
using CaffeMenuBot.AppHost.Models;

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
        public async Task<ActionResult<IEnumerable<Review>>> Get(CancellationToken cancellationToken)
        {
            var reviews = await _context
                .Reviews
                .AsNoTracking()
                .ToListAsync(cancellationToken);
            return reviews;
        }

        //GET api/dashboard/reviews/{id}
        [HttpGet("{id:int:min(1)}")]
        public async Task<ActionResult<Review>> Get(int id, CancellationToken cancellationToken)
        {
            var review = await _context
                .Reviews
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
            
            if (review == null)
                return NotFound();
            
            return review;
        }

        //DELETE api/dashboard/reviews/{id}
        [HttpDelete("{id:int:min(1)}")]
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
        public async Task<ActionResult<CreatedItemResult>> Post([FromBody] Review review,
            CancellationToken cancellationToken)
        {
            // ReSharper disable once MethodHasAsyncOverloadWithCancellation
            _context.Reviews.Add(review);
            await _context.SaveChangesAsync(cancellationToken);

            return new CreatedItemResult {CreatedItemId = review.Id};
        }

        //PUT api/dashboard/reviews/
        [HttpPut]
        public async Task<ActionResult<CreatedItemResult>> Put([FromBody] Review review,
            CancellationToken cancellationToken)
        {
            // ReSharper disable once MethodHasAsyncOverloadWithCancellation
            _context.Reviews.Update(review);
            await _context.SaveChangesAsync(cancellationToken);

            return new CreatedItemResult { CreatedItemId = review.Id };
        }

    }
}
