using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using ProviderInbox.Domain;
using ProviderInbox.Service.Features.Inbox.Reprioritize;
using ProviderInbox.Service.Infrastructure;
using ProviderInbox.Service.Shared.Security;

namespace ProviderInbox.Service.Features.Inbox.Complete;

public sealed class Response
{
    public Guid Id { get; set; }
    public MessageStatus Status { get; set; }
    public PriorityLevel Priority { get; set; }
    public bool Pinned { get; set; }
    public DateTimeOffset? CompletedAt { get; set; }
}

public sealed class Endpoint : EndpointWithoutRequest<Response>
{
    private readonly AppDbContext _db;
    private readonly IAudienceContext _aud;
    private readonly PriorityRulesEngine _rules;

    public Endpoint(AppDbContext db, IAudienceContext aud, PriorityRulesEngine rules)
    {
        _db = db; _aud = aud; _rules = rules;
    }

    public override void Configure()
    {
        Post("/inbox/messages/{id:guid}/complete");
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

        msg.Status = MessageStatus.Completed;
        msg.CompletedAt = DateTimeOffset.UtcNow;
        msg.UpdatedAt = DateTimeOffset.UtcNow;

        _rules.Apply(msg, DateTimeOffset.UtcNow);

        await _db.SaveChangesAsync(ct);

        await Send.OkAsync(new Response { Id = msg.Id, Status = msg.Status, Priority = msg.Priority, Pinned = msg.Pinned, CompletedAt = msg.CompletedAt }, cancellation: ct);
    }
}
