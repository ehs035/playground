using FastEndpoints;

public class RoleEndpoint : EndpointWithoutRequest
{
    public override void Configure()
    {
        Routes("/fast/role");
        Verbs("GET");
        AuthSchemes("Bearer");
        Policies("RequireAdminRole");
    }

    public override Task HandleAsync(CancellationToken ct)
    {
        return Send.OkAsync(new { msg = "fastendpoint role ok" }, ct);
    }
}
