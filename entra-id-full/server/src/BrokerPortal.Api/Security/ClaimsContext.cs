using System.Security.Claims;

namespace BrokerPortal.Api.Security;

public sealed record ClaimsContext(
	string? BrokerId,
	string? AgencyId,
	string? FullName,
	string? Role
);