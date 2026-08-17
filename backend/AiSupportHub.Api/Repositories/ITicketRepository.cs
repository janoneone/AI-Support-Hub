using AiSupportHub.Api.Models;

namespace AiSupportHub.Api.Repositories;

public interface ITicketRepository
{
    Task<int> CreateTicketAsync(Ticket ticket);

    Task UpdateTicketNumberAsync(int id,string ticketNumber);

    Task<IEnumerable<Ticket>> GetTicketsAsync();

    Task<Ticket?> GetTicketByIdAsync(int id);

    Task UpdateStatusAsync(
        int id,
        string status);

    Task<int> CountTicketsAsync();

    Task<int> CountTicketsByStatusAsync(
        string status);
}