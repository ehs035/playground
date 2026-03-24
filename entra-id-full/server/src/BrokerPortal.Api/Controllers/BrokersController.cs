using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BrokerPortal.Api.Controllers;

[ApiController]
[Route("api/agencies/{agencyId}/brokers")]
public sealed class BrokersController : ControllerBase
{
    [HttpGet("{brokerId}")]
    [Authorize(Policy = "BrokerOfAgency")]
    public IActionResult GetBroker(Guid agencyId, Guid brokerId)
    {
        return Ok(new
        {
            agencyId,
            brokerId,
            message = "Authorized to access broker resource."
        });
    }
}
