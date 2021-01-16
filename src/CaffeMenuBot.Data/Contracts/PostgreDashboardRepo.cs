using CaffeMenuBot.Data.Models.Menu;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CaffeMenuBot.Data.Contracts
{
    public class PostgreDashboardRepo : IDashboardRepo
    {
        private readonly CaffeMenuBotContext _context;

        public PostgreDashboardRepo(CaffeMenuBotContext context)
        {
            _context = context;
        }

        public bool SaveChanges()
        {
            return _context.SaveChanges() >= 0;
        }


        #region dishes

        public IEnumerable<Dish> GetAllDishes()
        {
            return _context.Dishes.ToList();
        }

        public Dish? GetDishById(int id)
        {
            return _context.Dishes.Find(id);
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

            _context.Dishes.Add(dish);
        }

        #endregion


        #region categories
        public IEnumerable<Category>? GetAllCategories()
        {
            return this.GetCategoriesRecursive(null);
        }

        public Category? GetCategoryById(int id)
        {
            var category = _context.Categories
                .Include(category => category.Dishes)
                .FirstOrDefault(category => category.Id == id);
            return category;
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

            _context.Categories.Add(category);
        }

        /// <summary>
        /// loads relates subcategories of category
        /// if null is passed = load all categories with all related subcategories
        /// </summary>
        /// <param name="parentCategoryId"></param>
        /// <returns>List of categories or selected single parent category</returns>
        List<Category>? GetCategoriesRecursive(int? parentCategoryId)
        {
            var rootCategories = _context.Categories
                               .Include(c => c.Dishes)
                               .Where(c => c.ParentCategoryId == parentCategoryId)
                               .ToList();

            if (rootCategories == null || rootCategories.Count == 0) return null;

            foreach (var category in rootCategories)
            {
                var subCategories = GetCategoriesRecursive(category.Id);

                category.SubCategories = subCategories;
            }

            return rootCategories;
        }

        #endregion
    }
}
