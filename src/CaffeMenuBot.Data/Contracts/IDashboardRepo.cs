using CaffeMenuBot.Data.Models.Menu;
using System.Collections.Generic;

namespace CaffeMenuBot.Data.Contracts
{
    public interface IDashboardRepo
    {
        public bool SaveChanges();
        public IEnumerable<Dish>? GetAllDishes();
        public Dish? GetDishById(int id);
        public void AddDish(Dish dish);

        public IEnumerable<Category>? GetAllCategories();
        public Category? GetCategoryById(int id);
        public void AddCategory(Category category);
    }
}
