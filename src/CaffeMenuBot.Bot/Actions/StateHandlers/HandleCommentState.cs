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


        // Contains uses this instead of COMMAND_NAME
        private const ChatState STATE_TYPE = ChatState.pending_comment;
        private const string MESSAGE_TITLE = "Дякую за ваш відгук! Натисніть на будь-яку з опцій оцінювання для завершення операції";

        public HandleCommentState(CaffeMenuBotContext context, ITelegramBotClient client)
        {
            Context = context;
            Client = client;
        }

        public bool Contains(BotUser user, Update update)
        {
            return user.State == STATE_TYPE;
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
                MESSAGE_TITLE,
                cancellationToken: ct);
        }
    }
}