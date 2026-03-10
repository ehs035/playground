namespace FeatureTelemetry.SampleApi.Telemetry;
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public sealed class FeatureTelemetryAttribute : Attribute
{
    public string ModuleName { get; }
    public string FeatureName { get; }
    public FeatureTelemetryAttribute(string moduleName, string featureName) { ModuleName = moduleName; FeatureName = featureName; }
}