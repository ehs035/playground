namespace FeatureTelemetry.SampleApi.Telemetry;

public sealed class FeatureTelemetryEvent
{
    public required string AppName { get; init; }
    public required string ModuleName { get; init; }
    public required string FeatureName { get; init; }
    public required string EventName { get; init; }
    public required string Outcome { get; init; }
    public required DateTime OccurredAtUtc { get; init; }
    public string? CorrelationId { get; init; }
    public string? TraceId { get; init; }
    public string? EntityId { get; init; }
    public int? DurationMs { get; init; }
    public string? ErrorCode { get; init; }
    public string? ErrorMessage { get; init; }
    public Dictionary<string, string>? Tags { get; init; }
}
