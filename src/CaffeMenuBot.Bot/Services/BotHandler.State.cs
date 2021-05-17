using System.Threading;
using System.Threading.Tasks;
using CaffeMenuBot.Data.Models.Bot;
using Telegram.Bot.Types;

namespace CaffeMenuBot.Bot.Services
{
    partial class BotHandler
    {
        private async Task HandleStateAsync(BotUser user, Update u, CancellationToken ct)
        {
            await _statePatternManager.HandlePatternAsync(user, u, ct);
        }
    }
}