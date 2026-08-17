namespace AiSupportHub.Api.Models;

public class ChatResponse
{
    public string Message { get; set; } = string.Empty;

    public int ConversationId { get; set; }

    public DateTime Timestamp { get; set; }
}