namespace FeatureTelemetry.SampleApi.Telemetry;
public interface IFeatureTelemetrySummaryStore
{
    void Record(FeatureTelemetryEvent evt);
    IReadOnlyCollection<FeatureTelemetryEvent> GetAllEvents();
    IReadOnlyCollection<FeatureTelemetryAggregate> GetAggregates();
    void ReplaceAggregates(IReadOnlyCollection<FeatureTelemetryAggregate> aggregates);
}