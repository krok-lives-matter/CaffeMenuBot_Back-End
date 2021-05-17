using CaffeMenuBot.Data.Models.Bot;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace CaffeMenuBot.Bot.Actions.Interface {
    public interface IPatternAction {
       Task ExecuteAsync(BotUser user, Update update, CancellationToken ct);

       bool Contains(BotUser user, Update update);
    }
}