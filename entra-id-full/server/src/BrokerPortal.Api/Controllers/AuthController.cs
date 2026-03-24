using System.Reflection.Metadata;
using BrokerPortal.Api.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BrokerPortal.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    [HttpGet("me")]
    [Authorize]
    public IActionResult Me()
    {
		//var claims = User.Claims.Select(c => new { c.Type, c.Value });

		//return Ok(new
		//{
		//    message = "Token validated by Microsoft Entra ID and claims enriched from SQL/cache.",
		//    claims
		//});
		var UserContext = User.GetClaimsContext(); // default claim names
		return Ok(new
		{
			UserContext

		});

	}
}

