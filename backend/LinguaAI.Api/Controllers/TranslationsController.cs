using LinguaAI.Application.Translations;
using Microsoft.AspNetCore.Mvc;

namespace LinguaAI.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TranslationsController(ITranslationService translationService) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(TranslateResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TranslateResponse>> Translate(
        [FromBody] TranslateRequest request,
        CancellationToken cancellationToken)
    {
        var result = await translationService.TranslateAsync(request, cancellationToken);

        return Ok(new TranslateResponse
        {
            TranslatedText = result.TranslatedText,
            SourceLanguage = result.SourceLanguage,
            TargetLanguage = result.TargetLanguage,
            Style = result.Style
        });
    }
}
