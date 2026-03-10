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

        // Resolve the attribute directly from the endpoint class itself
        var attr = GetType()
            .GetCustomAttributes(typeof(FeatureTelemetryAttribute), inherit: true)
            .OfType<FeatureTelemetryAttribute>()
            .FirstOrDefault();

        if (attr is null)
            throw new InvalidOperationException(
                $"{GetType().Name} is missing FeatureTelemetryAttribute.");

        // Always put telemetry metadata into HttpContext.Items
        HttpContext.Items["Telemetry.ModuleName"] = attr.ModuleName;
        HttpContext.Items["Telemetry.FeatureName"] = attr.FeatureName;

        // Let derived endpoints resolve services here if needed
        await OnBeforeHandleAsync(req, ct);

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

    protected virtual Task OnBeforeHandleAsync(TRequest req, CancellationToken ct)
        => Task.CompletedTask;

    protected abstract Task<TResponse> ExecuteFeatureAsync(TRequest req, CancellationToken ct);

    protected virtual string? GetEntityId(TRequest req) => null;

    protected virtual Dictionary<string, string>? GetTelemetryTags(TRequest req) => null;
}