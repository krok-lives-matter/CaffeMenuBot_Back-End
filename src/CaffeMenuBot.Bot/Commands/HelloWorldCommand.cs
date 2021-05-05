using System.Threading;
using System.Threading.Tasks;
using CaffeMenuBot.Bot.Commands;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace CaffeMenuBot.Bot.Сommands
{
    public sealed class HelloWorldCommand : IChatAction
    {
        private ITelegramBotClient Client { get; }

        private const string CommandName = "/helloworld";

        public HelloWorldCommand(ITelegramBotClient client)
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
                "Hello world!", cancellationToken: ct);
        }
    }
}