using System.Collections.Concurrent; using Serilog; using ILogger = Serilog.ILogger;
namespace FeatureTelemetry.SampleApi.Telemetry;
public sealed class InMemoryFeatureTelemetrySummaryStore : IFeatureTelemetrySummaryStore
{
    private readonly ConcurrentQueue<FeatureTelemetryEvent> _events = new();
    private readonly object _lock = new();
    private IReadOnlyCollection<FeatureTelemetryAggregate> _aggregates = Array.Empty<FeatureTelemetryAggregate>();
    private readonly ILogger _logger = Log.ForContext<InMemoryFeatureTelemetrySummaryStore>();
    public void Record(FeatureTelemetryEvent evt){ _events.Enqueue(evt); while(_events.Count>5000 && _events.TryDequeue(out _)){} }
    public IReadOnlyCollection<FeatureTelemetryEvent> GetAllEvents() => _events.ToArray();
    public IReadOnlyCollection<FeatureTelemetryAggregate> GetAggregates(){ lock(_lock){ return _aggregates.ToArray(); } }
    public void ReplaceAggregates(IReadOnlyCollection<FeatureTelemetryAggregate> aggregates){ lock(_lock){ _aggregates = aggregates.ToArray(); } foreach(var a in aggregates){ _logger.Information("Telemetry Aggregate => App={AppName}, Module={ModuleName}, Feature={FeatureName}, SuccessCount={SuccessCount}, FailureCount={FailureCount}, TimeoutCount={TimeoutCount}, AvgSuccessDurationMs={AvgSuccessDurationMs}", a.AppName, a.ModuleName, a.FeatureName, a.SuccessCount, a.FailureCount, a.TimeoutCount, Math.Round(a.AvgSuccessDurationMs,2)); } }
}