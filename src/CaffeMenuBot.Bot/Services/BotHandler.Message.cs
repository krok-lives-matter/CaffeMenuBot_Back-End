using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;

namespace CaffeMenuBot.Bot.Services
{
    partial class BotHandler
    {
        private async Task HandleMessageAsync(Message m, CancellationToken ct)
        {
            await _commandPatternManager.HandleCommandAsync(m, ct);
        }
    }
}