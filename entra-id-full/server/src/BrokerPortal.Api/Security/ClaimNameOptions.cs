namespace BrokerPortal.Api.Security;

public sealed class ClaimNameOptions
{
	/// <summary>Claim name for broker identifier (e.g., "broker_id").</summary>
	public string BrokerId { get; init; } = "brokerId";

	/// <summary>Claim name for agency identifier (e.g., "agency_id").</summary>
	public string AgencyId { get; init; } = "agencyId";

	public string Role { get; init; } = "role";

	/// <summary>Claim name indicating admin flag (e.g., "is_agency_admin").</summary>
	public string IsAdminFlag { get; init; } = "is_agency_admin";

	/// <summary>Claim name for full display name (e.g., "name").</summary>
	public string DisplayName { get; init; } = "name";

	/// <summary>Claim name for given/first name (fallback if DisplayName missing).</summary>
	public string FirstName { get; init; } = "first_name";

	/// <summary>Claim name for family/last name (fallback if DisplayName missing).</summary>
	public string LastName { get; init; } = "last_name";

}