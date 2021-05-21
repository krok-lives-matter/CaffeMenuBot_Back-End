using System;
using System.Threading;
using System.Threading.Tasks;
using CaffeMenuBot.Bot.Actions.Interface;
using CaffeMenuBot.Data;
using Microsoft.Extensions.DependencyInjection;
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
        private readonly PatternManager<IChatAction> _commandPatternManager;
        private readonly PatternManager<IStateAction> _statePatternManager;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<BotHandler> _logger;

        public BotHandler
            (ITelegramBotClient client,
            PatternManager<IChatAction> commandPatternManager,
            PatternManager<IStateAction> stateManager,
            IServiceProvider serviceProvider, ILogger<BotHandler> logger)
        {
            _client = client;
            _commandPatternManager = commandPatternManager;
            _statePatternManager = stateManager;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task HandleUpdate(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {

            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<CaffeMenuBotContext>();

            var user = await HandleUser(context, update, cancellationToken);

            if (user.State != Data.Models.Bot.ChatState.default_state)
            {
                await HandleStateAsync(user, update, cancellationToken);
                return;
            }

            await (update switch
            {
                { Type: UpdateType.CallbackQuery } => HandleMessageAsync(user, update, cancellationToken),
                { Message: { Chat: { Type: ChatType.Private }, Text: { } } } => HandleMessageAsync(user, update, cancellationToken),
                _ => Task.CompletedTask
            });
        }

        public Task HandleError(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            Console.WriteLine(exception);
            return Task.CompletedTask;
        }

        public UpdateType[] AllowedUpdates => new[] {UpdateType.Message, UpdateType.CallbackQuery};
    }
}