using FastEndpoints;

public class CustomAttributeEndpoint : EndpointWithoutRequest
{
    public override void Configure()
    {
        Routes("/fast/custom");
        Verbs("GET");
        AuthSchemes("Bearer");
        Policies("RequireCustomAttribute");
    }

    public override Task HandleAsync(CancellationToken ct)
    {
        return Send.OkAsync(new { msg = "fastendpoint custom attribute ok" }, ct);
    }
}
