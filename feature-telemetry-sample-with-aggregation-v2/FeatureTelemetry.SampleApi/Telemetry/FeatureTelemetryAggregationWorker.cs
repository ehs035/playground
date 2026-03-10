using Serilog;
using ILogger = Serilog.ILogger;
namespace FeatureTelemetry.SampleApi.Telemetry;
public sealed class FeatureTelemetryAggregationWorker : BackgroundService
{
    private readonly IFeatureTelemetrySummaryStore _summaryStore;
    private readonly ILogger _logger = Log.ForContext<FeatureTelemetryAggregationWorker>();
    public FeatureTelemetryAggregationWorker(IFeatureTelemetrySummaryStore summaryStore) => _summaryStore = summaryStore;
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            Aggregate();
            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        }
    }
    private void Aggregate()
    {
        var events = _summaryStore.GetAllEvents();
        var aggregates = events.GroupBy(x => new { x.AppName, x.ModuleName, x.FeatureName })
            .Select(g => {
                var success = g.Where(x => x.Outcome == "success" && x.DurationMs.HasValue).ToList();
                return new FeatureTelemetryAggregate {
                    AppName = g.Key.AppName, ModuleName = g.Key.ModuleName, FeatureName = g.Key.FeatureName,
                    SuccessCount = g.Count(x => x.Outcome == "success"),
                    FailureCount = g.Count(x => x.Outcome == "failure"),
                    TimeoutCount = g.Count(x => x.Outcome == "timeout"),
                    AvgSuccessDurationMs = success.Count > 0 ? success.Average(x => x.DurationMs!.Value) : 0,
                    MinSuccessDurationMs = success.Count > 0 ? success.Min(x => x.DurationMs!.Value) : null,
                    MaxSuccessDurationMs = success.Count > 0 ? success.Max(x => x.DurationMs!.Value) : null,
                    CalculatedAtUtc = DateTime.UtcNow
                };
            }).OrderBy(x => x.AppName).ThenBy(x => x.ModuleName).ThenBy(x => x.FeatureName).ToArray();
        _summaryStore.ReplaceAggregates(aggregates);
        _logger.Information("Aggregation worker completed. AggregateCount={AggregateCount}", aggregates.Length);
    }
}