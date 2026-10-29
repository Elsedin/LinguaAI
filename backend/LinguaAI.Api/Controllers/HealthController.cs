using Microsoft.AspNetCore.Mvc;

namespace LinguaAI.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new
        {
            status = "ok",
            service = "LinguaAI.Api",
            utc = DateTime.UtcNow
        });
    }
}
