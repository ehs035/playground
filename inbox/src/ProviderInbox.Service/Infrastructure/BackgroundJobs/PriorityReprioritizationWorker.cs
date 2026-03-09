using ProviderInbox.Service.Features.Inbox.Reprioritize;

namespace ProviderInbox.Service.Infrastructure.BackgroundJobs;

public sealed class PriorityReprioritizationWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<PriorityReprioritizationWorker> _log;
    private readonly IConfiguration _cfg;

    public PriorityReprioritizationWorker(IServiceScopeFactory scopeFactory, ILogger<PriorityReprioritizationWorker> log, IConfiguration cfg)
    {
        _scopeFactory = scopeFactory;
        _log = log;
        _cfg = cfg;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var intervalSeconds = _cfg.GetValue("Reprioritization:IntervalSeconds", 300);
        var batchSize = _cfg.GetValue("Reprioritization:BatchSize", 500);

        var timer = new PeriodicTimer(TimeSpan.FromSeconds(intervalSeconds));
        _log.LogInformation("PriorityReprioritizationWorker started: interval={IntervalSeconds}s batchSize={BatchSize}", intervalSeconds, batchSize);

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var svc = scope.ServiceProvider.GetRequiredService<IInboxReprioritizeService>();
                var updated = await svc.RunOnceAsync(DateTimeOffset.UtcNow, batchSize, stoppingToken);
                _log.LogInformation("Reprioritization complete. Updated={Updated}", updated);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested) { break; }
            catch (Exception ex) { _log.LogError(ex, "Reprioritization failed."); }
        }
    }
}
