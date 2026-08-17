using System.Net.Http.Json;
using System.Text.Json;

namespace AiSupportHub.Api.Services;

public class SupportFlowService : ISupportFlowService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public SupportFlowService(
        HttpClient httpClient,
        IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<bool> IsSupportRequestAsync(string message)
    {
        var apiKey = _configuration["Gemini:ApiKey"];

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            return false;
        }

        var url =
            $"https://generativelanguage.googleapis.com/v1beta/models/gemini-3.6-flash:generateContent?key={apiKey}";

        var prompt = $"""
            Clasifica el siguiente mensaje.

            Debes responder SOLO con una de estas palabras:

            SUPPORT
            NORMAL

            Usa SUPPORT cuando el usuario:
            - tenga un problema técnico
            - no pueda acceder a un sistema
            - reporte un error
            - necesite asistencia
            - quiera generar una solicitud de soporte

            Usa NORMAL cuando sea:
            - una pregunta general
            - una conversación normal
            - una consulta informativa

            Mensaje:
            {message}
            """;

        var requestBody = new
        {
            contents = new[]
            {
                new
                {
                    role = "user",
                    parts = new[]
                    {
                        new
                        {
                            text = prompt
                        }
                    }
                }
            }
        };

        var response =
            await _httpClient.PostAsJsonAsync(
                url,
                requestBody
            );

        if (!response.IsSuccessStatusCode)
        {
            return false;
        }

        var json =
            await response.Content.ReadAsStringAsync();

        using var document =
            JsonDocument.Parse(json);

        var result = document.RootElement
            .GetProperty("candidates")[0]
            .GetProperty("content")
            .GetProperty("parts")[0]
            .GetProperty("text")
            .GetString()
            ?.Trim()
            .ToUpperInvariant();

        return result == "SUPPORT";
    }
}