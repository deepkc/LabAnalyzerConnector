using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace LabAnalyzerConnector.Infrastructure.Persistence;

public sealed class LabAnalyzerDbContextFactory
    : IDesignTimeDbContextFactory<LabAnalyzerDbContext>
{
    public LabAnalyzerDbContext CreateDbContext(
        string[] args)
    {
        string dataDirectory =
            Path.Combine(
                AppContext.BaseDirectory,
                "Data");

        Directory.CreateDirectory(
            dataDirectory);

        string databasePath =
            Path.Combine(
                dataDirectory,
                "LabAnalyzerConnector.db");

        string connectionString =
            $"Data Source={databasePath}";

        DbContextOptionsBuilder<LabAnalyzerDbContext>
            optionsBuilder =
                new();

        optionsBuilder.UseSqlite(
            connectionString);

        return new LabAnalyzerDbContext(
            optionsBuilder.Options);
    }
}