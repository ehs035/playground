using BrokerPortal.Api.Cache;
using BrokerPortal.Api.Data;
using BrokerPortal.Api.Models;

namespace BrokerPortal.Api.Services;


public interface IUserContextService
{
	Task<UserContext?> GetCurrentAsync(string oid, HttpContext httpContext, CancellationToken cancellationToken = default);
}

public sealed class UserContextService : IUserContextService
{
    private readonly IUserContextCache _cache;
    private readonly IUserClaimsRepository _repository;

    public UserContextService(IUserContextCache cache, IUserClaimsRepository repository)
    {
        _cache = cache;
        _repository = repository;
    }

    public async Task<UserContext?> GetCurrentAsync(string oid, HttpContext httpContext, CancellationToken cancellationToken = default)
    {
        const string itemKey = "__user_context";

        if (httpContext.Items.TryGetValue(itemKey, out var cachedPerRequest) && cachedPerRequest is UserContext existing)
            return existing;

        var result = await _cache.GetOrCreateAsync(
            oid,
            () => _repository.GetUserContextByOidAsync(oid, cancellationToken),
            cancellationToken);

        if (result is not null)
            httpContext.Items[itemKey] = result;

        return result;
    }
}
