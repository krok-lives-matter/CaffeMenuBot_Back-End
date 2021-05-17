using System.Threading;
using System.Threading.Tasks;
using CaffeMenuBot.Bot.Commands;
using CaffeMenuBot.Bot.Keyboards.Reply;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace CaffeMenuBot.Bot.Сommands
{
    public sealed class StartCommand : IChatAction
    {
        private ITelegramBotClient Client { get; }

        private const string CommandName = "/start";
        private const string messageTitle = "Натисніть на кнопки меню нижче для отримання необхідної інформації";

        public StartCommand(ITelegramBotClient client)
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
                messageTitle,
                replyMarkup: MenuKeyboard.MainMenu,
                cancellationToken: ct);
        }
    }
}