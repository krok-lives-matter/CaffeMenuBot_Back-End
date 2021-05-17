using System.Threading;
using System.Threading.Tasks;
using CaffeMenuBot.Bot.Commands;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace CaffeMenuBot.Bot.Сommands
{
    public sealed class MenuCommand : IChatAction
    {
        private ITelegramBotClient Client { get; }

        private const string CommandName = "Меню";

        public MenuCommand(ITelegramBotClient client)
        {
            Client = client;
        }

        public bool Contains(Message message)
        {
            return message.Text.StartsWith(CommandName);
        }

        public async Task ExecuteAsync(Message message, CancellationToken ct)
        {
            await Client.SendTextMessageAsync(
                message.Chat.Id,
                "Temp",
                cancellationToken: ct);
        }
    }
}