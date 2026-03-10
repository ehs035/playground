using System.Diagnostics;

namespace FeatureTelemetry.SampleApi.Telemetry;

public sealed class TelemetryContextAccessor : ITelemetryContextAccessor
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TelemetryContextAccessor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? GetCorrelationId()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext is null) return null;

        if (httpContext.Items.TryGetValue("CorrelationId", out var itemValue))
            return itemValue?.ToString();

        return httpContext.TraceIdentifier;
    }

    public string? GetTraceId() => Activity.Current?.TraceId.ToString();
}
