using BrokerPortal.Api.Models;

namespace BrokerPortal.Api.Data;

public interface IUserClaimsRepository
{
    Task<UserContext?> GetUserContextByOidAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> BrokerBelongsToAgencyAsync(string brokerId, string agencyId, CancellationToken cancellationToken = default);
}
