using FeatureTelemetry.SampleApi.Telemetry;
using Microsoft.AspNetCore.Mvc;

namespace FeatureTelemetry.SampleApi.Controllers;

[ApiController]
[Route("api/telemetry")]
public sealed class TelemetrySummaryController : ControllerBase
{
    private readonly IFeatureTelemetrySummaryStore _summaryStore;

    public TelemetrySummaryController(IFeatureTelemetrySummaryStore summaryStore)
    {
        _summaryStore = summaryStore;
    }

    [HttpGet("summary")]
    public ActionResult<IReadOnlyCollection<FeatureTelemetryAggregate>> GetSummary()
        => Ok(_summaryStore.GetAggregates());

    [HttpGet("events")]
    public ActionResult<IReadOnlyCollection<FeatureTelemetryEvent>> GetEvents()
        => Ok(_summaryStore.GetAllEvents()
            .OrderByDescending(x => x.OccurredAtUtc)
            .Take(200)
            .ToArray());
}
