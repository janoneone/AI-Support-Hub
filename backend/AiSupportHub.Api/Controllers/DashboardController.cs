using AiSupportHub.Api.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AiSupportHub.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IConversationRepository _conversationRepository;

    public DashboardController(
        ITicketRepository ticketRepository,
        IConversationRepository conversationRepository)
    {
        _ticketRepository = ticketRepository;
        _conversationRepository = conversationRepository;
    }

    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary()
    {
        var totalTickets =
            await _ticketRepository.CountTicketsAsync();

        var openTickets =
            await _ticketRepository
                .CountTicketsByStatusAsync("OPEN");

        var closedTickets =
            await _ticketRepository
                .CountTicketsByStatusAsync("CLOSED");

        var conversations =
            await _conversationRepository
                .GetConversationsAsync();

        return Ok(new
        {
            totalTickets,
            openTickets,
            closedTickets,
            conversations =
                conversations.Count()
        });
    }
}