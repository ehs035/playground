namespace ProviderInbox.Service.Shared.Security;

public sealed class QueryStringAudienceContext : IAudienceContext
{
    private readonly IHttpContextAccessor _http;
    public QueryStringAudienceContext(IHttpContextAccessor http) => _http = http;

    public AudienceKey GetAudience()
    {
        var ctx = _http.HttpContext ?? throw new InvalidOperationException("Missing HttpContext");
        var npi = ctx.Request.Query["npi"].ToString();
        var office = ctx.Request.Query["officeNumber"].ToString();
        if (string.IsNullOrWhiteSpace(npi) || string.IsNullOrWhiteSpace(office))
            throw new InvalidOperationException("Missing npi/officeNumber query params (dev mode).");
        return new AudienceKey(npi, office);
    }
}
