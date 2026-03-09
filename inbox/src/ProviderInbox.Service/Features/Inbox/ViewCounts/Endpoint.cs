using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using ProviderInbox.Domain;
using ProviderInbox.Service.Infrastructure;
using ProviderInbox.Service.Shared.Security;

namespace ProviderInbox.Service.Features.Inbox.ViewCounts;

public sealed class Response
{
    public int Total { get; set; }
    public int Unread { get; set; }
    public int P0 { get; set; }
    public int P1 { get; set; }
    public int Informational { get; set; }
    public int Completed { get; set; }
}

public sealed class Endpoint : EndpointWithoutRequest<Response>
{
    private readonly AppDbContext _db;
    private readonly IAudienceContext _aud;

    public Endpoint(AppDbContext db, IAudienceContext aud) { _db = db; _aud = aud; }

    public override void Configure()
    {
        Get("/inbox/views");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var (npi, office) = _aud.GetAudience();
        var q = _db.InboxMessages.AsNoTracking().Where(x => x.Npi == npi && x.OfficeNumber == office);

        var total = await q.CountAsync(ct);
        var unread = await q.Where(x => x.Status == MessageStatus.Unread).CountAsync(ct);
        var p0 = await q.Where(x => x.Priority == PriorityLevel.P0 && x.Status != MessageStatus.Completed).CountAsync(ct);
        var p1 = await q.Where(x => x.Priority == PriorityLevel.P1 && x.Status != MessageStatus.Completed).CountAsync(ct);
        var info = await q.Where(x => x.Priority == PriorityLevel.P2 && x.Status != MessageStatus.Completed).CountAsync(ct);
        var completed = await q.Where(x => x.Status == MessageStatus.Completed).CountAsync(ct);

        await Send.OkAsync(new Response { Total = total, Unread = unread, P0 = p0, P1 = p1, Informational = info, Completed = completed }, cancellation: ct);
    }
}
