using LabAnalyzerSimulator.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace LabAnalyzerSimulator.Persistence;

public sealed class SimulatorDbContext
    : DbContext
{
    public SimulatorDbContext(
        DbContextOptions<SimulatorDbContext> options)
        : base(options)
    {
    }

    public DbSet<PatientEntity> Patients =>
        Set<PatientEntity>();

    public DbSet<OrderEntity> Orders =>
        Set<OrderEntity>();

    public DbSet<ResultEntity> Results =>
        Set<ResultEntity>();

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<PatientEntity>()
            .HasIndex(x => x.Barcode)
            .IsUnique();

        modelBuilder.Entity<OrderEntity>()
            .HasIndex(x => x.Barcode);

        modelBuilder.Entity<ResultEntity>()
            .HasIndex(x => x.Barcode);
    }
}