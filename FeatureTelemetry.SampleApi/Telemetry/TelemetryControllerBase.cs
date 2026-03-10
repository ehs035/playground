using Microsoft.AspNetCore.Mvc;

namespace FeatureTelemetry.SampleApi.Telemetry;

[ApiController]
public abstract class TelemetryControllerBase : ControllerBase
{
    protected IFeatureTelemetry Telemetry { get; }

    protected TelemetryControllerBase(IFeatureTelemetry telemetry)
    {
        Telemetry = telemetry;
    }

    protected Task<TResponse> ExecuteFeatureAsync<TResponse>(
        Func<CancellationToken, Task<TResponse>> action,
        string? entityId = null,
        Dictionary<string, string>? tags = null,
        CancellationToken ct = default)
    {
        return Telemetry.ExecuteAsync(HttpContext, action, entityId, tags, ct);
    }

    protected Task ExecuteFeatureAsync(
        Func<CancellationToken, Task> action,
        string? entityId = null,
        Dictionary<string, string>? tags = null,
        CancellationToken ct = default)
    {
        return Telemetry.ExecuteAsync(HttpContext, action, entityId, tags, ct);
    }
}
