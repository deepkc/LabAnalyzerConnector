using LabAnalyzerSimulator.Database.Entities;
using LabAnalyzerSimulator.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LabAnalyzerSimulator.Services;

public sealed class DatabaseSeeder
{
    private readonly IDbContextFactory<SimulatorDbContext>
        _dbContextFactory;

    public DatabaseSeeder(
        IDbContextFactory<SimulatorDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task SeedAsync()
    {
        using SimulatorDbContext db =
            _dbContextFactory.CreateDbContext();

        if (await db.Patients.AnyAsync())
        {
            return;
        }

        // Patients

        // Orders

        // Results

        await db.SaveChangesAsync();
    }
}