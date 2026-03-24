using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BrokerPortal.Api.Authorization;

public sealed class AgencyAccessHandler : AuthorizationHandler<AgencyAccessRequirement>
{
	protected override Task HandleRequirementAsync(
		AuthorizationHandlerContext context,
		AgencyAccessRequirement requirement)
	{
		if (context.Resource is not AuthorizationFilterContext mvcContext)
			return Task.CompletedTask;

		var http = mvcContext.HttpContext;

		// 1. Resolve agencyId from route, query, fallback to claim
		var requested = ResolveGuid(
			mvcContext.RouteData.Values["agencyId"]?.ToString(),
			http.Request.Query["agencyId"].FirstOrDefault(),
			null
		);

		var tokenAgency = ResolveGuid(
			null,
			null,
			context.User.FindFirstValue("agency_id")
		);

		var isAdmin = string.Equals(
			context.User.FindFirstValue("is_agency_admin"),
			"true",
			StringComparison.OrdinalIgnoreCase);

		// 2. No agency? Then cannot authorize
		if (requested is null && tokenAgency is null)
			return Task.CompletedTask;

		// 3. Effective agency = query/route OR token claim
		var effective = requested ?? tokenAgency;

		// 4. Enforce: Caller must be admin or match agency
		if (effective == tokenAgency || isAdmin)
		{
			context.Succeed(requirement);
		}

		return Task.CompletedTask;
	}

	private static Guid? ResolveGuid(string? route, string? query, string? claim)
	{
		if (Guid.TryParse(route, out var r)) return r;
		if (Guid.TryParse(query, out var q)) return q;
		if (Guid.TryParse(claim, out var c)) return c;
		return null;
	}
}