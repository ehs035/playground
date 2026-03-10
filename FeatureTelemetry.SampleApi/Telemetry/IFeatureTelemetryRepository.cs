namespace FeatureTelemetry.SampleApi.Telemetry;

public interface IFeatureTelemetryRepository
{
    ValueTask EnqueueAsync(FeatureTelemetryEvent evt, CancellationToken ct = default);
}
