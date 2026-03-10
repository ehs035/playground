namespace FeatureTelemetry.SampleApi.Telemetry;
public interface IFeatureTelemetry
{
    Task<T> ExecuteAsync<T>(HttpContext httpContext, Func<CancellationToken, Task<T>> action, string? entityId = null, Dictionary<string, string>? tags = null, CancellationToken ct = default);
    Task ExecuteAsync(HttpContext httpContext, Func<CancellationToken, Task> action, string? entityId = null, Dictionary<string, string>? tags = null, CancellationToken ct = default);
}