namespace AiSupportHub.Api.Models;

public class Conversation
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public string? SupportStatus { get; set; }

    public string? SupportName { get; set; }

    public string? SupportEmail { get; set; }

    public string? SupportDescription { get; set; }
}