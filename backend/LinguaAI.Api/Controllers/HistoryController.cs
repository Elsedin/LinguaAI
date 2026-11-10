using LinguaAI.Api.Auth;
using LinguaAI.Application.History;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LinguaAI.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/history")]
public class HistoryController(IHistoryService historyService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<HistoryItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PagedResult<HistoryItemDto>>> List(
        [FromQuery] HistoryQuery query,
        CancellationToken cancellationToken)
    {
        var userId = CurrentUserId.From(User);
        if (userId is null)
            return Unauthorized();

        return Ok(await historyService.ListAsync(userId, query, cancellationToken));
    }

    [HttpPost("{id:guid}/favorite")]
    [ProducesResponseType(typeof(HistoryItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<HistoryItemDto>> ToggleFavorite(
        Guid id,
        CancellationToken cancellationToken)
    {
        var userId = CurrentUserId.From(User);
        if (userId is null)
            return Unauthorized();

        var item = await historyService.ToggleFavoriteAsync(userId, id, cancellationToken);
        if (item is null)
            return NotFound();

        return Ok(item);
    }
}
