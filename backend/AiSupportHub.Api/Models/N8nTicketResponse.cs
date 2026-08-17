namespace AiSupportHub.Api.Models;

public class N8nTicketResponse
{
    public bool Success { get; set; }

    public bool EmailSent { get; set; }

    public string? TicketNumber { get; set; }

    public string? Message { get; set; }
}