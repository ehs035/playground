using BrokerPortal.Api.Models;

namespace BrokerPortal.Api.Services;

public interface IUserContextService
{
    Task<UserContext?> GetCurrentAsync(string oid, HttpContext httpContext, CancellationToken cancellationToken = default);
}
