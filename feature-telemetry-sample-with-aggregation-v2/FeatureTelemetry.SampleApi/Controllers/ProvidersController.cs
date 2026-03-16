using FeatureTelemetry.SampleApi.Models;
using FeatureTelemetry.SampleApi.Services;
using FeatureTelemetry.SampleApi.Telemetry;
using Microsoft.AspNetCore.Mvc;
namespace FeatureTelemetry.SampleApi.Controllers;

[Route("api/providers")]
public sealed class ProvidersController : TelemetryControllerBase
{
    private readonly IProviderSearchService _providerSearchService;
    public ProvidersController(IFeatureTelemetry telemetry, IProviderSearchService providerSearchService) : base(telemetry)
    {
        _providerSearchService = providerSearchService;
    }

    [HttpPost("search")]
    [FeatureTelemetry("Search", "Ralfs wOLRD")]
    public async Task<ActionResult<ProviderSearchResponse>> Search([FromBody] ProviderSearchRequest request, CancellationToken ct)
    {
        var response = await ExecuteFeatureAsync(async token => {
            var providers = await _providerSearchService.SearchAsync(request.Query, token);
            return new ProviderSearchResponse
            {
                Providers = providers
            };
        }, request.Query, new()
        {
            ["channel"] = "web",
            ["framework"] = "mvc",
            ["search_term"] = request.Query
        }, ct);
        return Ok(response);
    }
}