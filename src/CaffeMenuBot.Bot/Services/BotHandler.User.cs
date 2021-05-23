using CaffeMenuBot.Data;
using CaffeMenuBot.Data.Models.Bot;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace CaffeMenuBot.Bot.Services
{
    partial class BotHandler
    {
        /// <summary>
        /// gets user object from context
        /// if user is not found in db - created the user and returns created object
        /// </summary>
        /// <param name="context">db context</param>
        /// <param name="update">update from telegram API</param>
        /// <param name="cancellationToken"></param>
        /// <returns>BotUser object</returns>
        private async Task<BotUser> HandleUser(CaffeMenuBotContext context, Update update, CancellationToken cancellationToken)
        {
            // get id either from callback or from messsage
            long telegramId = update.Message == null ? update.CallbackQuery.From.Id : update.Message.From.Id;

            var user = await context.BotUsers.FirstOrDefaultAsync(u => u.Id == telegramId, cancellationToken);

            if (user == null)
            {
                user = new BotUser
                {
                    Id = update.Message.From.Id
                };
                context.BotUsers.Add(user);
                await context.SaveChangesAsync(cancellationToken);
            }
            return user;
        }
    }
}
