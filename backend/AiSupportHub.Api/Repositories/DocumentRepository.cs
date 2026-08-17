using AiSupportHub.Api.Models;
using Dapper;
using Microsoft.Data.SqlClient;

namespace AiSupportHub.Api.Repositories;

public class DocumentRepository : IDocumentRepository
{
    private readonly string _connectionString;

    public DocumentRepository(IConfiguration configuration)
    {
        _connectionString =
            configuration.GetConnectionString("DefaultConnection")
            ?? throw new Exception("Connection string no configurada.");
    }

    public async Task<int> CreateDocumentAsync(
        string fileName,
        string content)
    {
        const string sql = """
            INSERT INTO dbo.Documents
                (FileName, Content, UploadedAt)
            VALUES
                (@FileName, @Content, GETUTCDATE());

            SELECT CAST(SCOPE_IDENTITY() AS INT);
            """;

        await using var connection =
            new SqlConnection(_connectionString);

        return await connection.ExecuteScalarAsync<int>(
            sql,
            new
            {
                FileName = fileName,
                Content = content
            }
        );
    }

    public async Task<IEnumerable<Document>> GetDocumentsAsync()
    {
        const string sql = """
            SELECT
                Id,
                FileName,
                UploadedAt
            FROM dbo.Documents
            ORDER BY UploadedAt DESC;
            """;

        await using var connection =
            new SqlConnection(_connectionString);

        return await connection.QueryAsync<Document>(sql);
    }

    public async Task<string> GetAllContentAsync()
    {
        const string sql = """
            SELECT Content
            FROM dbo.Documents
            ORDER BY UploadedAt DESC;
            """;

        await using var connection =
            new SqlConnection(_connectionString);

        var contents =
            await connection.QueryAsync<string>(sql);

        return string.Join(
            Environment.NewLine + Environment.NewLine,
            contents
        );
    }
}