using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CaffeMenuBot.Data;
using CaffeMenuBot.Data.Models.Menu;
using Microsoft.EntityFrameworkCore;

namespace CaffeMenuBot.AppHost.Helpers
{
    public static class CategoriesRecursiveHelper
    {
        public static async Task<List<Category>> GetAllCategoriesRecursivelyAsync(this CaffeMenuBotContext context,
            CancellationToken cancellationToken, int? parentCategoryId = null)
        {
            var categories = await context.Categories
                .AsNoTracking()
                .Where(c => c.ParentCategoryId == parentCategoryId)
                .ToListAsync(cancellationToken);

            foreach (Category category in categories)
            {
                category.SubCategories = await context.GetAllCategoriesRecursivelyAsync(cancellationToken, category.Id);
            }

            return categories;
        }
    }
}