namespace BrokerPortal.Api.Authorization;

/// <summary>
/// Resource passed into authorization to validate broker access within an agency.
/// </summary>
public sealed record BrokerAgencyResource(string AgencyId, string BrokerId);