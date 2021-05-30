using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CaffeMenuBot.Bot.Actions.Interface;
using CaffeMenuBot.Bot.Keyboards.Inline;
using CaffeMenuBot.Data;
using CaffeMenuBot.Data.Models.Bot;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace CaffeMenuBot.Bot.Actions.Commands
{
    /// <summary>
    /// Handles menu button press in main menu keyboard
    /// </summary>
    public sealed class MenuCommand : IChatAction
    {
        private readonly CaffeMenuBotContext _context;
        private readonly ITelegramBotClient _client;

        private const string COMMAND_NAME = "Menu";

        public MenuCommand(ITelegramBotClient client, CaffeMenuBotContext context)
        {
            _client = client;
            _context = context;
        }

        public bool Contains(BotUser user, Update update)
        {
            if (update.CallbackQuery != null) return false;
            return update.Message.Text.StartsWith(COMMAND_NAME);
        }

        public async Task ExecuteAsync(BotUser user, Update update, CancellationToken ct)
        {
            var categories = await _context.Categories
                .AsNoTracking()
                .Where(c => c.IsVisible)
                .ToListAsync(ct);

            await _client.SendTextMessageAsync(
                update.Message.From.Id,
                "Press category button to get menu",
                replyMarkup: MenuCategoriesKeyboard.GetCategoriesMenu(categories),
                cancellationToken: ct);
        }
    }
}