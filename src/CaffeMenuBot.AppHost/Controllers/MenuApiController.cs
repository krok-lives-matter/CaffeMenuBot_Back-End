﻿using CaffeMenuBot.Data.Models.Menu;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using CaffeMenuBot.AppHost.Models;
using CaffeMenuBot.Data;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.AspNetCore.Hosting;
using CaffeMenuBot.AppHost.Models.DTO.Requests;
using CaffeMenuBot.AppHost.Helpers;
using System.Linq;

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

        //GET api/dashboard/menu/dish?dish_id=1
        [HttpGet("dish")]
        [SwaggerOperation("Gets dish by id", Tags = new[] { "Menu, Dishes" })]
        [SwaggerResponse(200, "Successfully found dish by specified id", typeof(Dish))]
        [SwaggerResponse(404, "Dish was not found by specified id")]
        public async Task<ActionResult<Dish>> GetDish(int dish_id, CancellationToken cancellationToken)
        {
            var dish = await _context
                .Dishes
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == dish_id, cancellationToken);
            
            if (dish == null)
                return NotFound();
            
            return Ok(dish);
        }

        //GET api/dashboard/menu/dishes?category_id=1
        [HttpGet("dishes")]
        [SwaggerOperation("Gets all dishes by category_id", Tags = new[] { "Menu, Dishes" })]
        [SwaggerResponse(200, "Successfully found all dishes by specified category_id", typeof(Dish))]
        [SwaggerResponse(404, "Dishes was not found by specified category_id")]
        public async Task<ActionResult<List<Dish>>> GetDishesByCategoryId(int category_id, CancellationToken cancellationToken)
        {
            var dishes = await _context
                .Dishes
                .AsNoTracking()
                .Where(d => d.CategoryId == category_id)
                .ToListAsync(cancellationToken);

            return Ok(dishes);
        }

        //DELETE api/dashboard/menu/dishes?dish_id=1
        [HttpDelete("dishes")]
        [SwaggerOperation("Deletes dish by id", Tags = new[] { "Menu, Dishes" })]
        [SwaggerResponse(204, "Successfully deleted dish by specified id")]
        [SwaggerResponse(404, "Dish was not found by specified id")]
        public async Task<ActionResult<Dish>> DeleteDish(int dish_id, CancellationToken cancellationToken)
        {
            var dishToDelete = await _context
                .Dishes
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == dish_id, cancellationToken);

            if (dishToDelete == null)
                return NotFound();

            _context.Remove(dishToDelete);

            await _context.SaveChangesAsync(cancellationToken);

            return NoContent();
        }

        //POST api/dashboard/menu/dishes
        [HttpPost("dishes")]
        [SwaggerOperation("Creates dish", Tags = new[] { "Menu, Dishes" })]
        [SwaggerResponse(200, "Successfully created dish with result of id of created dish", typeof(Dish))]
        [SwaggerResponse(402, "Bad request, bad data was specified")]
        public async Task<ActionResult<CreatedItemResult>> PostDish([FromBody] Dish dish, CancellationToken cancellationToken)
        {
            if (ModelState.IsValid)
            {
                // ReSharper disable once MethodHasAsyncOverloadWithCancellation
                _context.Dishes.Add(dish);
                await _context.SaveChangesAsync(cancellationToken);
    
                return Ok(dish);
            }
            return BadRequest(ModelState);        
        }

        //Put api/dashboard/menu/dishes
        [HttpPut("dishes")]
        [SwaggerOperation("Updates dish", Tags = new[] { "Menu, Dishes" })]
        [SwaggerResponse(200, "Successfully updated dish with result of id of updated dish", typeof(Dish))]
        [SwaggerResponse(402, "Bad request, bad data was specified")]
        public async Task<ActionResult<CreatedItemResult>> PutDish([FromBody] Dish dish, CancellationToken cancellationToken)
        {
            if(ModelState.IsValid)
            {
                _context.Dishes.Update(dish);
                await _context.SaveChangesAsync(cancellationToken);
                return Ok(dish);
            }
            return BadRequest(ModelState);          
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

            if(categories != null)
            {
                foreach(var category in categories)
                    if (!string.IsNullOrEmpty(category.CoverPhotoFileName))
                        category.CoverPhotoUrl = $"{Startup.BaseImageUrl}/{MEDIA_SUBFOLDER}/{category.CoverPhotoFileName}";
            }

            return Ok(categories);
        }


        //GET api/dashboard/menu/categories/{id}
        [HttpGet("categories/{id:int:min(1)}")]
        [SwaggerOperation("Gets category by id", Tags = new[] { "Menu, Categories" })]
        [SwaggerResponse(200, "Successfully found category by specified id", typeof(Category))]
        [SwaggerResponse(404, "Category was not found by specified id")]
        public async Task<ActionResult<Category>> GetCategory(int id, CancellationToken cancellationToken)
        {
            var category = await _context
                .Categories
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
     
            if (category == null)
                return NotFound();

            if (!string.IsNullOrEmpty(category.CoverPhotoFileName))
                category.CoverPhotoUrl = $"{Startup.BaseImageUrl}/{MEDIA_SUBFOLDER}/{category.CoverPhotoFileName}";

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

            await _context.SaveChangesAsync(cancellationToken);

            return NoContent();
        }


        //POST api/dashboard/menu/categories
        [HttpPost("categories")]
        [SwaggerOperation("Creates category", Tags = new[] { "Menu, Categories" })]
        [SwaggerResponse(200, "Successfully created category with result of id of created category", typeof(Category))]
        [SwaggerResponse(402, "Bad request, bad data was specified")]
        public async Task<ActionResult<CreatedItemResult>> PostCategory(CategoryRequest addCategoryRequest,
            CancellationToken cancellationToken)
        {
            if(ModelState.IsValid)
            {
                var category = new Category()
                {
                    CategoryName = addCategoryRequest.CategoryName,
                    IsVisible = addCategoryRequest.IsVisible
                };

                // ReSharper disable once MethodHasAsyncOverloadWithCancellation
                _context.Categories.Add(category);
                await _context.SaveChangesAsync(cancellationToken);

                return Ok(category);
            }
            return BadRequest(ModelState);
        }

        [HttpPost("categories/setCoverPhoto")]
        [SwaggerOperation("Sets category's cover photo.", Tags = new[] { "Menu, Categories" })]
        [SwaggerResponse(200, "Successfully set category and returned a link to it.", typeof(CreateItemLinkResult))]
        [SwaggerResponse(400, "Bad request.")]
        public async Task<ActionResult<CreateItemLinkResult>> SetCategoryCoverPhoto([FromForm] SetCoverPhotoRequest request,
            CancellationToken cancellationToken)
        {
            if(ModelState.IsValid)
            {
                var category = await _context.Categories
                    .FirstOrDefaultAsync(c => c.Id == request.CategoryId, cancellationToken);
                if (category == null)
                {
                    return NotFound();
                }

                category.CoverPhotoFileName =
                    ImageHelper.SaveImage(new ImageModel
                    {
                        ContentType = request.File.ContentType,
                        ImageStream = request.File.OpenReadStream(),
                        FileExtension = Path.GetExtension(request.File.FileName)
                    }, _webHostEnvironment, MEDIA_SUBFOLDER);
                await _context.SaveChangesAsync(cancellationToken);

                return new CreateItemLinkResult
                {
                    ImageLink = $"{Startup.BaseImageUrl}/{MEDIA_SUBFOLDER}/{category.CoverPhotoFileName}"
                };
            }
            return BadRequest(ModelState);
        }

        //PUT api/dashboard/menu/categories
        [HttpPut("categories")]
        [SwaggerOperation("Updates category", Tags = new[] { "Menu, Categories" })]
        [SwaggerResponse(200, "Successfully category dish with result of id of category dish", typeof(Category))]
        [SwaggerResponse(404, "category you are trying to edit was not found, check Id that you are passing")]
        [SwaggerResponse(402, "Bad request, bad data was specified")]
        public async Task<ActionResult<CreatedItemResult>> PutCategory(CategoryRequest updateCategoryRequest,
            CancellationToken cancellationToken)
        {
            if(ModelState.IsValid)
            {
                var category = await _context.Categories.FindAsync(updateCategoryRequest.Id);

                if (category == null)
                    return NotFound("category you are trying to edit was not found, check Id that you are passing");

                category.CategoryName = updateCategoryRequest.CategoryName;

                category.IsVisible = updateCategoryRequest.IsVisible;

                _context.Entry(category).State = EntityState.Modified;

                await _context.SaveChangesAsync(cancellationToken);

                return Ok(category);
            }
            return BadRequest(ModelState);
        }
    }
}
