namespace CaffeMenuBot.AppHost.Models.DTO.Responses
{
    public sealed record LoadInfoResponse
    {
        public double Avg1 { get; init; }
        public double Avg2 { get; init; }
        public double Avg3 { get; init; }
        public double Avg4 { get; init; }
        public double Avg5 { get; init; }
    }
}