using System.Security.Claims;
using BrokerPortal.Api.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing; // for GetRouteData()

namespace BrokerPortal.Api.Authorization;

public sealed class BrokerOfAgencyRequirement : IAuthorizationRequirement { }

public sealed class BrokerOfAgencyHandler : AuthorizationHandler<BrokerOfAgencyRequirement>
{
	private readonly IUserClaimsRepository _repo;

	public BrokerOfAgencyHandler(IUserClaimsRepository repo) => _repo = repo;

	protected override async Task HandleRequirementAsync(
		AuthorizationHandlerContext context,
		BrokerOfAgencyRequirement requirement)
	{
		// Normalize to HttpContext (supports endpoint routing & old MVC)
		HttpContext? httpContext = context.Resource switch
		{
			AuthorizationFilterContext filterCtx => filterCtx.HttpContext,
			HttpContext ctx => ctx,
			_ => null
		};

		if (httpContext is null)
		{
			context.Fail();
			return;
		}

		var ct = httpContext.RequestAborted;
		var routeValues = httpContext.GetRouteData()?.Values;

		// --- Resolve IDs (priority: route -> query -> headers -> claims) as STRINGS ---
		var requestedAgencyIdStr =
			routeValues?["agencyId"]?.ToString()
			?? httpContext.Request.Query["agencyId"].FirstOrDefault()
			?? GetHeader(httpContext, "X-Agency-Id")                 // preferred custom header
			?? GetHeader(httpContext, "Agency-Id");                  // optional alt name

		var requestedBrokerIdStr =
			routeValues?["brokerId"]?.ToString()
			?? httpContext.Request.Query["brokerId"].FirstOrDefault()
			?? GetHeader(httpContext, "X-Broker-Id")
			?? GetHeader(httpContext, "Broker-Id");

		var tokenAgencyIdStr = context.User.FindFirstValue("agency_id");
		var tokenBrokerIdStr = context.User.FindFirstValue("broker_id");

		// Fallbacks: if not passed, use token values
		var effectiveAgencyIdStr = requestedAgencyIdStr ?? tokenAgencyIdStr;
		var effectiveBrokerIdStr = requestedBrokerIdStr ?? tokenBrokerIdStr;

		// Must have both after fallback; and token must have agency for consistency checks
		if (string.IsNullOrWhiteSpace(effectiveAgencyIdStr) ||
			string.IsNullOrWhiteSpace(effectiveBrokerIdStr) ||
			string.IsNullOrWhiteSpace(tokenAgencyIdStr))
		{
			context.Fail();
			return; // → 403
		}

		// Strict agency consistency: if request includes agencyId and it differs from token agency, forbid
		if (!string.IsNullOrWhiteSpace(requestedAgencyIdStr) &&
			!StringEquals(requestedAgencyIdStr, tokenAgencyIdStr))
		{
			context.Fail();
			return; // → 403
		}

		// Repository check: ensure broker belongs to agency
		var belongs = await _repo.BrokerBelongsToAgencyAsync(effectiveBrokerIdStr, effectiveAgencyIdStr, ct);
		if (!belongs)
		{
			context.Fail();
			return; // → 403
		}

		// Caller attributes (from token claims)
		var isAgencyAdmin = EqualsTrue(context.User.FindFirstValue("is_agency_admin"));
		var sameBroker = !string.IsNullOrWhiteSpace(tokenBrokerIdStr) &&
						 StringEquals(tokenBrokerIdStr, effectiveBrokerIdStr);

		if (isAgencyAdmin || sameBroker)
		{
			context.Succeed(requirement); // → 200
		}
		else
		{
			context.Fail();
		}
	}

	// Case-insensitive equality for string IDs
	private static bool StringEquals(string? a, string? b) =>
		string.Equals(a?.Trim(), b?.Trim(), StringComparison.OrdinalIgnoreCase);

	private static bool EqualsTrue(string? s) =>
		string.Equals(s, "true", StringComparison.OrdinalIgnoreCase);

	// Read a single header value (first value if multiple). Header names are case-insensitive.
	private static string? GetHeader(HttpContext httpContext, string name)
	{
		if (httpContext.Request.Headers.TryGetValue(name, out var values))
		{
			// Return the first non-empty value
			foreach (var v in values)
			{
				var trimmed = v?.Trim();
				if (!string.IsNullOrEmpty(trimmed))
					return trimmed;
			}
		}
		return null;
	}
}