using CaffeMenuBot.Bot.Actions.Interface;
using CaffeMenuBot.Data;
using CaffeMenuBot.Data.Models.Bot;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace CaffeMenuBot.Bot.Actions.Callbacks
{
    public sealed class AwaitCommentCallback : IChatAction
    {
        private CaffeMenuBotContext Context { get; }
        private ITelegramBotClient Client { get; }
        
        private const string CALLBACK_ID = "CCC";
        private const string titleMessage = "Надішліть ваш відгук використовуючи поле для вводу повідомлення";

        public AwaitCommentCallback(CaffeMenuBotContext context, ITelegramBotClient client)
        {
            Context = context;
            Client = client;
        }

        public bool Contains(BotUser user, Update update)
        {
            if (update.CallbackQuery == null) return false;
            return update.CallbackQuery.Data.Contains(CALLBACK_ID);
        }

        public async Task ExecuteAsync(BotUser user, Update update, CancellationToken ct)
        {
            user.State = ChatState.pending_comment;
            Context.Entry(user).State = EntityState.Modified;
            await Context.SaveChangesAsync();

            await Client.SendTextMessageAsync(
                update.CallbackQuery.From.Id,
                titleMessage,
                cancellationToken: ct);
        }
    }
}