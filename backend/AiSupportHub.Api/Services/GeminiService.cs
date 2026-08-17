using AiSupportHub.Api.Models;
using System.Net.Http.Json;
using System.Text.Json;

namespace AiSupportHub.Api.Services;

public class GeminiService : IAIService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public GeminiService(
        HttpClient httpClient,
        IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<string> GetResponseAsync(
        IEnumerable<Message> messages,
        string? documentContext = null)
    {
        var apiKey = _configuration["Gemini:ApiKey"];

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new Exception(
                "Gemini API Key no configurada."
            );
        }

        var url =
            $"https://generativelanguage.googleapis.com/v1beta/models/gemini-3.6-flash:generateContent?key={apiKey}";

        var contents = messages.Select(message => new
        {
            role = message.Role == "assistant"
                ? "model"
                : "user",

            parts = new[]
            {
                new
                {
                    text = message.Content
                }
            }
        }).ToArray();

        // Primero construimos el prompt del sistema
        var systemPrompt = """
            Eres un asistente de soporte empresarial.
            Responde de forma clara, profesional y breve.

            Cuando se proporcione CONTEXTO DOCUMENTAL:
            - Utiliza ese contenido para responder.
            - No inventes información que no aparezca en los documentos.
            - Si la respuesta no está en los documentos, indícalo claramente.
            """;

        // Si existen documentos, agregamos su contenido
        if (!string.IsNullOrWhiteSpace(documentContext))
        {
            systemPrompt += $"""

                CONTEXTO DOCUMENTAL:

                {documentContext}
                """;
        }

        // Después construimos el body para Gemini
        var requestBody = new
        {
            systemInstruction = new
            {
                parts = new[]
                {
                    new
                    {
                        text = systemPrompt
                    }
                }
            },

            contents
        };

        for (int attempt = 1; attempt <= 3; attempt++)
        {
            var response =
                await _httpClient.PostAsJsonAsync(
                    url,
                    requestBody
                );

            var json =
                await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                using var document =
                    JsonDocument.Parse(json);

                return document.RootElement
                    .GetProperty("candidates")[0]
                    .GetProperty("content")
                    .GetProperty("parts")[0]
                    .GetProperty("text")
                    .GetString()
                    ?? "No pude generar una respuesta.";
            }

            if ((int)response.StatusCode == 503 &&
                attempt < 3)
            {
                await Task.Delay(attempt * 2000);

                continue;
            }

            throw new Exception(
                $"Error Gemini: {(int)response.StatusCode} - {json}"
            );
        }

        return "El servicio de IA no está disponible.";
    }
}