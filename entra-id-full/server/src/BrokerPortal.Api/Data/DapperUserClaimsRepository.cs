using System.Data;
using BrokerPortal.Api.Models;
using Dapper;
using Microsoft.Data.SqlClient;

namespace BrokerPortal.Api.Data;

public sealed class DapperUserClaimsRepository : IUserClaimsRepository
{
    private readonly IConfiguration _configuration;

    public DapperUserClaimsRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    private IDbConnection CreateConnection()
        => new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

    public async Task<UserContext?> GetUserContextByOidAsync(string oid, CancellationToken cancellationToken = default)
    {
        const string sql = """
        SELECT
            u.Oid,
            u.AgencyId,
            a.Name AS AgencyName,
            u.BrokerId,
            b.Name AS BrokerName,
            u.IsAgencyAdmin,
            ISNULL(b.IsGoodStanding, 0) AS IsGoodStanding,
            b.ContractStart,
            b.ContractEnd
        FROM Users u
        LEFT JOIN Agencies a ON a.Id = u.AgencyId
        LEFT JOIN Brokers b ON b.Id = u.BrokerId
        WHERE u.Oid = @oid
        """;

        using var connection = CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<UserContext>(new CommandDefinition(
            sql,
            new { oid },
            cancellationToken: cancellationToken));
    }

    public async Task<bool> BrokerBelongsToAgencyAsync(Guid brokerId, Guid agencyId, CancellationToken cancellationToken = default)
    {
        const string sql = """
        SELECT COUNT(1)
        FROM Brokers
        WHERE Id = @brokerId AND AgencyId = @agencyId
        """;

        using var connection = CreateConnection();
        var count = await connection.ExecuteScalarAsync<int>(new CommandDefinition(
            sql,
            new { brokerId, agencyId },
            cancellationToken: cancellationToken));

        return count > 0;
    }
}
