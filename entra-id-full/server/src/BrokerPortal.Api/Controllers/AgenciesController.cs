using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BrokerPortal.Api.Controllers;

[ApiController]
[Route("api/agencies")]
public sealed class AgenciesController : ControllerBase
{

    [HttpGet("current")]
    [Authorize(Policy = "AgencyAccess")]
    public IActionResult Current()
    {
        // agencyId resolved from token claims automatically
        var agencyId = User.FindFirstValue("agency_id");
        return Ok(new { agencyId, message = "Current agency profile." });
    }

}
