namespace FeatureTelemetry.SampleApi.Services;

public interface IProviderSearchService
{
    Task<List<string>> SearchAsync(string query, CancellationToken ct);
}
