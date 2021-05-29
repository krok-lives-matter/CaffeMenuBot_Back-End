namespace CaffeMenuBot.AppHost.Models.DTO.Responses
{
    public sealed record StatusResponse
    {
        public bool IsRunning { get; init; }
    }
}