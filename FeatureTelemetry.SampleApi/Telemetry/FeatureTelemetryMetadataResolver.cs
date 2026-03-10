namespace FeatureTelemetry.SampleApi.Telemetry;

public sealed class FeatureTelemetryMetadataResolver : IFeatureTelemetryMetadataResolver
{
    public (string ModuleName, string FeatureName) Resolve(HttpContext httpContext)
    {
        var endpoint = httpContext.GetEndpoint();
        var attr = endpoint?.Metadata.GetMetadata<FeatureTelemetryAttribute>();

        if (attr is null)
            throw new InvalidOperationException("FeatureTelemetryAttribute is missing on the endpoint/action.");

        return (attr.ModuleName, attr.FeatureName);
    }
}
