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
    [Route("api/dashboard")]
    [Authorize]
    public class DashboardApiController : ControllerBase
    {
        private readonly CaffeMenuBotContext _context;
        
        public DashboardApiController(CaffeMenuBotContext context)
        {
            _context = context;
        }

        #region dishes

        //GET api/dashboard/getAllDishes
        [HttpGet("getAllDishes")]
        public async Task<ActionResult<IEnumerable<Dish>>> GetAllDishes(CancellationToken cancellationToken)
        {
            var dishes = await _context
                .Dishes
                .AsNoTracking()
                .ToListAsync(cancellationToken);
            return dishes;
        }


        //GET api/dashboard/getDish/{id}
        [HttpGet("getDish/{id:int:min(1)}")]
        public async Task<ActionResult<Dish>> GetDishById(int id, CancellationToken cancellationToken)
        {
            var dish = await _context
                .Dishes
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
            
            if (dish == null)
                return NotFound();
            
            return dish;
        }

        //GET api/dashboard/deleteDish/{id}
        [HttpGet("deleteDish/{id:int:min(1)}")]
        public async Task<ActionResult<Dish>> DeleteDishById(int id, CancellationToken cancellationToken)
        {
            var dishToDelete = await _context
                .Dishes
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);

            if (dishToDelete == null)
                return NotFound();

            _context.Remove(dishToDelete);

            await _context.SaveChangesAsync();

            return Ok();
        }

        //POST api/dashboard/addDish
        [HttpPost("addDish")]
        public async Task<ActionResult<CreatedItemResult>> AddDish([FromBody] Dish dish, CancellationToken cancellationToken)
        {
            // ReSharper disable once MethodHasAsyncOverloadWithCancellation
            _context.Dishes.Add(dish);
            await _context.SaveChangesAsync(cancellationToken);

            return new CreatedItemResult {CreatedItemId = dish.Id};
        }

        //POST api/dashboard/updateDish
        [HttpPost("updateDish")]
        public async Task<ActionResult<CreatedItemResult>> UpdateDish([FromBody] Dish dish, CancellationToken cancellationToken)
        {
            _context.Dishes.Update(dish);
            await _context.SaveChangesAsync(cancellationToken);

            return new CreatedItemResult { CreatedItemId = dish.Id };
        }

        #endregion

        #region categories

        //GET api/dashboard/getAllCategories
        [HttpGet("getAllCategories")]
        public async Task<ActionResult<List<Category>>> GetAllCategories(CancellationToken cancellationToken)
        {
            var categories = await _context.GetAllCategoriesRecursivelyAsync(cancellationToken);
            return Ok(categories);
        }


        //GET api/dashboard/getCategory/{id}
        [HttpGet("getCategory/{id:int:min(1)}")]
        public async Task<ActionResult<Category>> GetCategoryById(int id, CancellationToken cancellationToken)
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
        //GET api/dashboard/deleteCategory/{id}
        [HttpGet("deleteCategory/{id:int:min(1)}")]
        public async Task<ActionResult<Category>> DeleteCategoryById(int id, CancellationToken cancellationToken)
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


        //POST api/dashboard/addCategory
        [HttpPost("addCategory")]
        public async Task<ActionResult<CreatedItemResult>> AddCategory([FromBody] Category category,
            CancellationToken cancellationToken)
        {
            // ReSharper disable once MethodHasAsyncOverloadWithCancellation
            _context.Categories.Add(category);
            await _context.SaveChangesAsync(cancellationToken);

            return new CreatedItemResult {CreatedItemId = category.Id};
        }

        //POST api/dashboard/updateCategory
        [HttpPost("updateCategory")]
        public async Task<ActionResult<CreatedItemResult>> UpdateCategory([FromBody] Category category,
            CancellationToken cancellationToken)
        {
            // ReSharper disable once MethodHasAsyncOverloadWithCancellation
            _context.Categories.Update(category);
            await _context.SaveChangesAsync(cancellationToken);

            return new CreatedItemResult { CreatedItemId = category.Id };
        }

        #endregion
    }
}
