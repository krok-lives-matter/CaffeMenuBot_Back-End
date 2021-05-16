using Telegram.Bot.Types.ReplyMarkups;

namespace CaffeMenuBot.Bot.Keyboards.Inline
{
    public static class RateKeyboard
    {
        public static InlineKeyboardMarkup GetRateKeyboard =>
            new InlineKeyboardMarkup(new[]
            {
                new[] 
                {
                    new InlineKeyboardButton{Text = "😡", CallbackData = "rating_bad"},
                    new InlineKeyboardButton{Text = "😐", CallbackData = "rating_ok"},
                    new InlineKeyboardButton{Text = "🙂", CallbackData = "rating_good"},
                    new InlineKeyboardButton{Text = "😀", CallbackData = "rating_great"},
                    new InlineKeyboardButton{Text = "😍", CallbackData = "rating_excellent"},
                },
                new[] {new InlineKeyboardButton{Text = "✉", CallbackData = "comment"}}
            });
    }
}
