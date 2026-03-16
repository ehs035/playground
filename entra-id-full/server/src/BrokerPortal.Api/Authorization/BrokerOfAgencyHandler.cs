using System.Security.Claims;
using BrokerPortal.Api.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BrokerPortal.Api.Authorization;

public sealed class BrokerOfAgencyHandler : AuthorizationHandler<BrokerOfAgencyRequirement>
{
    private readonly IServiceScopeFactory _scopeFactory;

    public BrokerOfAgencyHandler(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, BrokerOfAgencyRequirement requirement)
    {
        if (context.Resource is not AuthorizationFilterContext mvcContext)
            return;

        var agencyIdText = mvcContext.RouteData.Values["agencyId"]?.ToString();
        var brokerIdText = mvcContext.RouteData.Values["brokerId"]?.ToString();

        if (!Guid.TryParse(agencyIdText, out var agencyId) || !Guid.TryParse(brokerIdText, out var brokerId))
            return;

        using var scope = _scopeFactory.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IUserClaimsRepository>();

        var belongsToAgency = await repo.BrokerBelongsToAgencyAsync(brokerId, agencyId, CancellationToken.None);
        if (!belongsToAgency)
            return;

        var isAgencyAdmin = string.Equals(
            context.User.FindFirstValue("is_agency_admin"),
            "true",
            StringComparison.OrdinalIgnoreCase);

        var callerBrokerId = context.User.FindFirstValue("broker_id");
        var sameBroker = string.Equals(callerBrokerId, brokerId.ToString(), StringComparison.OrdinalIgnoreCase);

        var isGoodStanding = string.Equals(
            context.User.FindFirstValue("is_good_standing"),
            "true",
            StringComparison.OrdinalIgnoreCase);

        var hasStart = DateTime.TryParse(context.User.FindFirstValue("contract_start"), out var contractStart);
        var hasEnd = DateTime.TryParse(context.User.FindFirstValue("contract_end"), out var contractEnd);
        var now = DateTime.UtcNow;

        var withinContract = (!hasStart || contractStart <= now) && (!hasEnd || contractEnd >= now);

        if ((isAgencyAdmin || sameBroker) && isGoodStanding && withinContract)
        {
            context.Succeed(requirement);
        }
    }
}
