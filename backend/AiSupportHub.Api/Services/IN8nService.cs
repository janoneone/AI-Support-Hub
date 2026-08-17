using AiSupportHub.Api.Models;

namespace AiSupportHub.Api.Services;

public interface IN8nService
{
    Task<N8nTicketResponse> SendTicketAsync(Ticket ticket);
}