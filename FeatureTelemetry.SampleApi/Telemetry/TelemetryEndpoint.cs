using FastEndpoints;

namespace FeatureTelemetry.SampleApi.Telemetry;

public abstract class TelemetryEndpoint<TRequest, TResponse> : Endpoint<TRequest, TResponse>
    where TRequest : notnull
{
    protected IFeatureTelemetry Telemetry { get; private set; } = default!;

    public override void Configure()
    {
        ConfigureEndpoint();
    }

    protected abstract void ConfigureEndpoint();

    public override async Task HandleAsync(TRequest req, CancellationToken ct)
    {
        Telemetry = Resolve<IFeatureTelemetry>();

        var entityId = GetEntityId(req);
        var tags = GetTelemetryTags(req);

        var response = await Telemetry.ExecuteAsync(
            HttpContext,
            token => ExecuteFeatureAsync(req, token),
            entityId,
            tags,
            ct);

        await SendOkAsync(response, ct);
    }

    protected abstract Task<TResponse> ExecuteFeatureAsync(TRequest req, CancellationToken ct);

    protected virtual string? GetEntityId(TRequest req) => null;

    protected virtual Dictionary<string, string>? GetTelemetryTags(TRequest req) => null;
}
