using FeatureTelemetry.SampleApi.Models;
using FeatureTelemetry.SampleApi.Services;
using FeatureTelemetry.SampleApi.Telemetry;

namespace FeatureTelemetry.SampleApi.Endpoints;

[FeatureTelemetry("Submission", "Secure PHI Submission FE")]
public sealed class SubmitPhiFormEndpoint : TelemetryEndpoint<SubmitPhiFormRequest, SubmitPhiFormResponse>
{
    private IPhiSubmissionService _phiSubmissionService = default!;

    protected override void ConfigureEndpoint()
    {
        Post("/phi/submit-fe");
        AllowAnonymous();
    }

    public override Task OnBeforeHandleAsync(SubmitPhiFormRequest req, CancellationToken ct)
    {
        _phiSubmissionService = Resolve<IPhiSubmissionService>();
        return Task.CompletedTask;
    }

    protected override async Task<SubmitPhiFormResponse> ExecuteFeatureAsync(SubmitPhiFormRequest req, CancellationToken ct)
    {
        var submissionId = await _phiSubmissionService.SubmitAsync(req, ct);
        return new SubmitPhiFormResponse
        {
            SubmissionId = submissionId,
            Status = "Submitted"
        };
    }

    protected override string? GetEntityId(SubmitPhiFormRequest req) => req.MemberId;

    protected override Dictionary<string, string>? GetTelemetryTags(SubmitPhiFormRequest req) => new()
    {
        ["channel"] = "member-portal",
        ["framework"] = "fastendpoints",
        ["has_attachments"] = req.HasAttachments.ToString()
    };
}
