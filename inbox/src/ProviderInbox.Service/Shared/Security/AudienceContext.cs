namespace ProviderInbox.Service.Shared.Security;
public sealed record AudienceKey(string Npi, string OfficeNumber);
public interface IAudienceContext { AudienceKey GetAudience(); }
