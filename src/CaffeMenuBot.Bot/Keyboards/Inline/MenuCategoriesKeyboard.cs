using CaffeMenuBot.Data.Models.Menu;
using System.Collections.Generic;
using Telegram.Bot.Types.ReplyMarkups;

namespace CaffeMenuBot.Bot.Keyboards.Inline
{
    public class MenuCategoriesKeyboard
    {
        public static InlineKeyboardMarkup GetCategoriesMenu(List<Category> categories)
        {
            var table = new List<InlineKeyboardButton[]>();

            foreach (var category in categories)
                table.Add(new[] { InlineKeyboardButton.WithCallbackData(category.CategoryName, $"CAT {category.Id}") });

            var output = new InlineKeyboardMarkup(table.ToArray());

            return output;
        }
    }
}
