using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using ProviderInbox.Domain;
using ProviderInbox.Service.Infrastructure;
using ProviderInbox.Service.Shared.Security;

namespace ProviderInbox.Service.Features.Inbox.MarkRead;

public sealed class Response
{
    public Guid Id { get; set; }
    public MessageStatus Status { get; set; }
    public DateTimeOffset? ReadAt { get; set; }
}

public sealed class Endpoint : EndpointWithoutRequest<Response>
{
    private readonly AppDbContext _db;
    private readonly IAudienceContext _aud;

    public Endpoint(AppDbContext db, IAudienceContext aud) { _db = db; _aud = aud; }

    public override void Configure()
    {
        Post("/inbox/messages/{id:guid}/read");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var (npi, office) = _aud.GetAudience();
        var id = Route<Guid>("id");

        var msg = await _db.InboxMessages
            .Where(x => x.Npi == npi && x.OfficeNumber == office && x.Id == id)
            .FirstOrDefaultAsync(ct);

        if (msg is null) { await Send.NotFoundAsync(ct); return; }

        if (msg.Status == MessageStatus.Unread)
        {
            msg.Status = MessageStatus.Read;
            msg.ReadAt = DateTimeOffset.UtcNow;
            msg.UpdatedAt = DateTimeOffset.UtcNow;
            await _db.SaveChangesAsync(ct);
        }

        await Send.OkAsync(new Response { Id = msg.Id, Status = msg.Status, ReadAt = msg.ReadAt }, cancellation: ct);
    }
}
