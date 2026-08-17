using AiSupportHub.Api.Models;

namespace AiSupportHub.Api.Services;

public interface IAIService
{
    Task<string> GetResponseAsync(
        IEnumerable<Message> messages,
        string? documentContext = null);
}