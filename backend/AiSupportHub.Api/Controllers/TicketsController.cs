using AiSupportHub.Api.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AiSupportHub.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TicketsController : ControllerBase
{
    private readonly ITicketRepository _repository;

    public TicketsController(
        ITicketRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<IActionResult> GetTickets()
    {
        var tickets =
            await _repository.GetTicketsAsync();

        return Ok(tickets);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTicket(int id)
    {
        var ticket =
            await _repository.GetTicketByIdAsync(id);

        if (ticket == null)
        {
            return NotFound();
        }

        return Ok(ticket);
    }

    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateStatus(
        int id,
        [FromBody] UpdateTicketStatusRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Status))
        {
            return BadRequest(
                "El estado es obligatorio."
            );
        }

        var allowedStatuses =
            new[] { "OPEN", "IN_PROGRESS", "CLOSED" };

        var status =
            request.Status.Trim().ToUpperInvariant();

        if (!allowedStatuses.Contains(status))
        {
            return BadRequest(
                "Estado inválido."
            );
        }

        var ticket =
            await _repository.GetTicketByIdAsync(id);

        if (ticket == null)
        {
            return NotFound();
        }

        await _repository.UpdateStatusAsync(
            id,
            status
        );

        return NoContent();
    }
}

public class UpdateTicketStatusRequest
{
    public string Status { get; set; } = string.Empty;
}