using LabAnalyzerSimulator.Database.Entities;
using LabAnalyzerSimulator.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LabAnalyzerSimulator.Repositories;

public sealed class SqliteOrderRepository
    : IOrderRepository
{
    private readonly IDbContextFactory<SimulatorDbContext>
        _dbContextFactory;

    public SqliteOrderRepository(
        IDbContextFactory<SimulatorDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task<IReadOnlyCollection<OrderEntity>>
        GetOrdersByBarcodeAsync(
            string barcode)
    {
        using SimulatorDbContext db =
            _dbContextFactory.CreateDbContext();

        return await db.Orders
            .AsNoTracking()
            .Where(x => x.Barcode == barcode)
            .OrderBy(x => x.TestCode)
            .ToListAsync();
    }

    public async Task AddAsync(
        OrderEntity order)
    {
        using SimulatorDbContext db =
            _dbContextFactory.CreateDbContext();

        db.Orders.Add(order);

        await db.SaveChangesAsync();
    }

    public async Task UpdateAsync(
        OrderEntity order)
    {
        using SimulatorDbContext db =
            _dbContextFactory.CreateDbContext();

        db.Orders.Update(order);

        await db.SaveChangesAsync();
    }

    public async Task DeleteAsync(
        Guid id)
    {
        using SimulatorDbContext db =
            _dbContextFactory.CreateDbContext();

        OrderEntity? entity =
            await db.Orders.FindAsync(id);

        if (entity == null)
            return;

        db.Orders.Remove(entity);

        await db.SaveChangesAsync();
    }
}