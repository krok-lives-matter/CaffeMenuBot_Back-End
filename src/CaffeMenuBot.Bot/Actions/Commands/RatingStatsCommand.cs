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
    /// <summary>
    /// Handles rating stats button press in menu
    /// </summary>
    public class RatingStatsCommand : IChatAction
    {
        private readonly CaffeMenuBotContext _context;
        private readonly ITelegramBotClient _client;
        private const string MESSAGE_TITLE = "5 Останніх відгуків";
        private const string COMMAND_NAME = "Відгуки";

        public RatingStatsCommand(CaffeMenuBotContext context, ITelegramBotClient client)
        {
            _context = context;
            _client = client;
        }
        
        public async Task ExecuteAsync(BotUser user, Update update, CancellationToken ct)
        {
            var reviews = await _context
                           .Reviews
                           .AsNoTracking()
                           .OrderByDescending(r => r.Id)
                           .Take(5)
                           .ToListAsync();

            StringBuilder reviewsMessage = new StringBuilder();
            reviewsMessage.Append(MESSAGE_TITLE + "\n");

            // Concat all reviews info in one string message
            foreach (var review in reviews)
            {
                reviewsMessage.Append
                (
                    $"<b>Коментар:</b> <i>{review.ReviewComment}</i>\n" +
                    $"<b>Оцінка:</b> {this.ConvertRatingToEmoji(review.Rating)}\n\n"
                );
            }
        
            await _client.SendTextMessageAsync
            (
                update.Message.From.Id,
                reviewsMessage.ToString(),
                Telegram.Bot.Types.Enums.ParseMode.Html
            );
        }

        private string ConvertRatingToEmoji(Rating rating)
        {
            switch(rating)
            {
                case Rating.rating_bad:
                {
                    return "😡";
                }
                case Rating.rating_ok:
                {
                    return "😐";
                }
                case Rating.rating_good:
                {
                    return "🙂";
                }
                case Rating.rating_great:
                {
                    return "😀";
                }
                case Rating.rating_excellent:
                {
                    return "😍";
                }
                case Rating.rating_unrated:
                {
                    return "Не оцінено";
                }
                default:
                {
                    return "Не оцінено";
                }
            }
        }
        public bool Contains(BotUser user, Update update)
        {
            if (update.CallbackQuery != null) return false;
            return update.Message.Text.StartsWith(COMMAND_NAME);
        }
    }
}
