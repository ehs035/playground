namespace FeatureTelemetry.SampleApi.Services;

public sealed class ProviderSearchService : IProviderSearchService
{
    public async Task<List<string>> SearchAsync(string query, CancellationToken ct)
    {
        await Task.Delay(150, ct);

        if (query.Equals("timeout", StringComparison.OrdinalIgnoreCase))
            throw new TimeoutException("Provider search timed out.");

        if (query.Equals("error", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Unexpected provider search error.");

        return new List<string>
        {
            $"Dr. Smith ({query})",
            $"Dr. Johnson ({query})"
        };
    }
}
