using CaffeMenuBot.Data.Contracts;
using CaffeMenuBot.Data.Models.Menu;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace CaffeMenuBot.AppHost.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardRepo _dashboardRepo;
        public DashboardController(IDashboardRepo dashboardRepo)
        {
            _dashboardRepo = dashboardRepo;
        }

        //GET api/dashboard/dishes
        [HttpGet("dishes")]
        public ActionResult<IEnumerable<Dish>> GetAllDishes()
        {
            var dishes = _dashboardRepo.GetAllDishes();
            return Ok(dishes);
        }


        //GET api/dashboard/dishes/{id}
        [HttpGet("dishes/{id}", Name = "GetDishById")]
        public ActionResult<Dish> GetDishById(int id)
        {
            var dish = _dashboardRepo.GetDishById(id);
            if (dish != null)
                return Ok(dish);
            return NotFound($"Dish with id {id} was not found");
        }

        [HttpPost("dishes")]
        public ActionResult<Dish> AddDish(Dish dish)
        {
            if (dish == null) return BadRequest();

            _dashboardRepo.AddDish(dish);

            _dashboardRepo.SaveChanges();

            var createdDish = _dashboardRepo.GetDishById(dish.Id);

            if (createdDish == null)
                return BadRequest();

            return CreatedAtRoute(nameof(GetDishById), new { Id = createdDish.Id }, createdDish);
        }

        //GET api/dashboard/categories
        [HttpGet("categories")]
        public ActionResult<IEnumerable<Category>> GetAllCategories()
        {
            var categories = _dashboardRepo.GetAllCategories();
            return Ok(categories);
        }


        //GET api/dashboard/categories/{id}
        [HttpGet("categories/{id}", Name = "GetCategoryById")]
        public ActionResult<Category> GetCategoryById(int id)
        {
            var category = _dashboardRepo.GetCategoryById(id);
            if (category != null)
                return Ok(category);
            return NotFound($"category with id {id} was not found");
        }

        [HttpPost("categories")]
        public ActionResult<Dish> AddCategory(Category category)
        {
            if (category == null) return BadRequest();

            _dashboardRepo.AddCategory(category);
            _dashboardRepo.SaveChanges();

            var createdCategory = _dashboardRepo.GetCategoryById(category.Id);

            if (createdCategory == null)
                return BadRequest();

            return CreatedAtRoute(nameof(GetCategoryById), new { Id = createdCategory.Id }, createdCategory);
        }
    }
}
