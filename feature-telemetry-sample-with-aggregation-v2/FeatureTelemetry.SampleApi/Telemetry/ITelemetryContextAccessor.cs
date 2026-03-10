namespace FeatureTelemetry.SampleApi.Telemetry;
public interface ITelemetryContextAccessor { string? GetCorrelationId(); string? GetTraceId(); }