using System.Data;
using BrokerPortal.Api.Cache;
using BrokerPortal.Api.Models;
using Dapper;
using Microsoft.Data.SqlClient;

namespace BrokerPortal.Api.Data;

public sealed class DapperUserClaimsRepository : IUserClaimsRepository
{
	private readonly IConfiguration _configuration;
	private readonly IAgencyBrokerCache _agencyBrokerCache;

	public DapperUserClaimsRepository(IConfiguration configuration, IAgencyBrokerCache agencyBrokerCache)
	{
		_configuration = configuration;
		_agencyBrokerCache = agencyBrokerCache;
	}

	private IDbConnection CreateConnection()
		=> new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

	public async Task<UserContext?> GetUserContextByOidAsync(
	string email,
	CancellationToken cancellationToken = default)
	{

		const string sql = """
		SELECT
			u.Email,
			u.Firstname,
			u.LastName,
			u.TaxID as AgencyId,
			u.NPN as BrokerId,
			u.FirstName,
			u.LastName,
			u.LicenseNumber,
			u.ContractDate as ContractStart,
			u.LicenseExpiration as ContractEnd
		FROM Agent.Demographic u
		WHERE u.Email = @email
		""";

		using var connection = CreateConnection();
		return await connection.QuerySingleOrDefaultAsync<UserContext>(
			new CommandDefinition(sql, new { email }, cancellationToken: cancellationToken));

	}


	/// <summary>
	/// Loads all broker IDs assigned to a given agency as a set for O(1) membership checks.
	/// Default implementation uses Brokers table: Brokers.AgencyId = @agencyId.
	/// </summary>
	public async Task<IReadOnlySet<string>> GetBrokerIdsForAgencyAsync(
		string agencyId,
		CancellationToken cancellationToken = default)
	{
		const string sql = """
            SELECT b.NPN
            FROM  Agent.Demographic AS b
            WHERE b.TaxID = @agencyId
            """;

		using var connection = CreateConnection();
		var brokerIds = await connection.QueryAsync<string>(new CommandDefinition(
			sql,
			new { agencyId },
			cancellationToken: cancellationToken));

		return brokerIds.ToHashSet();
	}


	/// <summary>
	/// Returns true if the broker belongs to the specified agency.
	/// Uses Strategy 1 cache: caches the full broker set per agency for 10 minutes.
	/// Uses Strategy 1 cache: caches the full broker set per agency for 10 minutes.
	/// </summary>
	public async Task<bool> BrokerBelongsToAgencyAsync(
		string brokerId,
		string agencyId,
		CancellationToken cancellationToken = default)
	{
		// 1) Try cache
		if (_agencyBrokerCache.TryGet(agencyId, out var cachedBrokerIds))
			return cachedBrokerIds.Contains(brokerId);

		// 2) Load from DB and set cache (cache even if empty to avoid repeated DB hits)
		var brokerIds = await GetBrokerIdsForAgencyAsync(agencyId, cancellationToken);
		_agencyBrokerCache.Set(agencyId, brokerIds, TimeSpan.FromMinutes(10));

		return brokerIds.Contains(brokerId);
	}


}
