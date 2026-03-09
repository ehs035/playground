using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ProviderInbox.Service.Infrastructure;

public sealed class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        // Design-time only fallback. At runtime, Aspire injects the connection string.
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer("Server=localhost,1433;Database=providerinboxdb;User Id=sa;Password=Your_password123;TrustServerCertificate=True;Encrypt=False")
            .Options;

        return new AppDbContext(options);
    }
}
