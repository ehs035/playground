using BrokerPortal.Api.Models;

namespace BrokerPortal.Api.Cache;

public interface IUserContextCache
{
    Task<UserContext?> GetOrCreateAsync(
        string oid,
        Func<Task<UserContext?>> factory,
        CancellationToken cancellationToken = default);
}
