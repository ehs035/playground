using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BrokerPortal.Api.Controllers;

[ApiController]
[Route("api/agencies")]
public sealed class AgenciesController : ControllerBase
{
    [HttpGet("{agencyId:guid}")]
    [Authorize(Policy = "AgencyAccess")]
    public IActionResult GetAgency(Guid agencyId)
    {
        return Ok(new
        {
            agencyId,
            message = "Authorized to access agency resource."
        });
    }
}
