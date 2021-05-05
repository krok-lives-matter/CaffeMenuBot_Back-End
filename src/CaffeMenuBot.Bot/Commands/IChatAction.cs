using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace CaffeMenuBot.Bot.Commands {
    public interface IChatAction {
       Task ExecuteAsync(Message message, CancellationToken ct);

       bool Contains(Message message);
    }
}