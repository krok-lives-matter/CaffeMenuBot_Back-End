using CaffeMenuBot.Data.Models.Menu;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CaffeMenuBot.Data.Contracts
{
    public class MockDashboardRepo : IDashboardRepo
    {
        private List<Dish> _dishes;
        private List<Category> _categories;

        public MockDashboardRepo()
        {
            _dishes = new List<Dish>()
            {
                new Dish()
                {
                    Id = 1,
                    DishName = "Львівські Пляцки",
                    Description = "Дуже смачно",
                    Price = 100.5m,
                    Serving = "200гр.",
                    CategoryId = 1
                },
                new Dish()
                {
                    Id = 2,
                    DishName = "Шашлик машлик",
                    Description = "мясо",
                    Price = 350.5m,
                    Serving = "250гр.",
                    CategoryId = 2,
                },
            };

            _categories = new List<Category>()
            {
                new Category()
                {
                    Id = 1,
                    CategoryName = "Десерти",
                    Dishes = new List<Dish>() {_dishes[0]}
                },
                new Category()
                {
                    Id = 2,
                    CategoryName = "Мясо",
                    Dishes = new List<Dish>() {_dishes[1]}
                },
            };
        }

        /// <summary>
        /// adds new category
        /// </summary>
        /// <exception cref="ArgumentNullException">if category is null</exception>
        /// <param name="category">catogery to add</param>
        public void AddCategory(Category category)
        {
            if (category == null)
                throw new ArgumentNullException(nameof(category));

            _categories.Add(category);
        }

        /// <summary>
        /// adds new dish
        /// </summary>
        /// <exception cref="ArgumentNullException">if dish is null</exception>
        /// <param name="dish">dish to add</param>

        public void AddDish(Dish dish)
        {
            if (dish == null)
                throw new ArgumentNullException(nameof(dish));

            _dishes.Add(dish);
        }

        public IEnumerable<Category> GetAllCategories()
        {
            return _categories;
        }

        public IEnumerable<Dish> GetAllDishes()
        {
            return _dishes;
        }

        public Category? GetCategoryById(int id)
        {
            return _categories.FirstOrDefault(category => category.Id == id);
        }

        public Dish? GetDishById(int id)
        {
            return _dishes.FirstOrDefault(dish => dish.Id == id);
        }

        public bool SaveChanges()
        {
            return true;
        }
    }
}
