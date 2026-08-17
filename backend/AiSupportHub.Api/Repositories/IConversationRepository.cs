using AiSupportHub.Api.Models;
using Microsoft.VisualBasic;

namespace AiSupportHub.Api.Repositories;

public interface IConversationRepository
{
    Task<int> CreateConversationAsync(string title);

    Task<Conversation?> GetConversationAsync(int id);
    Task<IEnumerable<Message>> GetMessagesAsync(int conversationId);

    Task<IEnumerable<Conversation>> GetConversationsAsync();

    Task RenameConversationAsync(int id, string title);

    Task DeleteConversationAsync(int id);

    Task SaveMessageAsync(
        int conversationId,
        string role,
        string content);

    Task UpdateSupportStatusAsync(
    int conversationId,
    string? status);

    Task UpdateSupportNameAsync(
    int conversationId,
    string name);

    Task UpdateSupportEmailAsync(
        int conversationId,
        string email);

    Task UpdateSupportDescriptionAsync(
        int conversationId,
        string description);
}