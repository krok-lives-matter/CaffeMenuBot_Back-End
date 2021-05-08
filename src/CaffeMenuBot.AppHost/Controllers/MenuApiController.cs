using CaffeMenuBot.Data.Models.Menu;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CaffeMenuBot.AppHost.Helpers;
using CaffeMenuBot.AppHost.Models;
using CaffeMenuBot.Data;
using Microsoft.EntityFrameworkCore;

namespace CaffeMenuBot.AppHost.Controllers
{
    [ApiController]
    [Route("api/dashboard/menu")]
    [Authorize]
    public class MenuApiController : ControllerBase
    {
        private readonly CaffeMenuBotContext _context;
        
        public MenuApiController(CaffeMenuBotContext context)
        {
            _context = context;
        }

        //GET api/dashboard/menu/dishes
        [HttpGet("dishes")]
        public async Task<ActionResult<IEnumerable<Dish>>> GetDishes(CancellationToken cancellationToken)
        {
            var dishes = await _context
                .Dishes
                .AsNoTracking()
                .ToListAsync(cancellationToken);
            return dishes;
        }


        //GET api/dashboard/menu/dishes/{id}
        [HttpGet("dishes/{id:int:min(1)}")]
        public async Task<ActionResult<Dish>> GetDish(int id, CancellationToken cancellationToken)
        {
            var dish = await _context
                .Dishes
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
            
            if (dish == null)
                return NotFound();
            
            return dish;
        }

        //DELETE api/dashboard/menu/dishes/{id}
        [HttpDelete("dishes/{id:int:min(1)}")]
        public async Task<ActionResult<Dish>> DeleteDish(int id, CancellationToken cancellationToken)
        {
            var dishToDelete = await _context
                .Dishes
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);

            if (dishToDelete == null)
                return NotFound();

            _context.Remove(dishToDelete);

            await _context.SaveChangesAsync();

            return NoContent();
        }

        //POST api/dashboard/menu/dishes
        [HttpPost("dishes")]
        public async Task<ActionResult<CreatedItemResult>> PostDish([FromBody] Dish dish, CancellationToken cancellationToken)
        {
            // ReSharper disable once MethodHasAsyncOverloadWithCancellation
            _context.Dishes.Add(dish);
            await _context.SaveChangesAsync(cancellationToken);

            return new CreatedItemResult {CreatedItemId = dish.Id};
        }

        //Put api/dashboard/menu/dishes
        [HttpPut("dishes")]
        public async Task<ActionResult<CreatedItemResult>> PutDish([FromBody] Dish dish, CancellationToken cancellationToken)
        {
            _context.Dishes.Update(dish);
            await _context.SaveChangesAsync(cancellationToken);

            return new CreatedItemResult { CreatedItemId = dish.Id };
        }

        //GET api/dashboard/menu/categories
        [HttpGet("categories")]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategories(CancellationToken cancellationToken)
        {
            var categories = await _context.GetAllCategoriesRecursivelyAsync(cancellationToken);
            return Ok(categories);
        }


        //GET api/dashboard/menu/categories/{id}
        [HttpGet("categories/{id:int:min(1)}")]
        public async Task<ActionResult<Category>> GetCategory(int id, CancellationToken cancellationToken)
        {
            var category = await _context
                .Categories
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
            
            if (category == null)
                return NotFound();
            
            return category;
        }
        

        // WARNING deletes all related subcategories and dishes
        //DELETE api/dashboard/menu/categories/{id}
        [HttpDelete("categories/{id:int:min(1)}")]
        public async Task<ActionResult<Category>> DeleteCategory(int id, CancellationToken cancellationToken)
        {
            var categoryToDelete = await _context
                .Categories
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

            if (categoryToDelete == null)
                return NotFound();

            if(categoryToDelete.ParentCategoryId == null)
            {
                var subcategories = await _context.GetAllCategoriesRecursivelyAsync(cancellationToken, categoryToDelete.Id);

                if (subcategories == null)
                    return Problem("failed to delete subcategories", "subcategories", 500, "Delete operation failed", "error");

                _context.RemoveRange(subcategories);
            }

            _context.Remove(categoryToDelete);

            await _context.SaveChangesAsync();

            return Ok();
        }


        //POST api/dashboard/menu/categories
        [HttpPost("categories")]
        public async Task<ActionResult<CreatedItemResult>> PostCategory([FromBody] Category category,
            CancellationToken cancellationToken)
        {
            // ReSharper disable once MethodHasAsyncOverloadWithCancellation
            _context.Categories.Add(category);
            await _context.SaveChangesAsync(cancellationToken);

            return new CreatedItemResult {CreatedItemId = category.Id};
        }

        //PUT api/dashboard/menu/categories
        [HttpPut("categories")]
        public async Task<ActionResult<CreatedItemResult>> PutCategory([FromBody] Category category,
            CancellationToken cancellationToken)
        {
            // ReSharper disable once MethodHasAsyncOverloadWithCancellation
            _context.Categories.Update(category);
            await _context.SaveChangesAsync(cancellationToken);

            return new CreatedItemResult { CreatedItemId = category.Id };
        }
    }
}
