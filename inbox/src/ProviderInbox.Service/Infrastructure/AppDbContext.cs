using Microsoft.EntityFrameworkCore;
using ProviderInbox.Domain;

namespace ProviderInbox.Service.Infrastructure;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<InboxMessage> InboxMessages => Set<InboxMessage>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<InboxMessage>()
            .HasIndex(x => new { x.Npi, x.OfficeNumber, x.Pinned, x.Priority, x.Status, x.DueAt, x.UpdatedAt });

        b.Entity<InboxMessage>()
            .HasIndex(x => new { x.Npi, x.OfficeNumber, x.Type, x.CreatedAt });

        base.OnModelCreating(b);
    }
}
