using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BrokerPortal.Api.Authorization;

public sealed class AgencyAccessHandler : AuthorizationHandler<AgencyAccessRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AgencyAccessRequirement requirement)
    {
        if (context.Resource is not AuthorizationFilterContext mvcContext)
            return Task.CompletedTask;

        if (!mvcContext.RouteData.Values.TryGetValue("agencyId", out var agencyIdValue))
            return Task.CompletedTask;

        var routeAgencyId = agencyIdValue?.ToString();
        var callerAgencyId = context.User.FindFirstValue("agency_id");
        var isAgencyAdmin = string.Equals(
            context.User.FindFirstValue("is_agency_admin"),
            "true",
            StringComparison.OrdinalIgnoreCase);

        if (isAgencyAdmin || (!string.IsNullOrWhiteSpace(routeAgencyId) && routeAgencyId == callerAgencyId))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
