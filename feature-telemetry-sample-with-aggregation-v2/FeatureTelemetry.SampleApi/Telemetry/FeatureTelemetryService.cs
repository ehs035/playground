using System.Diagnostics; using Serilog; using ILogger = Serilog.ILogger;
namespace FeatureTelemetry.SampleApi.Telemetry;
public sealed class FeatureTelemetryService : IFeatureTelemetry
{
    private readonly string _appName;
    private readonly ITelemetryContextAccessor _contextAccessor;
    private readonly IFeatureTelemetryMetadataResolver _metadataResolver;
    private readonly IFeatureTelemetryRepository _repository;
    private readonly ILogger _logger;
    public FeatureTelemetryService(IConfiguration configuration, ITelemetryContextAccessor contextAccessor, IFeatureTelemetryMetadataResolver metadataResolver, IFeatureTelemetryRepository repository)
    { _appName = configuration["Telemetry:AppName"] ?? throw new InvalidOperationException("Telemetry:AppName is not configured."); _contextAccessor = contextAccessor; _metadataResolver = metadataResolver; _repository = repository; _logger = Log.ForContext<FeatureTelemetryService>(); }
    public async Task<T> ExecuteAsync<T>(HttpContext httpContext, Func<CancellationToken, Task<T>> action, string? entityId = null, Dictionary<string, string>? tags = null, CancellationToken ct = default)
    {
        var (moduleName, featureName) = _metadataResolver.Resolve(httpContext);
        var eventName = featureName.ToLowerInvariant().Replace(" ", "_");
        var sw = Stopwatch.StartNew();
        await TrackAsync(new FeatureTelemetryEvent{ AppName=_appName, ModuleName=moduleName, FeatureName=featureName, EventName=eventName, Outcome="usage", OccurredAtUtc=DateTime.UtcNow, CorrelationId=_contextAccessor.GetCorrelationId(), TraceId=_contextAccessor.GetTraceId(), EntityId=entityId, Tags=tags }, ct);
        try {
            var result = await action(ct); sw.Stop();
            await TrackAsync(new FeatureTelemetryEvent{ AppName=_appName, ModuleName=moduleName, FeatureName=featureName, EventName=eventName, Outcome="success", OccurredAtUtc=DateTime.UtcNow, CorrelationId=_contextAccessor.GetCorrelationId(), TraceId=_contextAccessor.GetTraceId(), EntityId=entityId, DurationMs=(int)sw.ElapsedMilliseconds, Tags=tags }, ct);
            return result;
        } catch (TimeoutException ex) {
            sw.Stop();
            await TrackAsync(new FeatureTelemetryEvent{ AppName=_appName, ModuleName=moduleName, FeatureName=featureName, EventName=eventName, Outcome="timeout", OccurredAtUtc=DateTime.UtcNow, CorrelationId=_contextAccessor.GetCorrelationId(), TraceId=_contextAccessor.GetTraceId(), EntityId=entityId, DurationMs=(int)sw.ElapsedMilliseconds, ErrorCode="TIMEOUT", ErrorMessage=ex.Message, Tags=tags }, ct);
            throw;
        } catch (Exception ex) {
            sw.Stop();
            await TrackAsync(new FeatureTelemetryEvent{ AppName=_appName, ModuleName=moduleName, FeatureName=featureName, EventName=eventName, Outcome="failure", OccurredAtUtc=DateTime.UtcNow, CorrelationId=_contextAccessor.GetCorrelationId(), TraceId=_contextAccessor.GetTraceId(), EntityId=entityId, DurationMs=(int)sw.ElapsedMilliseconds, ErrorCode="UNHANDLED_ERROR", ErrorMessage=ex.Message, Tags=tags }, ct);
            throw;
        }
    }
    public async Task ExecuteAsync(HttpContext httpContext, Func<CancellationToken, Task> action, string? entityId = null, Dictionary<string, string>? tags = null, CancellationToken ct = default)
        => await ExecuteAsync<object?>(httpContext, async token => { await action(token); return null; }, entityId, tags, ct);
    private async Task TrackAsync(FeatureTelemetryEvent evt, CancellationToken ct)
    {
        await _repository.EnqueueAsync(evt, ct);
        _logger.ForContext("app_name", evt.AppName).ForContext("module_name", evt.ModuleName).ForContext("feature_name", evt.FeatureName).ForContext("event_name", evt.EventName).ForContext("outcome", evt.Outcome).ForContext("correlation_id", evt.CorrelationId).ForContext("trace_id", evt.TraceId).Information("Feature telemetry recorded: {FeatureName} -> {Outcome}", evt.FeatureName, evt.Outcome);
    }
}