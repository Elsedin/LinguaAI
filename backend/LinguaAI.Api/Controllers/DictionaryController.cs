using LinguaAI.Api.Auth;
using LinguaAI.Application.Dictionary;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LinguaAI.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/dictionary")]
public class DictionaryController(IDictionaryService dictionaryService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(PagedDictionaryResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PagedDictionaryResult>> List(
        [FromQuery] DictionaryQuery query,
        CancellationToken cancellationToken)
    {
        var userId = CurrentUserId.From(User);
        if (userId is null)
            return Unauthorized();

        return Ok(await dictionaryService.ListAsync(userId, query, cancellationToken));
    }

    [HttpPost]
    [ProducesResponseType(typeof(DictionaryEntryDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<DictionaryEntryDto>> Create(
        [FromBody] CreateDictionaryEntryRequest request,
        CancellationToken cancellationToken)
    {
        var userId = CurrentUserId.From(User);
        if (userId is null)
            return Unauthorized();

        var entry = await dictionaryService.CreateAsync(userId, request, cancellationToken);
        return Created($"/api/dictionary/{entry.Id}", entry);
    }
}
