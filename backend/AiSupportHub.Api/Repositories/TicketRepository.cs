using AiSupportHub.Api.Models;
using Dapper;
using Microsoft.Data.SqlClient;

namespace AiSupportHub.Api.Repositories;

public class TicketRepository : ITicketRepository
{
    private readonly string _connectionString;

    public TicketRepository(
        IConfiguration configuration)
    {
        _connectionString =
            configuration.GetConnectionString("DefaultConnection")
            ?? throw new Exception(
                "Connection string no configurada."
            );
    }

    public async Task<int> CreateTicketAsync(
        Ticket ticket)
    {
        const string sql = """
            INSERT INTO dbo.Tickets
            (
                ConversationId,
                TicketNumber,
                Name,
                Email,
                Description,
                Status,
                CreatedAt
            )
            VALUES
            (
                @ConversationId,
                @TicketNumber,
                @Name,
                @Email,
                @Description,
                @Status,
                GETUTCDATE()
            );

            SELECT CAST(SCOPE_IDENTITY() AS INT);
            """;

        await using var connection =
            new SqlConnection(_connectionString);

        return await connection.ExecuteScalarAsync<int>(
            sql,
            ticket
        );
    }

    public async Task UpdateTicketNumberAsync(
    int id,
    string ticketNumber)
    {
        const string sql = """
        UPDATE dbo.Tickets
        SET TicketNumber = @TicketNumber
        WHERE Id = @Id;
        """;

        await using var connection =
            new SqlConnection(_connectionString);

        await connection.ExecuteAsync(
            sql,
            new
            {
                Id = id,
                TicketNumber = ticketNumber
            });
    }

    public async Task<IEnumerable<Ticket>> GetTicketsAsync()
    {
        const string sql = """
        SELECT
            Id,
            ConversationId,
            TicketNumber,
            Name,
            Email,
            Description,
            Status,
            CreatedAt
        FROM dbo.Tickets
        ORDER BY CreatedAt DESC;
        """;

        await using var connection =
            new SqlConnection(_connectionString);

        return await connection.QueryAsync<Ticket>(sql);
    }

    public async Task UpdateStatusAsync(
    int id,
    string status)
    {
        const string sql = """
        UPDATE dbo.Tickets
        SET Status = @Status
        WHERE Id = @Id;
        """;

        await using var connection =
            new SqlConnection(_connectionString);

        await connection.ExecuteAsync(
            sql,
            new
            {
                Id = id,
                Status = status
            }
        );
    }

    public async Task<int> CountTicketsAsync()
    {
        const string sql = """
        SELECT COUNT(*)
        FROM dbo.Tickets;
        """;

        await using var connection =
            new SqlConnection(_connectionString);

        return await connection.ExecuteScalarAsync<int>(sql);
    }

    public async Task<int> CountTicketsByStatusAsync(
    string status)
    {
        const string sql = """
        SELECT COUNT(*)
        FROM dbo.Tickets
        WHERE Status = @Status;
        """;

        await using var connection =
            new SqlConnection(_connectionString);

        return await connection.ExecuteScalarAsync<int>(
            sql,
            new { Status = status }
        );
    }
    public async Task<Ticket?> GetTicketByIdAsync(int id)
    {
        const string sql = """
        SELECT
            Id,
            ConversationId,
            TicketNumber,
            Name,
            Email,
            Description,
            Status,
            CreatedAt
        FROM dbo.Tickets
        WHERE Id = @Id;
        """;

        await using var connection =
            new SqlConnection(_connectionString);

        return await connection.QueryFirstOrDefaultAsync<Ticket>(
            sql,
            new { Id = id }
        );
    }
}