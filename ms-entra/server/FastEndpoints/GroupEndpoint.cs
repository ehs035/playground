using FastEndpoints;

public class GroupEndpoint : EndpointWithoutRequest
{
    public override void Configure()
    {
        Routes("/fast/group");
        Verbs("GET");
        AuthSchemes("Bearer");
        Policies("RequireGroup");
        //AllowAnonymous(false);
    }

    public override Task HandleAsync(CancellationToken ct)
    {
        return Send.OkAsync(new { msg = "fastendpoint group ok" }, cancellation: ct);
    }
}
