using Telegram.Bot.Types.ReplyMarkups;

namespace CaffeMenuBot.Bot.Keyboards.Reply
{
    public static class MenuKeyboard
    {
        public static readonly ReplyKeyboardMarkup MainMenu = new ReplyKeyboardMarkup(
            new[]
            {
                new[]
                {
                    new KeyboardButton("Меню"),
                },
                new[]
                {
                    new KeyboardButton("Графік роботи")
                },
                new[]
                {
                    new KeyboardButton("Оцінити роботу")
                }
            })
        { ResizeKeyboard = true };
    }
}