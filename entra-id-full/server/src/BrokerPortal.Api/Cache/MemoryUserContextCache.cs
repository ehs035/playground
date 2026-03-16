using System.Collections.Concurrent;
using BrokerPortal.Api.Models;
using Microsoft.Extensions.Caching.Memory;

namespace BrokerPortal.Api.Cache;

public sealed class MemoryUserContextCache : IUserContextCache
{
    private readonly IMemoryCache _cache;
    private static readonly ConcurrentDictionary<string, SemaphoreSlim> Locks = new();

    public MemoryUserContextCache(IMemoryCache cache)
    {
        _cache = cache;
    }

    public async Task<UserContext?> GetOrCreateAsync(
        string oid,
        Func<Task<UserContext?>> factory,
        CancellationToken cancellationToken = default)
    {
        var cacheKey = $"userctx:{oid}";

        if (_cache.TryGetValue<UserContext>(cacheKey, out var existing))
            return existing;

        var gate = Locks.GetOrAdd(cacheKey, _ => new SemaphoreSlim(1, 1));
        await gate.WaitAsync(cancellationToken);

        try
        {
            if (_cache.TryGetValue<UserContext>(cacheKey, out existing))
                return existing;

            var created = await factory();
            if (created is null)
                return null;

            _cache.Set(cacheKey, created, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10),
                SlidingExpiration = TimeSpan.FromMinutes(3)
            });

            return created;
        }
        finally
        {
            gate.Release();
        }
    }
}
