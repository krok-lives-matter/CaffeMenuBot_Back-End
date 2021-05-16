using System.Threading;
using Telegram.Bot;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using CaffeMenuBot.Bot.Keyboards.Inline;

namespace CaffeMenuBot.Bot.Commands
{
    public class SendRatingMenuCommand : IChatAction
    {
        private readonly ITelegramBotClient _client;
        private const string ratingTitleMessage = "<b>УВАГА!</b> При натисканні будь якої опції оцінки ваш відгук буде відправлено";
        private const string commandName = "Оцінити роботу";

        public SendRatingMenuCommand(ITelegramBotClient client)
        {
            _client = client;
        }

        public async Task ExecuteAsync(Message message, CancellationToken ct)
        {
            await _client.SendTextMessageAsync
            (
                message.From.Id,
                ratingTitleMessage,
                Telegram.Bot.Types.Enums.ParseMode.Html,
                replyMarkup: RateKeyboard.GetRateKeyboard
            );
        }
        public bool Contains(Message message)
        {
            return message.Text.StartsWith(commandName);
        }
    }
}
