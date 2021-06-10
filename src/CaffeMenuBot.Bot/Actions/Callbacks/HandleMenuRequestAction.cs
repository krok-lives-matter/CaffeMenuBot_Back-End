using System;
using System.Threading;
using System.Threading.Tasks;
using CaffeMenuBot.Bot.Actions.Interface;
using CaffeMenuBot.Bot.Services;
using CaffeMenuBot.Data;
using CaffeMenuBot.Data.Models.Bot;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using Microsoft.AspNetCore.Hosting;

namespace CaffeMenuBot.Bot.Actions.Callbacks
{
    public sealed class HandleMenuRequestAction : IChatAction
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IPdfGeneratorService _pdfGeneratorService;
        private readonly CaffeMenuBotContext _context;
        private readonly ITelegramBotClient _client;
        
        // Contains uses this identifier instead of COMMAND_NAME
        private const string CALLBACK_ID = "CAT";

        public HandleMenuRequestAction(ITelegramBotClient client, IPdfGeneratorService pdfGeneratorService,
            CaffeMenuBotContext context, IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
            _pdfGeneratorService = pdfGeneratorService;
            _context = context;
            _client = client;
        }

        public bool Contains(BotUser user, Update update)
        {
            if (update.CallbackQuery == null) return false;
            return update.CallbackQuery.Data.StartsWith(CALLBACK_ID);
        }

        public async Task ExecuteAsync(BotUser user, Update update, CancellationToken ct)
        {
            await _client.SendChatActionAsync(update.CallbackQuery.Message.Chat, ChatAction.UploadDocument, ct);

            var catId = update.CallbackQuery.Data.Split(' ', 2)[^1];
            if (!int.TryParse(catId, out int categoryId))
            {
                await _client.AnswerCallbackQueryAsync(update.CallbackQuery.Id, "Error!", true,
                    cancellationToken: ct);
                return;
            }

            var category = await _context.Categories
                .AsNoTracking()
                .Include(c => c.Dishes)
                .FirstOrDefaultAsync(c => c.Id == categoryId, ct);
            if (category == null)
            {
                await _client.AnswerCallbackQueryAsync(update.CallbackQuery.Id, "Couldn't find selected category",
                    true, cancellationToken: ct);
                return;
            }

            await using var pdfFileStream = await _pdfGeneratorService.GenerateMenuForCategoryAsync(category, _webHostEnvironment, ct);
            await _client.SendDocumentAsync(update.CallbackQuery.Message.Chat,
                new InputOnlineFile(pdfFileStream, $"Menu_{category.CategoryName}.pdf"),
                $"Menu of category \"{category.CategoryName}\"",
                cancellationToken: ct);

            try
            {
                await _client.AnswerCallbackQueryAsync(
                    update.CallbackQuery.Id,
                    cancellationToken: ct);
            }
            catch
            {
                // silent
            }
        }
    }
}