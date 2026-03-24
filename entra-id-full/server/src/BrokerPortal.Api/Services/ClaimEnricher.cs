using System.Security.Claims;
using BrokerPortal.Api.Models;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;

namespace BrokerPortal.Api.Services;

public sealed class ClaimEnricher
{
	private readonly IUserContextService _userContextService;
	private readonly string? _appIdNoHyphens;


	public ClaimEnricher(IUserContextService userContextService, IOptionsMonitor<MicrosoftIdentityOptions> identityOptions)
	{
		_userContextService = userContextService;
		var clientId = identityOptions.CurrentValue.ClientId;
		_appIdNoHyphens = NormalizeAppId(clientId);
	}

	public async Task EnrichAsync(ClaimsPrincipal principal, HttpContext httpContext, CancellationToken cancellationToken = default)
	{

		if (principal.Identity is not ClaimsIdentity identity || !identity.IsAuthenticated)
			return;

		// Read "extn.*" (preferred) or fall back to "extension_<appId>_*"
		var agencyId = GetFirst(principal, "extn.agencyId")
						?? GetFirst(principal, $"extension_{_appIdNoHyphens}_agencyId");

		var brokerId = GetFirst(principal, "extn.brokerId")
						?? GetFirst(principal, $"extension_{_appIdNoHyphens}_brokerId");

		var userLevel = GetFirst(principal, "extn.userLevel")
						?? GetFirst(principal, $"extension_{_appIdNoHyphens}_userLevel");

		// Add normalized claims if present (avoid duplicates)
		AddIfMissing(identity, "agencyId", agencyId);
		AddIfMissing(identity, "brokerId", brokerId);
		AddIfMissing(identity, "role", userLevel);

	}

	private static void AddIfMissing(ClaimsIdentity identity, string type, string? value)
	{
		if (string.IsNullOrWhiteSpace(value))
			return;

		if (!identity.HasClaim(c => c.Type == type))
			identity.AddClaim(new Claim(type, value));
	}


	private static string? GetFirst(ClaimsPrincipal principal, string claimType)
		=> principal.FindAll(claimType).Select(c => c.Value).FirstOrDefault();


	private static string? NormalizeAppId(string? appId)
	{
		if (string.IsNullOrWhiteSpace(appId))
			return null;

		// Remove hyphens and lower-case to match AAD extension claim naming conventions
		return appId.Replace("-", "").ToLowerInvariant();
	}


}
