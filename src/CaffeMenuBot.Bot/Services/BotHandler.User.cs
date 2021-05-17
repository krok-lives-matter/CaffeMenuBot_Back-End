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
