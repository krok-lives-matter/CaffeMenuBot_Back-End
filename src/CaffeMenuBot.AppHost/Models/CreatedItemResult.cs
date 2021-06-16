namespace CaffeMenuBot.AppHost.Models
{
    public sealed record CreatedItemResult
    {
        public int CreatedItemId { get; init; }
    }

    public sealed record CreatedItemStringResult
    {
        public string CreatedItemId { get; set; }
    }
}