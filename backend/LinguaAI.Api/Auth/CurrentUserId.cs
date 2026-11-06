using System.Security.Claims;

namespace LinguaAI.Api.Auth;

public static class CurrentUserId
{
    public static string? From(ClaimsPrincipal user)
    {
        if (user.Identity?.IsAuthenticated != true)
            return null;

        return user.FindFirstValue(ClaimTypes.NameIdentifier);
    }
}
