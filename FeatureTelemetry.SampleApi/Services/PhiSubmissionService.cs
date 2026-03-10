using FeatureTelemetry.SampleApi.Models;

namespace FeatureTelemetry.SampleApi.Services;

public sealed class PhiSubmissionService : IPhiSubmissionService
{
    public async Task<string> SubmitAsync(SubmitPhiFormRequest req, CancellationToken ct)
    {
        await Task.Delay(200, ct);

        if (req.MemberId == "timeout")
            throw new TimeoutException("PHI submission timed out.");

        if (req.MemberId == "error")
            throw new InvalidOperationException("Submission failed.");

        return Guid.NewGuid().ToString("N");
    }
}
