namespace BrokerPortal.Api.Models;

public sealed class UserContext
{
    public string Oid { get; set; } = default!;
    public String? Email { get; set; }
    public String? AgencyId { get; set; }
	public string? AgencyName { get; set; }
	public String? BrokerId { get; set; }
	public String? BrokerName { get; set; }
	public String? FirstName { get; set; }
	public String? LastName { get; set; }

	public String? UserLevel { get; set; }

	public string? LicenseExpiration { get; set; }
    public string? LicenseNumber { get; set; }
    public bool IsAgencyAdmin { get; set; }
    public bool IsGoodStanding { get; set; }
    public DateTime? ContractStart { get; set; }
    public DateTime? ContractEnd { get; set; }
}

