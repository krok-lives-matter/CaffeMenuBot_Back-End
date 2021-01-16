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
    [AllowAnonymous]
    public class DashboardApiController : ControllerBase
    {
        private readonly CaffeMenuBotContext _context;
        
        public DashboardApiController(CaffeMenuBotContext context)
        {
            _context = context;
        }

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

        [HttpPost("addDish")]
        public async Task<ActionResult<CreatedItemResult>> AddDish([FromBody] Dish dish, CancellationToken cancellationToken)
        {
            // ReSharper disable once MethodHasAsyncOverloadWithCancellation
            _context.Dishes.Add(dish);
            await _context.SaveChangesAsync(cancellationToken);

            return new CreatedItemResult {CreatedItemId = dish.Id};
        }

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
    }
}
