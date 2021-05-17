using System;
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
    public sealed class HandleRatingAction : IChatAction
    {
        private CaffeMenuBotContext Context { get; }
        private ITelegramBotClient Client { get; }
        
        private const string CALLBACK_ID = "RRR";
        private const string CALLBACK_ANSWER_MESSAGE = "Дякую за ваш відгук!";

        public HandleRatingAction(CaffeMenuBotContext context, ITelegramBotClient client)
        {
            Context = context;
            Client = client;
        }

        public bool Contains(BotUser user, Update update)
        {
            if (update.CallbackQuery == null) return false;
            return update.CallbackQuery.Data.StartsWith(CALLBACK_ID);
        }

        public async Task ExecuteAsync(BotUser user, Update update, CancellationToken ct)
        {
            Rating userRating = ParseRating(update);

            var review = await Context.Reviews.FirstOrDefaultAsync(r => r.User.Id == user.Id);

            if (review == null)
            {
                review = new Review()
                {
                    User = user,
                    Rating = userRating
                };
                Context.Reviews.Add(review);
            }
            else
                review.Rating = userRating;

            await Context.SaveChangesAsync();

            await Client.AnswerCallbackQueryAsync(
                update.CallbackQuery.Id,
                CALLBACK_ANSWER_MESSAGE,
                cancellationToken: ct);

            await Client.DeleteMessageAsync(
                update.CallbackQuery.From.Id,
                update.CallbackQuery.Message.MessageId,
                cancellationToken: ct);
        }

        /// <summary>
        /// parses rating from callback data string back to Rating type
        /// </summary>
        /// <param name="update"></param>
        /// <returns>Rating</returns>
        private Rating ParseRating(Update update)
        {
            string rawRating = update.CallbackQuery.Data.Split()[1];
            return (Rating)Enum.Parse(typeof(Rating), rawRating, true);
        }
    }
}