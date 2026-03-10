namespace FeatureTelemetry.SampleApi.Telemetry;

public sealed class FeatureTelemetryMetadataResolver : IFeatureTelemetryMetadataResolver
{
    public (string ModuleName, string FeatureName) Resolve(HttpContext httpContext)
    {
        // First try the values we explicitly put into HttpContext.Items
        if (httpContext.Items.TryGetValue("Telemetry.ModuleName", out var moduleObj) &&
            httpContext.Items.TryGetValue("Telemetry.FeatureName", out var featureObj) &&
            moduleObj is string moduleName &&
            featureObj is string featureName)
        {
            return (moduleName, featureName);
        }

        // Fallback to endpoint metadata (useful for MVC/controllers)
        var endpoint = httpContext.GetEndpoint();
        var attr = endpoint?.Metadata.GetMetadata<FeatureTelemetryAttribute>();

        if (attr is not null)
            return (attr.ModuleName, attr.FeatureName);

        throw new InvalidOperationException(
            "FeatureTelemetry metadata is missing. Ensure the endpoint/controller has FeatureTelemetryAttribute.");
    }
}