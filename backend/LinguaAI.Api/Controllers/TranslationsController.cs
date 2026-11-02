using LinguaAI.Api.RateLimiting;
using LinguaAI.Application.Translations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace LinguaAI.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TranslationsController(ITranslationService translationService) : ControllerBase
{
    [HttpPost]
    [EnableRateLimiting(TranslationRateLimiting.PolicyName)]
    [ProducesResponseType(typeof(TranslateResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
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
