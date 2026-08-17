using Microsoft.EntityFrameworkCore;

namespace LabAnalyzerConnector.Infrastructure.Persistence;

public sealed class LabAnalyzerDatabaseInitializer
{
    private readonly IDbContextFactory<LabAnalyzerDbContext>
        _dbContextFactory;

    public LabAnalyzerDatabaseInitializer(
        IDbContextFactory<LabAnalyzerDbContext>
            dbContextFactory)
    {
        ArgumentNullException.ThrowIfNull(
            dbContextFactory);

        _dbContextFactory =
            dbContextFactory;
    }

    public async Task InitializeAsync(
        CancellationToken cancellationToken = default)
    {
        await using LabAnalyzerDbContext db =
            await _dbContextFactory
                .CreateDbContextAsync(
                    cancellationToken);

        await db.Database.MigrateAsync(
      cancellationToken);
    }
}