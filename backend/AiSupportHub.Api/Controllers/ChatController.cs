using AiSupportHub.Api.Models;
using AiSupportHub.Api.Repositories;
using AiSupportHub.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace AiSupportHub.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
    private readonly IAIService _aiService;
    private readonly IConversationRepository _repository;
    private readonly IDocumentRepository _documentRepository;
    private readonly ISupportFlowService _supportFlowService;
    private readonly ITicketRepository _ticketRepository;
    private readonly IN8nService _n8nService;

    public ChatController(
        IAIService aiService,
        IConversationRepository repository,
        IDocumentRepository documentRepository,
        ISupportFlowService supportFlowService,
        ITicketRepository ticketRepository,
        IN8nService n8nService)
    {
        _aiService = aiService;
        _repository = repository;
        _documentRepository = documentRepository;
        _supportFlowService = supportFlowService;
        _ticketRepository = ticketRepository;
        _n8nService = n8nService;
    }

    [HttpPost]
    public async Task<ActionResult<ChatResponse>> SendMessage(
        ChatRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Message))
        {
            return BadRequest("El mensaje es obligatorio.");
        }

        int conversationId;

        // 1. Obtener o crear conversación
        if (request.ConversationId.HasValue)
        {
            var conversation =
                await _repository.GetConversationAsync(
                    request.ConversationId.Value
                );

            if (conversation == null)
            {
                return NotFound("Conversación no encontrada.");
            }

            conversationId = conversation.Id;
        }
        else
        {
            var title = request.Message.Length > 40
                ? request.Message[..40]
                : request.Message;

            conversationId =
                await _repository.CreateConversationAsync(title);
        }

        // 2. Obtener estado actual de soporte
        var currentConversation =
            await _repository.GetConversationAsync(
                conversationId
            );

        var status =
            currentConversation?.SupportStatus
            ?? "NORMAL";

        // 3. Guardar mensaje del usuario
        await _repository.SaveMessageAsync(
            conversationId,
            "user",
            request.Message
        );

        // ==================================================
        // FLUJO DE SOPORTE
        // ==================================================

        // Estado normal: detectar intención
        if (status == "NORMAL")
        {
            var isSupportRequest =
                await _supportFlowService
                    .IsSupportRequestAsync(
                        request.Message
                    );

            if (isSupportRequest)
            {

                await _repository.UpdateSupportStatusAsync(
                    conversationId,
                    "WAITING_NAME"
                );

                var responseText =
                    "Entiendo. Voy a ayudarte a generar una solicitud de soporte. ¿Cuál es tu nombre?";

                await SaveAssistantMessage(
                    conversationId,
                    responseText
                );

                return CreateResponse(
                    conversationId,
                    responseText
                );
            }
        }

        // Esperando nombre
        if (status == "WAITING_NAME")
        {
            await _repository.UpdateSupportNameAsync(
                conversationId,
                request.Message.Trim()
            );
            await _repository.UpdateSupportStatusAsync(
                conversationId,
                "WAITING_EMAIL"
            );

            var responseText =
                $"Gracias, {request.Message}. ¿Cuál es tu correo electrónico?";

            await SaveAssistantMessage(
                conversationId,
                responseText
            );

            return CreateResponse(
                conversationId,
                responseText
            );
        }

        // Esperando correo
        if (status == "WAITING_EMAIL")
        {
            // Por ahora validación simple
            if (!request.Message.Contains("@"))
            {
                var invalidEmail =
                    "Ese correo no parece válido. Por favor ingresa un correo electrónico válido.";

                await SaveAssistantMessage(
                    conversationId,
                    invalidEmail
                );

                return CreateResponse(
                    conversationId,
                    invalidEmail
                );
            }
            await _repository.UpdateSupportEmailAsync(
                conversationId,
                request.Message.Trim()
            );

            await _repository.UpdateSupportStatusAsync(
                conversationId,
                "WAITING_DESCRIPTION"
            );

            var responseText =
                "Perfecto. Describe brevemente el problema que estás teniendo.";

            await SaveAssistantMessage(
                conversationId,
                responseText
            );

            return CreateResponse(
                conversationId,
                responseText
            );
        }

        // Esperando descripción
        if (status == "WAITING_DESCRIPTION")
        {
            await _repository.UpdateSupportDescriptionAsync(
                conversationId,
                request.Message.Trim()
            );

            var conversation =
                await _repository.GetConversationAsync(
                    conversationId
                );

            if (conversation == null)
            {
                return NotFound(
                    "Conversación no encontrada."
                );
            }

            var ticket = new Ticket
            {
                ConversationId = conversationId,
                Name = conversation.SupportName ?? "",
                Email = conversation.SupportEmail ?? "",
                Description = request.Message.Trim(),
                Status = "OPEN"
            };

            var ticketId =
                await _ticketRepository.CreateTicketAsync(
                    ticket
                );

            var ticketNumber =
                $"SUP-{ticketId:D6}";

            await _ticketRepository.UpdateTicketNumberAsync(
                ticketId,
                ticketNumber
            );

            ticket.Id = ticketId;
            ticket.TicketNumber = ticketNumber;
            ticket.CreatedAt = DateTime.UtcNow;

            await _repository.UpdateSupportStatusAsync(
                conversationId,
                "TICKET_CREATED"
            );

            N8nTicketResponse? automationResult = null;

            try
            {
                automationResult =
                    await _n8nService.SendTicketAsync(ticket);
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"Error enviando ticket a n8n: {ex.Message}"
                );
            }

            string responseText;

            if (automationResult?.Success == true &&
                automationResult.EmailSent)
            {
                responseText =
                    $"Tu ticket de soporte {ticketNumber} fue creado correctamente. " +
                    $"Enviamos una confirmación a {ticket.Email}.";
            }
            else
            {
                responseText =
                    $"Tu ticket de soporte {ticketNumber} fue creado correctamente, " +
                    "pero no fue posible confirmar el envío del correo.";
            }

            await SaveAssistantMessage(
                conversationId,
                responseText
            );

            return CreateResponse(
                conversationId,
                responseText
            );
        }

        // ==================================================
        // CHAT NORMAL
        // ==================================================

        var messages =
            await _repository.GetMessagesAsync(
                conversationId
            );

        var documentContext =
            await _documentRepository.GetAllContentAsync();

        var aiResponse =
            await _aiService.GetResponseAsync(
                messages,
                documentContext
            );

        await SaveAssistantMessage(
            conversationId,
            aiResponse
        );

        return CreateResponse(
            conversationId,
            aiResponse
        );
    }

    private async Task SaveAssistantMessage(
        int conversationId,
        string message)
    {
        await _repository.SaveMessageAsync(
            conversationId,
            "assistant",
            message
        );
    }

    private ActionResult<ChatResponse> CreateResponse(
        int conversationId,
        string message)
    {
        return Ok(new ChatResponse
        {
            Message = message,
            ConversationId = conversationId,
            Timestamp = DateTime.UtcNow
        });
    }
}