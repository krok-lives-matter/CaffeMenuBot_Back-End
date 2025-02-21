using Telegram.Bot.Types.ReplyMarkups;

namespace CaffeMenuBot.Bot.Keyboards.Reply
{
    /// <summary>
    /// Used as main menu keyboard of bot
    /// </summary>
    public static class MenuKeyboard
    {
        public static readonly ReplyKeyboardMarkup MainMenu = new ReplyKeyboardMarkup(
            new[]
            {
                new[]
                {
                    new KeyboardButton("Menu"),
                },
                new[]
                {
                    new KeyboardButton("Schedule")
                },
                new[]
                {
                    new KeyboardButton("Reviews")
                },
                new[]
                {
                    new KeyboardButton("Rate")
                }
            })
        { ResizeKeyboard = true };
    }
}