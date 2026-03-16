namespace BrokerPortal.Api.Models;

public sealed class UserContext
{
    public string Oid { get; set; } = default!;
    public Guid? AgencyId { get; set; }
    public string? AgencyName { get; set; }
    public Guid? BrokerId { get; set; }
    public string? BrokerName { get; set; }
    public bool IsAgencyAdmin { get; set; }
    public bool IsGoodStanding { get; set; }
    public DateTime? ContractStart { get; set; }
    public DateTime? ContractEnd { get; set; }
}
