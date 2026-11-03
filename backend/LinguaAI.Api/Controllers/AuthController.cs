using LinguaAI.Application.Auth;
using LinguaAI.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LinguaAI.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager) : ControllerBase
{
    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthUserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AuthUserDto>> Register([FromBody] RegisterRequest request)
    {
        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email
        };

        var createResult = await userManager.CreateAsync(user, request.Password);
        if (!createResult.Succeeded)
        {
            var errors = string.Join(" ", createResult.Errors.Select(e => e.Description));
            return BadRequest(new { detail = errors });
        }

        await signInManager.SignInAsync(user, isPersistent: true);

        return Ok(new AuthUserDto
        {
            Id = user.Id,
            Email = user.Email!
        });
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthUserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthUserDto>> Login([FromBody] LoginRequest request)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user is null)
            return Unauthorized(new { detail = "Invalid email or password." });

        var signInResult = await signInManager.PasswordSignInAsync(
            user,
            request.Password,
            isPersistent: true,
            lockoutOnFailure: false);

        if (!signInResult.Succeeded)
            return Unauthorized(new { detail = "Invalid email or password." });

        return Ok(new AuthUserDto
        {
            Id = user.Id,
            Email = user.Email!
        });
    }
}