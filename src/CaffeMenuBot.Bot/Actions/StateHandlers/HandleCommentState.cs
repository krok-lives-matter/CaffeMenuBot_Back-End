using System.Threading;
using System.Threading.Tasks;
using CaffeMenuBot.Bot.Actions.Interface;
using CaffeMenuBot.Data;
using CaffeMenuBot.Data.Models.Bot;
using CaffeMenuBot.Data.Models.Reviews;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace CaffeMenuBot.Bot.Actions.Сommands
{
    public sealed class HandleCommentState : IStateAction
    {
        private CaffeMenuBotContext Context { get; }
        private ITelegramBotClient Client { get; }

        private const ChatState stateType = ChatState.pending_comment;
        private const string titleMessage = "Дякую за ваш відгук! Натисніть на будь-яку з опцій оцінювання для завершення операції";

        public HandleCommentState(CaffeMenuBotContext context, ITelegramBotClient client)
        {
            Context = context;
            Client = client;
        }

        public bool Contains(BotUser user, Update update)
        {
            return user.State == stateType;
        }

        public async Task ExecuteAsync(BotUser user, Update update, CancellationToken ct)
        {
            var review = await Context.Reviews.FirstOrDefaultAsync(r => r.User.Id == user.Id);

            if (review == null)
            {
                review = new Review()
                {
                    User = user,
                    ReviewComment = update.Message.Text
                };
                Context.Reviews.Add(review);
            }
            else
                review.ReviewComment = update.Message.Text;


            user.State = ChatState.default_state;
            Context.Entry(user).State = EntityState.Modified;

            await Context.SaveChangesAsync();

            await Client.SendTextMessageAsync(
                update.Message.Chat.Id,
                titleMessage,
                cancellationToken: ct);
        }
    }
}