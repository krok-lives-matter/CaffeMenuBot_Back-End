using System.Threading;
using System.Threading.Tasks;
using CaffeMenuBot.Bot.Actions.Interface;
using CaffeMenuBot.Data.Models.Bot;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace CaffeMenuBot.Bot.Actions.Сommands
{
    public sealed class MenuCommand : IChatAction
    {
        private ITelegramBotClient Client { get; }

        private const string CommandName = "Меню";

        public MenuCommand(ITelegramBotClient client)
        {
            Client = client;
        }

        public bool Contains(BotUser user, Update update)
        {
            if (update.CallbackQuery != null) return false;
            return update.Message.Text.StartsWith(CommandName);
        }

        public async Task ExecuteAsync(BotUser user, Update update, CancellationToken ct)
        {
            await Client.SendTextMessageAsync(
                update.Message.From.Id,
                "Temp",
                cancellationToken: ct);
        }
    }
}