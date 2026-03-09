using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using ProviderInbox.Domain;
using ProviderInbox.Service.Infrastructure;
using ProviderInbox.Service.Shared.Security;

namespace ProviderInbox.Service.Features.Inbox.GetMessage;

public sealed class Response
{
    public Guid Id { get; set; }
    public MessageType Type { get; set; }
    public string Title { get; set; } = default!;
    public string? Summary { get; set; }
    public string? ReferenceId { get; set; }
    public PriorityLevel Priority { get; set; }
    public bool Pinned { get; set; }
    public bool ActionRequired { get; set; }
    public MessageStatus Status { get; set; }
    public DateTimeOffset? DueAt { get; set; }
    public int? SlaHours { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

public sealed class Endpoint : EndpointWithoutRequest<Response>
{
    private readonly AppDbContext _db;
    private readonly IAudienceContext _aud;

    public Endpoint(AppDbContext db, IAudienceContext aud) { _db = db; _aud = aud; }

    public override void Configure()
    {
        Get("/inbox/messages/{id:guid}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var (npi, office) = _aud.GetAudience();
        var id = Route<Guid>("id");

        var msg = await _db.InboxMessages.AsNoTracking()
            .Where(x => x.Npi == npi && x.OfficeNumber == office && x.Id == id)
            .Select(x => new Response
            {
                Id = x.Id,
                Type = x.Type,
                Title = x.Title,
                Summary = x.Summary,
                ReferenceId = x.ReferenceId,
                Priority = x.Priority,
                Pinned = x.Pinned,
                ActionRequired = x.ActionRequired,
                Status = x.Status,
                DueAt = x.DueAt,
                SlaHours = x.SlaHours,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt
            })
            .FirstOrDefaultAsync(ct);

        if (msg is null) { await Send.NotFoundAsync(ct); return; }
        await Send.OkAsync(msg, cancellation: ct);
    }
}
