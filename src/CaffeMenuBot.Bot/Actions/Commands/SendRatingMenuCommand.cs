using System.Threading;
using Telegram.Bot;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using CaffeMenuBot.Bot.Keyboards.Inline;
using CaffeMenuBot.Data.Models.Bot;
using CaffeMenuBot.Bot.Actions.Interface;

namespace CaffeMenuBot.Bot.Actions.Commands
{
    /// <summary>
    /// Handles send rating menu button press in bot
    /// </summary>
    public class SendRatingMenuCommand : IChatAction
    {
        private readonly ITelegramBotClient _client;
        private const string MESSAGE_TITLE = "<b>УВАГА!</b> При натисканні будь якої опції оцінки ваш відгук буде відправлено";
        private const string COMMAND_NAME = "Оцінити роботу";

        public SendRatingMenuCommand(ITelegramBotClient client)
        {
            _client = client;
        }

        public async Task ExecuteAsync(BotUser user, Update update, CancellationToken ct)
        {
            // Send inline rating keyboard to user
            await _client.SendTextMessageAsync
            (
                update.Message.From.Id,
                MESSAGE_TITLE,
                Telegram.Bot.Types.Enums.ParseMode.Html,
                replyMarkup: RateKeyboard.GetRateKeyboard
            );
        }
        public bool Contains(BotUser user, Update update)
        {
            if (update.CallbackQuery != null) return false;
            return update.Message.Text.StartsWith(COMMAND_NAME);
        }
    }
}
