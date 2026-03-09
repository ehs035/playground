using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using ProviderInbox.Domain;
using ProviderInbox.Service.Infrastructure;
using ProviderInbox.Service.Shared.Contracts;
using ProviderInbox.Service.Shared.Security;

namespace ProviderInbox.Service.Features.Inbox.ListMessages;

public sealed class Endpoint : Endpoint<Request, PagedResponse<Item>>
{
    private readonly AppDbContext _db;
    private readonly IAudienceContext _aud;

    public Endpoint(AppDbContext db, IAudienceContext aud) { _db = db; _aud = aud; }

    public override void Configure()
    {
        Get("/inbox/messages");
        AllowAnonymous();
    }

    public override async Task HandleAsync(Request r, CancellationToken ct)
    {
        var (npi, office) = _aud.GetAudience();
        var now = DateTimeOffset.UtcNow;

        var q = _db.InboxMessages.AsNoTracking()
            .Where(x => x.Npi == npi && x.OfficeNumber == office)
            .Where(x => x.SnoozedUntil == null || x.SnoozedUntil <= now);

        if (r.Type is not null) q = q.Where(x => x.Type == r.Type);
        if (r.Status is not null) q = q.Where(x => x.Status == r.Status);
        if (r.Priority is not null) q = q.Where(x => x.Priority == r.Priority);
        if (r.ActionRequired is not null) q = q.Where(x => x.ActionRequired == r.ActionRequired);

        if (!string.IsNullOrWhiteSpace(r.Search))
        {
            var s = r.Search.Trim();
            q = q.Where(x => x.Title.Contains(s) || (x.ReferenceId != null && x.ReferenceId.Contains(s)));
        }

        q = q
            .OrderByDescending(x => x.Pinned)
            .ThenBy(x => x.Priority)
            .ThenBy(x => x.DueAt == null)
            .ThenBy(x => x.DueAt)
            .ThenBy(x => x.Status == MessageStatus.Read)
            .ThenByDescending(x => x.UpdatedAt);

        var page = r.Page < 1 ? 1 : r.Page;
        var pageSize = r.PageSize is < 10 or > 100 ? 25 : r.PageSize;

        var total = await q.LongCountAsync(ct);

        var items = await q.Skip((page - 1) * pageSize).Take(pageSize)
            .Select(x => new Item
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
                UpdatedAt = x.UpdatedAt
            })
            .ToListAsync(ct);

        await Send.OkAsync(new PagedResponse<Item> { Page = page, PageSize = pageSize, Total = total, Items = items }, cancellation: ct);
    }
}
