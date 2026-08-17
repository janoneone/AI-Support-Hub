namespace AiSupportHub.Api.Models;

public class ChatRequest
{
    public string Message { get; set; } = string.Empty;

    public int? ConversationId { get; set; }
}