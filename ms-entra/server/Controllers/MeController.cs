using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
public class MeController : ControllerBase
{
    [Authorize]
    [HttpGet]
    public IActionResult Get()
    {
        var user = HttpContext.User;
        var name = user.Identity?.Name
            ?? user.FindFirst("preferred_username")?.Value
            ?? user.FindFirst(ClaimTypes.Name)?.Value;

        var roles = user.Claims
            .Where(c => c.Type == ClaimTypes.Role || c.Type == "roles" || c.Type == "role")
            .Select(c => c.Value)
            .Distinct();

        var groups = user.Claims
            .Where(c => c.Type == "groups")
            .Select(c => c.Value)
            .Distinct();

        var custom = user.Claims
            .FirstOrDefault(c => c.Type == "extension_CustomAttribute" || c.Type.Contains("extension_"))
            ?.Value;

        var claims = user.Claims.Select(c => new { c.Type, c.Value });

        return Ok(new { name, roles, groups, customAttribute = custom, claims });
    }
}
