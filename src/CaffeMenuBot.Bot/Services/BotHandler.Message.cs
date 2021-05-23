using System.Threading;
using System.Threading.Tasks;
using CaffeMenuBot.Data.Models.Bot;
using Telegram.Bot.Types;

namespace CaffeMenuBot.Bot.Services
{
    partial class BotHandler
    {
        /// <summary>
        /// Handles user command messages
        /// </summary>
        /// <param name="user">user object</param>
        /// <param name="u">update object</param>
        /// <param name="ct">cancellation token</param>
        private async Task HandleMessageAsync(BotUser user, Update u, CancellationToken ct)
        {
            await _commandPatternManager.HandlePatternAsync(user, u, ct);
        }
    }
}