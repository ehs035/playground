using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BrokerPortal.Api.Controllers;

public sealed record RevenueCurrentQuery(string? agencyId, string? brokerId);

[ApiController]
[Route("api/revenue")]
public sealed class RevenueController : ControllerBase
{
    /// <summary>
    /// GET /Revenue/Current?agencyId=<string>&brokerId=<string>
    /// If either param is omitted, policy falls back to token claims to validate.
    /// </summary>
    [HttpGet("Current")]
    [Authorize(Policy = "BrokerOfAgency")]
    public IActionResult GetCurrent([FromQuery] RevenueCurrentQuery query)
    {
        // If policy passed, IDs are consistent and authorized.
        // You can echo resolved values or compute revenue with your service.

        return Ok(new
        {
            message = "Authorized: agency/broker validated against token and membership.",
            // Optionally include what the client sent; final validation is in the policy
            requestedAgencyId = query.agencyId,
            requestedBrokerId = query.brokerId
        });
    }
}
