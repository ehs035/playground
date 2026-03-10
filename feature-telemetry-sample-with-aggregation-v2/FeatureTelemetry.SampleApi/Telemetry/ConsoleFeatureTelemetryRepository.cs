using Serilog; using ILogger = Serilog.ILogger;
namespace FeatureTelemetry.SampleApi.Telemetry;
public sealed class ConsoleFeatureTelemetryRepository : IFeatureTelemetryRepository
{
    private readonly ILogger _logger = Log.ForContext<ConsoleFeatureTelemetryRepository>();
    private readonly IFeatureTelemetrySummaryStore _summaryStore;
    public ConsoleFeatureTelemetryRepository(IFeatureTelemetrySummaryStore summaryStore) => _summaryStore = summaryStore;
    public ValueTask EnqueueAsync(FeatureTelemetryEvent evt, CancellationToken ct = default)
    {
        _logger.ForContext("app_name", evt.AppName).ForContext("module_name", evt.ModuleName).ForContext("feature_name", evt.FeatureName).ForContext("event_name", evt.EventName).ForContext("outcome", evt.Outcome).ForContext("correlation_id", evt.CorrelationId).ForContext("trace_id", evt.TraceId).ForContext("entity_id", evt.EntityId).ForContext("duration_ms", evt.DurationMs).ForContext("error_code", evt.ErrorCode).ForContext("error_message", evt.ErrorMessage).Information("Telemetry event {@TelemetryEvent}", evt);
        _summaryStore.Record(evt);
        return ValueTask.CompletedTask;
    }
}