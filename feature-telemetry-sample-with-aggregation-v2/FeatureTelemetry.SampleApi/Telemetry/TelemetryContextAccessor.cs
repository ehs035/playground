using System.Diagnostics;
namespace FeatureTelemetry.SampleApi.Telemetry;
public sealed class TelemetryContextAccessor : ITelemetryContextAccessor
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    public TelemetryContextAccessor(IHttpContextAccessor httpContextAccessor) => _httpContextAccessor = httpContextAccessor;
    public string? GetCorrelationId()
    {
        var ctx = _httpContextAccessor.HttpContext;
        if (ctx is null) return null;
        if (ctx.Items.TryGetValue("CorrelationId", out var item)) return item?.ToString();
        return ctx.TraceIdentifier;
    }
    public string? GetTraceId() => Activity.Current?.TraceId.ToString();
}