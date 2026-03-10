namespace FeatureTelemetry.SampleApi.Telemetry;
public interface IFeatureTelemetryMetadataResolver { (string ModuleName, string FeatureName) Resolve(HttpContext httpContext); }