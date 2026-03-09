using Microsoft.EntityFrameworkCore;
using ProviderInbox.Domain;
using ProviderInbox.Service.Infrastructure;

namespace ProviderInbox.Service.Features.Inbox.Reprioritize;

public interface IInboxReprioritizeService
{
    Task<int> RunOnceAsync(DateTimeOffset nowUtc, int batchSize, CancellationToken ct);
}

public sealed class InboxReprioritizeService : IInboxReprioritizeService
{
    private readonly AppDbContext _db;
    private readonly PriorityRulesEngine _rules;

    public InboxReprioritizeService(AppDbContext db, PriorityRulesEngine rules)
    {
        _db = db;
        _rules = rules;
    }

    public async Task<int> RunOnceAsync(DateTimeOffset nowUtc, int batchSize, CancellationToken ct)
    {
        var updated = 0;

        var candidates = _db.InboxMessages
            .Where(m => m.Status != MessageStatus.Completed)
            .Where(m => m.ActionRequired)
            .Where(m => m.DueAt != null)
            .Where(m => m.SnoozedUntil == null || m.SnoozedUntil <= nowUtc)
            .OrderBy(m => m.Id)
            .AsQueryable();

        Guid? last = null;

        while (true)
        {
            var batch = await candidates
                .Where(m => last == null || m.Id.CompareTo(last.Value) > 0)
                .Take(batchSize)
                .ToListAsync(ct);

            if (batch.Count == 0) break;

            foreach (var m in batch)
            {
                if (_rules.Apply(m, nowUtc))
                {
                    m.UpdatedAt = nowUtc;
                    updated++;
                }
                last = m.Id;
            }

            if (updated > 0)
                await _db.SaveChangesAsync(ct);
        }

        return updated;
    }
}
