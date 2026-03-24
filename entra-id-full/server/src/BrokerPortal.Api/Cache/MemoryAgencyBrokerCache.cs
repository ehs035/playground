using System;
using System.Collections.Generic;
using Microsoft.Extensions.Caching.Memory;

namespace BrokerPortal.Api.Cache
{
	public interface IAgencyBrokerCache
	{
		bool TryGet(string agencyId, out IReadOnlySet<string> brokerIds);
		void Set(string agencyId, IReadOnlySet<string> brokerIds, TimeSpan ttl);
		void Invalidate(string agencyId);
	}

	public sealed class MemoryAgencyBrokerCache : IAgencyBrokerCache
	{
		private readonly IMemoryCache _cache;
		private static readonly TimeSpan DefaultTtl = TimeSpan.FromMinutes(10);

		public MemoryAgencyBrokerCache(IMemoryCache cache)
		{
			_cache = cache;
		}

		public bool TryGet(string agencyId, out IReadOnlySet<string> brokerIds)
		{
			if (_cache.TryGetValue(GetKey(agencyId), out HashSet<string>? set) && set is not null)
			{
				brokerIds = set;
				return true;
			}

			brokerIds = Array.Empty<string>().ToHashSet();
			return false;
		}

		// Matches the interface: non-nullable TimeSpan. We add a default value in the implementation for convenience.
		public void Set(string agencyId, IReadOnlySet<string> brokerIds, TimeSpan ttl = default)
		{
			var key = GetKey(agencyId);

			// Copy into a HashSet to guarantee O(1) membership and isolation
			var setCopy = brokerIds is HashSet<string> hs ? hs : brokerIds.ToHashSet();

			// If caller passed default(TimeSpan), use our DefaultTtl
			var effectiveTtl = ttl == default ? DefaultTtl : ttl;

			_cache.Set(key, setCopy, effectiveTtl);
		}

		public void Invalidate(string agencyId)
		{
			_cache.Remove(GetKey(agencyId));
		}

		private static string GetKey(string agencyId) => $"agency:{agencyId}:brokers";
	}
}