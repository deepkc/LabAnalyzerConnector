using Microsoft.EntityFrameworkCore;

namespace LabAnalyzerConnector.Tests;

public sealed class TestDbContextFactory<TContext>
    : IDbContextFactory<TContext>
    where TContext : DbContext
{
    private readonly DbContextOptions<TContext>
        _options;

    public TestDbContextFactory(
        DbContextOptions<TContext> options)
    {
        _options =
            options
            ?? throw new ArgumentNullException(
                nameof(options));
    }

    public TContext CreateDbContext()
    {
        return (TContext)Activator.CreateInstance(
            typeof(TContext),
            _options)!;
    }

    public Task<TContext> CreateDbContextAsync(
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(
            CreateDbContext());
    }
}