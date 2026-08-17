using AiSupportHub.Api.Repositories;
using Microsoft.AspNetCore.Mvc;
using UglyToad.PdfPig;

namespace AiSupportHub.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DocumentsController : ControllerBase
{
    private readonly IDocumentRepository _repository;

    public DocumentsController(
        IDocumentRepository repository)
    {
        _repository = repository;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("Debe seleccionar un archivo.");
        }

        if (!Path.GetExtension(file.FileName)
            .Equals(".pdf", StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest(
                "Por ahora solo se permiten archivos PDF."
            );
        }

        await using var stream =
            file.OpenReadStream();

        using var pdf =
            PdfDocument.Open(stream);

        var text = string.Join(
            Environment.NewLine,
            pdf.GetPages()
               .Select(page => page.Text)
        );

        if (string.IsNullOrWhiteSpace(text))
        {
            return BadRequest(
                "No fue posible extraer texto del PDF."
            );
        }

        var id =
            await _repository.CreateDocumentAsync(
                file.FileName,
                text
            );

        return Ok(new
        {
            id,
            fileName = file.FileName,
            message = "Documento procesado correctamente."
        });
    }

    [HttpGet]
    public async Task<IActionResult> GetDocuments()
    {
        var documents =
            await _repository.GetDocumentsAsync();

        return Ok(documents);
    }
}