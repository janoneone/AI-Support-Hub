namespace AiSupportHub.Api.Models;

public class Ticket
{
    public int Id { get; set; }

    public int ConversationId { get; set; }

    public string? TicketNumber { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Status { get; set; } = "OPEN";

    public DateTime CreatedAt { get; set; }
}