using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class SampleController : ControllerBase
{
    [HttpGet("public")]
    public IActionResult Public() => Ok(new { msg = "public endpoint" });

    [Authorize(Policy = "RequireAdminRole")]
    [HttpGet("role")]
    public IActionResult Role() => Ok(new { msg = "role access - Admin" });

    [Authorize(Policy = "RequireGroup")]
    [HttpGet("group")]
    public IActionResult Group() => Ok(new { msg = "group access - member of configured group" });

    [Authorize(Policy = "RequireCustomAttribute")]
    [HttpGet("custom")]
    public IActionResult Custom() => Ok(new { msg = "custom attribute matched" });
}
