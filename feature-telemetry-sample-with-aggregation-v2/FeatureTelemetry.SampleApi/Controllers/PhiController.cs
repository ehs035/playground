using FeatureTelemetry.SampleApi.Models;
using FeatureTelemetry.SampleApi.Services;
using FeatureTelemetry.SampleApi.Telemetry;
using Microsoft.AspNetCore.Mvc;
namespace FeatureTelemetry.SampleApi.Controllers;

[Route("api/phi")]
public sealed class PhiController : TelemetryControllerBase
{
    private readonly IPhiSubmissionService _phiSubmissionService;
    public PhiController(IFeatureTelemetry telemetry, IPhiSubmissionService phiSubmissionService) : base(telemetry)
    {
        _phiSubmissionService = phiSubmissionService;
    }

    [HttpPost("submit")]
    [FeatureTelemetry("Submission", "Secure PHI Submission MVC")]
    public async Task<ActionResult<SubmitPhiFormResponse>> Submit([FromBody] SubmitPhiFormRequest request, CancellationToken ct)
    {
        var response = await ExecuteFeatureAsync(
          async token => {
              var id = await _phiSubmissionService.SubmitAsync(request, token);
              return new SubmitPhiFormResponse
              {
                  SubmissionId = id,
                  Status = "Submitted"
              };
          }, request.MemberId, new()
          {
              ["channel"] = "member-portal",
              ["framework"] = "mvc",
              ["has_attachments"] = request.HasAttachments.ToString()
          }, ct);
        return Ok(response);
    }
}