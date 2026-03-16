using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BrokerPortal.Api.Controllers;

[ApiController]
[Route("api/agencies/{agencyId:guid}/brokers")]
public sealed class BrokersController : ControllerBase
{
    [HttpGet("{brokerId:guid}")]
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
