using AiSupportHub.Api.Repositories;
using Microsoft.AspNetCore.Mvc;
using AiSupportHub.Api.Models;

namespace AiSupportHub.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ConversationsController : ControllerBase
{
    private readonly IConversationRepository _repository;

    public ConversationsController(
        IConversationRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<IActionResult> GetConversations()
    {
        var conversations =
            await _repository.GetConversationsAsync();

        return Ok(conversations);
    }

    [HttpGet("{id}/messages")]
    public async Task<IActionResult> GetMessages(int id)
    {
        var conversation =
            await _repository.GetConversationAsync(id);

        if (conversation == null)
        {
            return NotFound();
        }

        var messages =
            await _repository.GetMessagesAsync(id);

        return Ok(messages);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> RenameConversation(
    int id,
    [FromBody] RenameConversationRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
        {
            return BadRequest("El título es obligatorio.");
        }

        var conversation =
            await _repository.GetConversationAsync(id);

        if (conversation == null)
        {
            return NotFound();
        }

        await _repository.RenameConversationAsync(
            id,
            request.Title.Trim());

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteConversation(int id)
    {
        var conversation =
            await _repository.GetConversationAsync(id);

        if (conversation == null)
        {
            return NotFound();
        }

        await _repository.DeleteConversationAsync(id);

        return NoContent();
    }
}