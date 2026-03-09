using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using ProviderInbox.Domain;
using ProviderInbox.Service.Features.Inbox.Reprioritize;
using ProviderInbox.Service.Infrastructure;

namespace ProviderInbox.Service.Features.Inbox.IngestMessage;

public sealed class Endpoint : Endpoint<Request, Response>
{
    private readonly AppDbContext _db;
    private readonly PriorityRulesEngine _rules;

    public Endpoint(AppDbContext db, PriorityRulesEngine rules)
    {
        _db = db;
        _rules = rules;
    }

    public override void Configure()
    {
        Post("/inbox/messages");
        AllowAnonymous();
    }

    public override async Task HandleAsync(Request r, CancellationToken ct)
    {
        var now = DateTimeOffset.UtcNow;

        InboxMessage msg;
        if (r.Id is not null)
        {
            msg = await _db.InboxMessages.FirstOrDefaultAsync(x => x.Id == r.Id.Value, ct)
                  ?? new InboxMessage { Id = r.Id.Value };
        }
        else msg = new InboxMessage();

        msg.Npi = r.Npi;
        msg.OfficeNumber = r.OfficeNumber;
        msg.Type = r.Type;
        msg.Title = r.Title;
        msg.Summary = r.Summary;
        msg.ReferenceId = r.ReferenceId;
        msg.ActionRequired = r.ActionRequired;
        msg.DueAt = r.DueAt;
        msg.SlaHours = r.SlaHours;
        msg.UpdatedAt = now;

        _rules.Apply(msg, now);

        _db.Update(msg);
        await _db.SaveChangesAsync(ct);

        await Send.OkAsync(new Response { Id = msg.Id, Priority = msg.Priority, Pinned = msg.Pinned }, cancellation: ct);
    }
}
