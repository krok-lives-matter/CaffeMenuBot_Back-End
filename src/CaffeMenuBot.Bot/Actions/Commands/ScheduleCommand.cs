using System.Text;
using System.Threading;
using CaffeMenuBot.Data;
using Telegram.Bot;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types;
using CaffeMenuBot.Data.Models.Bot;
using CaffeMenuBot.Bot.Actions.Interface;

namespace CaffeMenuBot.Bot.Actions.Commands
{
    public class ScheduleCommand : IChatAction
    {
        private readonly CaffeMenuBotContext _context;
        private readonly ITelegramBotClient _client;
        private const string scheduleTitleMessage = "Завітайте до нас!";
        private const string commandName = "Графік роботи";

        public ScheduleCommand(CaffeMenuBotContext context, ITelegramBotClient client)
        {
            _context = context;
            _client = client;
        }

        public async Task ExecuteAsync(BotUser user, Update update, CancellationToken ct)
        {
            var schedule = await _context
                           .Schedule
                           .AsNoTracking()
                           .OrderBy(s => s.OrderIndex)
                           .ToListAsync();

            StringBuilder scheduleMessage = new StringBuilder();
            scheduleMessage.Append(scheduleTitleMessage + "\n");

            foreach (var weekday in schedule)
            {
                scheduleMessage.Append
                (
                    $"<b>{weekday.WeekdayName}</b> {weekday.OpenTime}-{weekday.CloseTime}\n"
                );
            }
        
            await _client.SendTextMessageAsync
            (
                update.Message.From.Id,
                scheduleMessage.ToString(),
                Telegram.Bot.Types.Enums.ParseMode.Html
            );
        }
        public bool Contains(BotUser user, Update update)
        {
            if (update.CallbackQuery != null) return false;
            return update.Message.Text.StartsWith(commandName);
        }
    }
}
