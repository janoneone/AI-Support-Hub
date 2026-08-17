using AiSupportHub.Api.Models;

namespace AiSupportHub.Api.Repositories;

public interface IDocumentRepository
{
    Task<int> CreateDocumentAsync(
        string fileName,
        string content);

    Task<IEnumerable<Document>> GetDocumentsAsync();

    Task<string> GetAllContentAsync();
}