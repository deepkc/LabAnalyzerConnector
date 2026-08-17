using LabAnalyzerSimulator.Database.Entities;
using LabAnalyzerSimulator.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LabAnalyzerSimulator.Repositories;

public sealed class SqliteResultRepository
    : IResultRepository
{
    private readonly IDbContextFactory<SimulatorDbContext>
        _dbContextFactory;

    public SqliteResultRepository(
        IDbContextFactory<SimulatorDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task<IReadOnlyCollection<ResultEntity>>
        GetResultsByBarcodeAsync(
            string barcode)
    {
        using SimulatorDbContext db =
            _dbContextFactory.CreateDbContext();

        return await db.Results
            .AsNoTracking()
            .Where(x => x.Barcode == barcode)
            .OrderBy(x => x.TestCode)
            .ToListAsync();
    }

    public async Task AddAsync(
        ResultEntity result)
    {
        using SimulatorDbContext db =
            _dbContextFactory.CreateDbContext();

        db.Results.Add(result);

        await db.SaveChangesAsync();
    }

    public async Task UpdateAsync(
        ResultEntity result)
    {
        using SimulatorDbContext db =
            _dbContextFactory.CreateDbContext();

        db.Results.Update(result);

        await db.SaveChangesAsync();
    }

    public async Task DeleteAsync(
        Guid id)
    {
        using SimulatorDbContext db =
            _dbContextFactory.CreateDbContext();

        ResultEntity? entity =
            await db.Results.FindAsync(id);

        if (entity == null)
            return;

        db.Results.Remove(entity);

        await db.SaveChangesAsync();
    }
}