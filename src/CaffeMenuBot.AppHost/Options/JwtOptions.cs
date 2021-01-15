namespace CaffeMenuBot.AppHost.Options
{
    public sealed record JwtOptions
    {
        public string SecretKey { get; init; } = null!;
        public string Audience { get; init; } = null!;
        public string Issuer { get; init; } = null!;
    }
}
