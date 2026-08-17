using AiSupportHub.Api.Models;
using System.Net.Http.Json;

namespace AiSupportHub.Api.Services;

public class N8nService : IN8nService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public N8nService(
        HttpClient httpClient,
        IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<N8nTicketResponse> SendTicketAsync(
        Ticket ticket)
    {
        var webhookUrl =
            _configuration["N8n:SupportWebhookUrl"];

        if (string.IsNullOrWhiteSpace(webhookUrl))
        {
            throw new Exception(
                "Webhook de n8n no configurado."
            );
        }

        var payload = new
        {
            ticketNumber = ticket.TicketNumber,
            conversationId = ticket.ConversationId,
            name = ticket.Name,
            email = ticket.Email,
            description = ticket.Description,
            status = ticket.Status,
            createdAt = ticket.CreatedAt
        };

        var response =
            await _httpClient.PostAsJsonAsync(
                webhookUrl,
                payload
            );

        if (!response.IsSuccessStatusCode)
        {
            return new N8nTicketResponse
            {
                Success = false,
                EmailSent = false,
                TicketNumber = ticket.TicketNumber,
                Message =
                    $"n8n respondió con HTTP {(int)response.StatusCode}"
            };
        }

        var result =
            await response.Content
                .ReadFromJsonAsync<N8nTicketResponse>();

        return result ?? new N8nTicketResponse
        {
            Success = false,
            EmailSent = false,
            TicketNumber = ticket.TicketNumber,
            Message = "n8n no devolvió una respuesta válida."
        };
    }
}