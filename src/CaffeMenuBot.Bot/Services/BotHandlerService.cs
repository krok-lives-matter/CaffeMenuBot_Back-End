using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;

namespace CaffeMenuBot.Bot.Services
{
    /// <summary>
    /// Used as hosted service to run bot
    /// </summary>
    public sealed class BotHandlerService : BackgroundService
    {
        private readonly ITelegramBotClient _client;
        private readonly IUpdateHandler _updateHandler;
        private bool _isReceiving = false;

        public BotHandlerService(ITelegramBotClient client, IUpdateHandler updateHandler)
        {
            _client = client;
            _updateHandler = updateHandler;
        }

        public bool IsBotRunning()
        {
            return this._isReceiving;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _isReceiving = true;
            await _client.ReceiveAsync(_updateHandler, stoppingToken);       
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
             _isReceiving = false;
             await base.StopAsync(cancellationToken);
        }
    }
}