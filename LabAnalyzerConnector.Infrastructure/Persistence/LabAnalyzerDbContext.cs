using LabAnalyzerConnector.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace LabAnalyzerConnector.Infrastructure.Persistence;

public sealed class LabAnalyzerDbContext
    : DbContext
{
    public LabAnalyzerDbContext(
        DbContextOptions<LabAnalyzerDbContext> options)
        : base(options)
    {
    }


    // =====================================================
    // ANALYZER CONFIGURATIONS
    // =====================================================

    public DbSet<AnalyzerConfigurationEntity>
        AnalyzerConfigurations =>
            Set<AnalyzerConfigurationEntity>();


    // =====================================================
    // LAB ORDERS
    // =====================================================

    public DbSet<LabOrderEntity>
        LabOrders =>
            Set<LabOrderEntity>();


    // =====================================================
    // LAB RESULTS
    // =====================================================

    public DbSet<LabResultEntity>
        LabResults =>
            Set<LabResultEntity>();

    public DbSet<AnalyzerMappingProfileEntity>
    AnalyzerMappingProfiles =>
        Set<AnalyzerMappingProfileEntity>();

    public DbSet<TestCodeMappingEntity>
        TestCodeMappings =>
            Set<TestCodeMappingEntity>();


    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        base.OnModelCreating(
            modelBuilder);


        // =================================================
        // ANALYZER CONFIGURATION
        // =================================================

        modelBuilder.Entity<AnalyzerConfigurationEntity>(
    entity =>
    {
        entity.HasIndex(
            x => x.AnalyzerId)
            .IsUnique();

        entity.Property(
            x => x.Name)
            .IsRequired();

        entity.Property(
            x => x.ProtocolJson)
            .IsRequired();
    });


        // =================================================
        // LAB ORDER
        // =================================================

        modelBuilder.Entity<LabOrderEntity>(
            entity =>
            {
                entity.HasKey(
                    x => x.Id);


                entity.HasIndex(
                    x => x.Barcode)
                    .IsUnique();


                entity.HasIndex(
                    x => x.AnalyzerId);


                entity.HasIndex(
                    x => x.OrderId);


                entity.HasIndex(
                    x => x.PatientId);


                entity.HasIndex(
                    x => x.SpecimenId);


                entity.Property(
                    x => x.OrderId)
                    .IsRequired();


                entity.Property(
                    x => x.PatientId)
                    .IsRequired();


                entity.Property(
                    x => x.PatientName)
                    .IsRequired();


                entity.Property(
                    x => x.SpecimenId)
                    .IsRequired();


                entity.Property(
                    x => x.Barcode)
                    .IsRequired();


                entity.Property(
                    x => x.OrderedTests)
                    .IsRequired();


                entity.Property(
                    x => x.Priority)
                    .IsRequired();


                entity.Property(
                    x => x.Status)
                    .IsRequired();
            });

        // =====================================================
        // ANALYZER MAPPING PROFILE
        // =====================================================

        modelBuilder.Entity<AnalyzerMappingProfileEntity>(
            entity =>
            {
                entity.HasKey(
                    x => x.Id);

                entity.HasIndex(
                    x => x.AnalyzerId)
                    .IsUnique();

                entity.Property(
                    x => x.AnalyzerName)
                    .IsRequired();
            });


        // =====================================================
        // TEST CODE MAPPING
        // =====================================================

        modelBuilder.Entity<TestCodeMappingEntity>(
            entity =>
            {
                entity.HasKey(
                    x => x.Id);

                entity.HasIndex(
                    x => x.AnalyzerId);

                entity.HasIndex(
                    x => new
                    {
                        x.AnalyzerId,
                        x.AnalyzerTestCode
                    })
                    .IsUnique();

                entity.Property(
                    x => x.AnalyzerTestCode)
                    .IsRequired();

                entity.Property(
                    x => x.StandardTestCode)
                    .IsRequired();

                entity.Property(
                    x => x.StandardTestName);

                entity.Property(
                    x => x.AnalyzerTestName);

                entity.Property(
                    x => x.ExpectedUnit);

                entity.Property(
                    x => x.StandardUnit);

                entity.Property(
                    x => x.IsActive)
                    .IsRequired();
            });


        // =================================================
        // LAB RESULT
        // =================================================

        modelBuilder.Entity<LabResultEntity>(
            entity =>
            {
                entity.HasKey(
                    x => x.Id);


                entity.HasIndex(
                    x => x.AnalyzerId);


                entity.HasIndex(
                    x => x.SampleId);


                entity.HasIndex(
                    x => x.PatientId);


                entity.HasIndex(
                    x => x.StandardTestCode);


                entity.Property(
                    x => x.ResultValue);


                entity.Property(
                    x => x.Units);


                entity.Property(
                    x => x.ReferenceRange);


                entity.Property(
                    x => x.AbnormalFlag);


                entity.Property(
                    x => x.RawMessage);
            });
    }
}