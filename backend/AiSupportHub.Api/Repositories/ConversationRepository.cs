using AiSupportHub.Api.Models;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.VisualBasic;

namespace AiSupportHub.Api.Repositories;

public class ConversationRepository : IConversationRepository
{
    private readonly string _connectionString;

    public ConversationRepository(IConfiguration configuration)
    {
        _connectionString =
            configuration.GetConnectionString("DefaultConnection")
            ?? throw new Exception(
                "Connection string no configurada."
            );
    }

    public async Task<int> CreateConversationAsync(string title)
    {
        const string sql = """
        INSERT INTO dbo.Conversations
            (Title, CreatedAt, SupportStatus)
        VALUES
            (@Title, GETUTCDATE(), 'NORMAL');

        SELECT CAST(SCOPE_IDENTITY() AS INT);
        """;

        await using var connection =
            new SqlConnection(_connectionString);

        return await connection.ExecuteScalarAsync<int>(
            sql,
            new { Title = title }
        );
    }

    public async Task<Conversation?> GetConversationAsync(int id)
    {
        const string sql = """
            SELECT
                Id,
                Title,
                CreatedAt,
                SupportStatus,
                SupportName,
                SupportEmail,
                SupportDescription
            FROM dbo.Conversations
            WHERE Id = @Id;
            """;

        await using var connection =
            new SqlConnection(_connectionString);

        return await connection.QueryFirstOrDefaultAsync<Conversation>(
            sql,
            new { Id = id }
        );
    }

    public async Task SaveMessageAsync(int conversationId,string role,string content)
    {
        const string sql = """
            INSERT INTO Messages
                (ConversationId, Role, Content, CreatedAt)
            VALUES
                (@ConversationId, @Role, @Content, GETUTCDATE());
            """;

        await using var connection =
            new SqlConnection(_connectionString);

        await connection.ExecuteAsync(
            sql,
            new
            {
                ConversationId = conversationId,
                Role = role,
                Content = content
            }
        );
    }

    public async Task<IEnumerable<Message>> GetMessagesAsync(int conversationId)
    {
        const string sql = """
        SELECT
            Id,
            ConversationId,
            Role,
            Content,
            CreatedAt
        FROM dbo.Messages
        WHERE ConversationId = @ConversationId
        ORDER BY CreatedAt, Id;
        """;

        await using var connection =
            new SqlConnection(_connectionString);

        return await connection.QueryAsync<Message>(
            sql,
            new
            {
                ConversationId = conversationId
            }
        );
    }

    public async Task<IEnumerable<Conversation>> GetConversationsAsync()
    {
        const string sql = """
        SELECT
            Id,
            Title,
            CreatedAt
        FROM dbo.Conversations
        ORDER BY CreatedAt DESC;
        """;

        await using var connection =
            new SqlConnection(_connectionString);

        return await connection.QueryAsync<Conversation>(sql);
    }

    public async Task RenameConversationAsync(int id, string title)
    {
        const string sql = """
        UPDATE dbo.Conversations
        SET Title = @Title
        WHERE Id = @Id;
        """;

        await using var connection =
            new SqlConnection(_connectionString);

        await connection.ExecuteAsync(
            sql,
            new
            {
                Id = id,
                Title = title
            }
        );
    }

    public async Task DeleteConversationAsync(int id)
    {
        const string sql = """
        DELETE FROM dbo.Conversations
        WHERE Id = @Id;
        """;

        await using var connection =
            new SqlConnection(_connectionString);

        await connection.ExecuteAsync(
            sql,
            new { Id = id }
        );
    }

    public async Task UpdateSupportStatusAsync(int conversationId, string? status)
    {
        const string sql = """
        UPDATE dbo.Conversations
        SET SupportStatus = @Status
        WHERE Id = @ConversationId;
        """;

        await using var connection =
            new SqlConnection(_connectionString);

        await connection.ExecuteAsync(
            sql,
            new
            {
                ConversationId = conversationId,
                Status = status
            }
        );
    }

    public async Task UpdateSupportNameAsync(int conversationId,string name)
    {
        const string sql = """
        UPDATE dbo.Conversations
        SET SupportName = @Name
        WHERE Id = @ConversationId;
        """;

        await using var connection =
            new SqlConnection(_connectionString);

        await connection.ExecuteAsync(
            sql,
            new
            {
                ConversationId = conversationId,
                Name = name
            });
    }
    public async Task UpdateSupportEmailAsync(int conversationId,string email)
    {
        const string sql = """
        UPDATE dbo.Conversations
        SET SupportEmail = @Email
        WHERE Id = @ConversationId;
        """;

        await using var connection =
            new SqlConnection(_connectionString);

        await connection.ExecuteAsync(
            sql,
            new
            {
                ConversationId = conversationId,
                Email = email
            });
    }
    public async Task UpdateSupportDescriptionAsync(int conversationId,string description)
    {
        const string sql = """
        UPDATE dbo.Conversations
        SET SupportDescription = @Description
        WHERE Id = @ConversationId;
        """;

        await using var connection =
            new SqlConnection(_connectionString);

        await connection.ExecuteAsync(
            sql,
            new
            {
                ConversationId = conversationId,
                Description = description
            });
    }

}