using BrokerPortal.Api.Models;

namespace BrokerPortal.Api.Data;

public interface IUserClaimsRepository
{
    Task<UserContext?> GetUserContextByOidAsync(string oid, CancellationToken cancellationToken = default);
    Task<bool> BrokerBelongsToAgencyAsync(Guid brokerId, Guid agencyId, CancellationToken cancellationToken = default);
}
