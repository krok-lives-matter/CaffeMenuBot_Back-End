namespace CaffeMenuBot.Shared.Models.Auth
{
    public sealed record LoginResponseModel
    {
        public string Token { get; init; }
    }
}