namespace BrokerPortal.Api.Models;

public sealed class UserDirectoryMapping
{
    public string Oid { get; set; } = default!;
    public Guid? AgencyId { get; set; }
    public Guid? BrokerId { get; set; }
    public bool IsAgencyAdmin { get; set; }
}
