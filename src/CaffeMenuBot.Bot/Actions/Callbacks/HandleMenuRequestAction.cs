using System.Threading;
using System.Threading.Tasks;
using CaffeMenuBot.Bot.Actions.Interface;
using CaffeMenuBot.Data.Models.Bot;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace CaffeMenuBot.Bot.Actions.Сommands
{
    public sealed class HandleMenuRequestAction : IChatAction
    {
        private ITelegramBotClient Client { get; }
        
        // Contains uses this identifier instead of COMMAND_NAME
        private const string CALLBACK_ID = "CAT";
        private const string MESSAGE_TITLE = "Натисніть щоб відкрити файл";

        public HandleMenuRequestAction(ITelegramBotClient client)
        {
            Client = client;
        }

        public bool Contains(BotUser user, Update update)
        {
            if (update.CallbackQuery == null) return false;
            return update.CallbackQuery.Data.StartsWith(CALLBACK_ID);
        }

        public async Task ExecuteAsync(BotUser user, Update update, CancellationToken ct)
        {
            await Client.AnswerCallbackQueryAsync(
                update.CallbackQuery.Id,
                MESSAGE_TITLE,
                cancellationToken: ct);

            await Client.SendTextMessageAsync(
                update.CallbackQuery.From.Id,
                MESSAGE_TITLE,
                cancellationToken: ct);
        }
    }
}