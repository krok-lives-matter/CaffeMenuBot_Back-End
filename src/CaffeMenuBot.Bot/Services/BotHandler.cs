using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace CaffeMenuBot.Bot.Services
{
    public sealed partial class BotHandler : IUpdateHandler
    {
        private readonly ITelegramBotClient _client;
        private readonly CommandPatternManager _commandPatternManager;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<BotHandler> _logger;

        public BotHandler(ITelegramBotClient client, CommandPatternManager commandPatternManager,
            IServiceProvider serviceProvider, ILogger<BotHandler> logger)
        {
            _client = client;
            _commandPatternManager = commandPatternManager;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task HandleUpdate(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            await (update switch
            {
                { Message: { Chat: { Type: ChatType.Private }, Text: { } } } =>
                    HandleMessageAsync(update.Message, cancellationToken),
                _ => Task.CompletedTask
            });
        }

        public Task HandleError(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            Console.WriteLine(exception);
            return Task.CompletedTask;
        }

        public UpdateType[] AllowedUpdates => new[] {UpdateType.Message};
    }
}