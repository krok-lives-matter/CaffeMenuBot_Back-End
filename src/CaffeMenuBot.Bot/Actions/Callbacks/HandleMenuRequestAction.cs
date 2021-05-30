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

namespace CaffeMenuBot.Bot.Actions.Callbacks
{
    public sealed class HandleMenuRequestAction : IChatAction
    {
        private readonly IPdfGeneratorService _pdfGeneratorService;
        private readonly CaffeMenuBotContext _context;
        private readonly ITelegramBotClient _client;
        
        // Contains uses this identifier instead of COMMAND_NAME
        private const string CALLBACK_ID = "CAT";
        private const string MESSAGE_TITLE = "Press to open file";

        public HandleMenuRequestAction(ITelegramBotClient client, IPdfGeneratorService pdfGeneratorService,
            CaffeMenuBotContext context)
        {
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
                await _client.AnswerCallbackQueryAsync(update.CallbackQuery.Id, "Ошибка!", true,
                    cancellationToken: ct);
                return;
            }

            var category = await _context.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.Id == categoryId, ct);
            if (category == null)
            {
                await _client.AnswerCallbackQueryAsync(update.CallbackQuery.Id, "Не удалось найти выбранную категорию",
                    true, cancellationToken: ct);
                return;
            }

            await using var pdfFileStream = await _pdfGeneratorService.GenerateMenuForCategoryAsync(category, ct);
            await _client.SendDocumentAsync(update.CallbackQuery.Message.Chat,
                new InputOnlineFile(pdfFileStream, $"Меню_{category.CategoryName}.pdf"),
                $"Меню категорії \"{category.CategoryName}\"",
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