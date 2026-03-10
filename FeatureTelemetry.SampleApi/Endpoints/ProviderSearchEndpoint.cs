using FeatureTelemetry.SampleApi.Models;
using FeatureTelemetry.SampleApi.Services;
using FeatureTelemetry.SampleApi.Telemetry;

namespace FeatureTelemetry.SampleApi.Endpoints;

[FeatureTelemetry("Search", "Provider Search FE")]
public sealed class ProviderSearchEndpoint : TelemetryEndpoint<ProviderSearchRequest, ProviderSearchResponse>
{
    private IProviderSearchService _providerSearchService = default!;

    protected override void ConfigureEndpoint()
    {
        Post("/providers/search-fe");
        AllowAnonymous();
    }

    public override Task OnBeforeHandleAsync(ProviderSearchRequest req, CancellationToken ct)
    {
        _providerSearchService = Resolve<IProviderSearchService>();
        return Task.CompletedTask;
    }

    protected override async Task<ProviderSearchResponse> ExecuteFeatureAsync(ProviderSearchRequest req, CancellationToken ct)
    {
        var providers = await _providerSearchService.SearchAsync(req.Query, ct);
        return new ProviderSearchResponse { Providers = providers };
    }

    protected override string? GetEntityId(ProviderSearchRequest req) => req.Query;

    protected override Dictionary<string, string>? GetTelemetryTags(ProviderSearchRequest req) => new()
    {
        ["channel"] = "web",
        ["framework"] = "fastendpoints",
        ["search_term"] = req.Query
    };
}
