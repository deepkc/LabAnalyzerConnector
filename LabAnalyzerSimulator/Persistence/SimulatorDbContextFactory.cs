using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace LabAnalyzerSimulator.Persistence;

public sealed class SimulatorDbContextFactory
    : IDesignTimeDbContextFactory<SimulatorDbContext>
{
    public SimulatorDbContext CreateDbContext(
        string[] args)
    {
        var options =
            new DbContextOptionsBuilder<SimulatorDbContext>();

        options.UseSqlite(
            "Data Source=Simulator.db");

        return new SimulatorDbContext(
            options.Options);
    }
}