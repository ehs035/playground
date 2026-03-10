namespace FeatureTelemetry.SampleApi.Telemetry;
public sealed class FeatureTelemetryAggregate
{
    public required string AppName { get; init; }
    public required string ModuleName { get; init; }
    public required string FeatureName { get; init; }
    public int SuccessCount { get; init; }
    public int FailureCount { get; init; }
    public int TimeoutCount { get; init; }
    public double AvgSuccessDurationMs { get; init; }
    public int? MinSuccessDurationMs { get; init; }
    public int? MaxSuccessDurationMs { get; init; }
    public DateTime CalculatedAtUtc { get; init; }
}