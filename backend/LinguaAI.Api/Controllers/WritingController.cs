using LinguaAI.Api.RateLimiting;
using LinguaAI.Application.Writing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace LinguaAI.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WritingController(IWritingService writingService) : ControllerBase
{
    [HttpPost]
    [EnableRateLimiting(TranslationRateLimiting.PolicyName)]
    [ProducesResponseType(typeof(WritingResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<WritingResponse>> Transform(
        [FromBody] WritingRequest request,
        CancellationToken cancellationToken)
    {
        var result = await writingService.TransformAsync(request, cancellationToken);

        return Ok(new WritingResponse
        {
            ResultText = result.ResultText,
            Operation = result.Operation,
            Language = result.Language
        });
    }
}
