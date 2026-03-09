using Microsoft.EntityFrameworkCore;
using ProviderInbox.Domain;
using ProviderInbox.Service.Features.Inbox.Reprioritize;
using ProviderInbox.Service.Infrastructure;

namespace ProviderInbox.Service.Seeds;

public sealed class SeedDataHostedService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IConfiguration _cfg;
    private readonly ILogger<SeedDataHostedService> _log;

    public SeedDataHostedService(IServiceScopeFactory scopeFactory, IConfiguration cfg, ILogger<SeedDataHostedService> log)
    {
        _scopeFactory = scopeFactory;
        _cfg = cfg;
        _log = log;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
        if (!_cfg.GetValue("Seed:Enabled", false)) return;

        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var rules = scope.ServiceProvider.GetRequiredService<PriorityRulesEngine>();

        try { await db.Database.MigrateAsync(stoppingToken); }
        catch { await db.Database.EnsureCreatedAsync(stoppingToken); }

        var npi = _cfg.GetValue<string>("Seed:Npi") ?? "1234567890";
        var office = _cfg.GetValue<string>("Seed:OfficeNumber") ?? "OFFICE-001";
        var count = _cfg.GetValue("Seed:Count", 50);

        if (await db.InboxMessages.AnyAsync(x => x.Npi == npi && x.OfficeNumber == office, stoppingToken))
        {
            _log.LogInformation("Seed skipped (existing data).");
            return;
        }

        var now = DateTimeOffset.UtcNow;
        var rnd = new Random(42);
        var types = Enum.GetValues<MessageType>();

        for (int i = 0; i < count; i++)
        {
            var t = types[rnd.Next(types.Length)];
            var actionable = rnd.NextDouble() < 0.35;
            var status = rnd.NextDouble() < 0.15 ? MessageStatus.Read : MessageStatus.Unread;

            DateTimeOffset? due = null;
            int? sla = null;
            if (actionable)
            {
                var bucket = rnd.Next(3);
                due = bucket switch
                {
                    0 => now.AddHours(rnd.Next(1, 12)),
                    1 => now.AddHours(rnd.Next(12, 72)),
                    _ => now.AddHours(-rnd.Next(1, 24)),
                };
                sla = rnd.Next(12, 72);
            }

            var m = new InboxMessage
            {
                Npi = npi,
                OfficeNumber = office,
                Type = t,
                Title = t switch
                {
                    MessageType.Claim => actionable ? $"Claim requires documentation (#{i})" : $"Claim submission confirmation (#{i})",
                    MessageType.Eligibility => actionable ? $"Eligibility issue requires review (#{i})" : $"Eligibility confirmation received (#{i})",
                    MessageType.Payment => actionable ? $"Refund request pending approval (#{i})" : $"Payment/EFT notice posted (#{i})",
                    _ => actionable ? $"System alert: action needed (#{i})" : $"System notice (#{i})"
                },
                Summary = "Demo seed message",
                ReferenceId = $"{t.ToString().ToUpperInvariant()}-{100000 + i}",
                ActionRequired = actionable,
                DueAt = due,
                SlaHours = sla,
                Status = status,
                CreatedAt = now.AddMinutes(-rnd.Next(0, 60 * 96)),
                UpdatedAt = now.AddMinutes(-rnd.Next(0, 60 * 72))
            };

            rules.Apply(m, now);
            db.InboxMessages.Add(m);
        }

        await db.SaveChangesAsync(stoppingToken);
        _log.LogInformation("Seeded {Count} messages.", count);
    }
}
