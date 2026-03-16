using System.Security.Claims;

namespace BrokerPortal.Api.Services;

public sealed class ClaimEnricher
{
    private readonly IUserContextService _userContextService;

    public ClaimEnricher(IUserContextService userContextService)
    {
        _userContextService = userContextService;
    }

    public async Task EnrichAsync(ClaimsPrincipal principal, HttpContext httpContext, CancellationToken cancellationToken = default)
    {
        if (principal.Identity is not ClaimsIdentity identity || !identity.IsAuthenticated)
            return;

        var oid = principal.FindFirstValue("oid");
        if (string.IsNullOrWhiteSpace(oid))
            return;

        var userContext = await _userContextService.GetCurrentAsync(oid, httpContext, cancellationToken);
        if (userContext is null)
            return;

        AddIfMissing(identity, "agency_id", userContext.AgencyId?.ToString());
        AddIfMissing(identity, "agency_name", userContext.AgencyName);
        AddIfMissing(identity, "broker_id", userContext.BrokerId?.ToString());
        AddIfMissing(identity, "broker_name", userContext.BrokerName);
        AddIfMissing(identity, "is_agency_admin", userContext.IsAgencyAdmin.ToString().ToLowerInvariant());
        AddIfMissing(identity, "is_good_standing", userContext.IsGoodStanding.ToString().ToLowerInvariant());
        AddIfMissing(identity, "contract_start", userContext.ContractStart?.ToString("O"));
        AddIfMissing(identity, "contract_end", userContext.ContractEnd?.ToString("O"));

        if (userContext.IsAgencyAdmin)
            AddIfMissing(identity, ClaimTypes.Role, "AgencyAdmin");
    }

    private static void AddIfMissing(ClaimsIdentity identity, string type, string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return;

        if (!identity.HasClaim(c => c.Type == type))
            identity.AddClaim(new Claim(type, value));
    }
}
