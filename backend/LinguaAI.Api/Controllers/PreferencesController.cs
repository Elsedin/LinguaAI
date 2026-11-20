using LinguaAI.Api.Auth;
using LinguaAI.Application.Preferences;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LinguaAI.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/preferences")]
public class PreferencesController(IPreferenceService preferenceService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(PreferenceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PreferenceDto>> Get(CancellationToken cancellationToken)
    {
        var userId = CurrentUserId.From(User);
        if (userId is null)
            return Unauthorized();

        return Ok(await preferenceService.GetAsync(userId, cancellationToken));
    }

    [HttpPut]
    [ProducesResponseType(typeof(PreferenceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PreferenceDto>> Upsert(
        [FromBody] UpdatePreferenceRequest request,
        CancellationToken cancellationToken)
    {
        var userId = CurrentUserId.From(User);
        if (userId is null)
            return Unauthorized();

        return Ok(await preferenceService.UpsertAsync(userId, request, cancellationToken));
    }
}
