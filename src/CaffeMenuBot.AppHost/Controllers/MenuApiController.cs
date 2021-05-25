using CaffeMenuBot.Data.Models.Menu;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CaffeMenuBot.AppHost.Models;
using CaffeMenuBot.Data;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.AspNetCore.Hosting;
using CaffeMenuBot.AppHost.Models.DTO.Requests;
using CaffeMenuBot.AppHost.Helpers;

namespace CaffeMenuBot.AppHost.Controllers
{
    [ApiController]
    [Route("api/dashboard/menu")]
    [Authorize]
    public class MenuApiController : ControllerBase
    {
        private readonly CaffeMenuBotContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        // used to save images of category covers while adding new category or updating it
        private const string MEDIA_SUBFOLDER = "category_covers";

        public MenuApiController(CaffeMenuBotContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        //GET api/dashboard/menu/dishes
        [HttpGet("dishes")]
        [SwaggerOperation("Gets all dishes", Tags = new[] { "Menu, Dishes" })]
        public async Task<ActionResult<IEnumerable<Dish>>> GetDishes(CancellationToken cancellationToken)
        {
            var dishes = await _context
                .Dishes
                .Include(d => d.Category)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
            return dishes;
        }


        //GET api/dashboard/menu/dishes/{id}
        [HttpGet("dishes/{id:int:min(1)}")]
        [SwaggerOperation("Gets dish by id", Tags = new[] { "Menu, Dishes" })]
        [SwaggerResponse(200, "Successfully found dish by specified id", typeof(Dish))]
        [SwaggerResponse(404, "Dish was not found by specified id")]
        public async Task<ActionResult<Dish>> GetDish(int id, CancellationToken cancellationToken)
        {
            var dish = await _context
                .Dishes
                .Include(d => d.Category)
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
            
            if (dish == null)
                return NotFound();
            
            return dish;
        }

        //DELETE api/dashboard/menu/dishes/{id}
        [HttpDelete("dishes/{id:int:min(1)}")]
        [SwaggerOperation("Deletes dish by id", Tags = new[] { "Menu, Dishes" })]
        [SwaggerResponse(204, "Successfully deleted dish by specified id")]
        [SwaggerResponse(404, "Dish was not found by specified id")]
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
        [SwaggerOperation("Creates dish", Tags = new[] { "Menu, Dishes" })]
        [SwaggerResponse(200, "Successfully created dish with result of id of created dish", typeof(Dish))]
        [SwaggerResponse(402, "Bad request, bad data was specified")]
        public async Task<ActionResult<CreatedItemResult>> PostDish([FromBody] Dish dish, CancellationToken cancellationToken)
        {
            // ReSharper disable once MethodHasAsyncOverloadWithCancellation
            _context.Dishes.Add(dish);
            await _context.SaveChangesAsync(cancellationToken);

            return Ok(dish);
        }

        //Put api/dashboard/menu/dishes
        [HttpPut("dishes")]
        [SwaggerOperation("Updates dish", Tags = new[] { "Menu, Dishes" })]
        [SwaggerResponse(200, "Successfully updated dish with result of id of updated dish", typeof(Dish))]
        [SwaggerResponse(402, "Bad request, bad data was specified")]
        public async Task<ActionResult<CreatedItemResult>> PutDish([FromBody] Dish dish, CancellationToken cancellationToken)
        {
            _context.Dishes.Update(dish);
            await _context.SaveChangesAsync(cancellationToken);

            return Ok(dish);
        }

        //GET api/dashboard/menu/categories
        [HttpGet("categories")]
        [SwaggerOperation("Gets all categories", Tags = new[] { "Menu, Categories" })]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategories(CancellationToken cancellationToken)
        {
            var categories = await _context.Categories
                                           .Include(c => c.Dishes)
                                           .AsNoTracking()
                                           .ToListAsync(cancellationToken);

            return Ok(categories);
        }


        //GET api/dashboard/menu/categories/{id}
        [HttpGet("categories/{id:int:min(1)}")]
        [SwaggerOperation("Gets category by id", Tags = new[] { "Menu, Categories" })]
        [SwaggerResponse(200, "Successfully found category by specified id", typeof(Category))]
        [SwaggerResponse(404, "Category was not found by specified id")]
        public async Task<ActionResult<Category>> GetCategory(int id, CancellationToken cancellationToken, CaffeMenuBotContext context)
        {
            var category = await _context
                .Categories
                .Include(c => c.Dishes)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
     
            if (category == null)
                return NotFound();

            return Ok(category);
        }
        

        // WARNING deletes all related subcategories and dishes
        //DELETE api/dashboard/menu/categories/{id}
        [HttpDelete("categories/{id:int:min(1)}")]
        [SwaggerOperation("Deletes category by id", Tags = new[] { "Menu, Categories" })]
        [SwaggerResponse(204, "Successfully deleted category by specified id")]
        [SwaggerResponse(404, "category was not found by specified id")]
        public async Task<ActionResult<Category>> DeleteCategory(int id, CancellationToken cancellationToken)
        {
            var categoryToDelete = await _context
                .Categories
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

            if (categoryToDelete == null)
                return NotFound();

            _context.Remove(categoryToDelete);

            await _context.SaveChangesAsync();

            return NoContent();
        }


        //POST api/dashboard/menu/categories
        [HttpPost("categories")]
        [SwaggerOperation("Creates category", Tags = new[] { "Menu, Categories" })]
        [SwaggerResponse(200, "Successfully created category with result of id of created category", typeof(Category))]
        [SwaggerResponse(402, "Bad request, bad data was specified")]
        public async Task<ActionResult<CreatedItemResult>> PostCategory([FromBody] CategoryRequest addCategoryRequest,
            CancellationToken cancellationToken)
        {
            var category = new Category()
            {
                CategoryName = addCategoryRequest.CategoryName,
                Dishes = addCategoryRequest.Dishes,
                IsVisible = addCategoryRequest.IsVisible
            };

            if (addCategoryRequest.CoverPhoto != null)
            {
                string categoryCoverFileName =
                    ImageHelper.SaveImage(addCategoryRequest.CoverPhoto, _webHostEnvironment, MEDIA_SUBFOLDER);

                category.CoverPhotoRelativeUrl = categoryCoverFileName;
            }
                                
            // ReSharper disable once MethodHasAsyncOverloadWithCancellation
            _context.Categories.Add(category);
            await _context.SaveChangesAsync(cancellationToken);

            return Ok(category);
        }

        //PUT api/dashboard/menu/categories
        [HttpPut("categories")]
        [SwaggerOperation("Updates category", Tags = new[] { "Menu, Categories" })]
        [SwaggerResponse(200, "Successfully category dish with result of id of category dish", typeof(Category))]
        [SwaggerResponse(404, "category you are trying to edit was not found, check Id that you are passing")]
        [SwaggerResponse(402, "Bad request, bad data was specified")]
        public async Task<ActionResult<CreatedItemResult>> PutCategory([FromBody] CategoryRequest updateCategoryRequest,
            CancellationToken cancellationToken)
        {
            var category = await _context.Categories.FindAsync(updateCategoryRequest.Id);

            if (category == null)
                return NotFound("category you are trying to edit was not found, check Id that you are passing");

            if (updateCategoryRequest.CoverPhoto != null)
            {
                string categoryCoverFileName =
                    ImageHelper.SaveImage(updateCategoryRequest.CoverPhoto, _webHostEnvironment, MEDIA_SUBFOLDER);

                category.CoverPhotoRelativeUrl = categoryCoverFileName;
            }

            category.CategoryName = updateCategoryRequest.CategoryName;
            category.Dishes = updateCategoryRequest.Dishes;
            category.IsVisible = updateCategoryRequest.IsVisible;

            _context.Entry(category).State = EntityState.Modified;

            await _context.SaveChangesAsync(cancellationToken);

            return Ok(category);
        }
    }
}
