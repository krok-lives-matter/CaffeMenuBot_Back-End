using CaffeMenuBot.Data.Models.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace CaffeMenuBot.Bot.Keyboards.Inline
{
    public static class RateKeyboard
    {
        /// <summary>
        /// "RRR" identifier is used for tracking which callback should fire
        /// Second parameter after space character in CallbackData is string representation of rating that user have selected
        /// </summary>
        public static InlineKeyboardMarkup GetRateKeyboard =>
            new InlineKeyboardMarkup(new[]
            {
                new[] 
                {
                    new InlineKeyboardButton{Text = "😡", CallbackData = "RRR " + Rating.rating_bad.ToString()},
                    new InlineKeyboardButton{Text = "😐", CallbackData = "RRR " + Rating.rating_ok.ToString()},
                    new InlineKeyboardButton{Text = "🙂", CallbackData = "RRR " + Rating.rating_good.ToString()},
                    new InlineKeyboardButton{Text = "😀", CallbackData = "RRR " + Rating.rating_great.ToString()},
                    new InlineKeyboardButton{Text = "😍", CallbackData = "RRR " + Rating.rating_excellent.ToString()},
                },
                new[] {new InlineKeyboardButton{Text = "✉", CallbackData = "CCC"} }
            });
    }
}
