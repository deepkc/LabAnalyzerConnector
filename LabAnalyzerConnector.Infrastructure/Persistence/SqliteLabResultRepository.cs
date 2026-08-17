using LabAnalyzerConnector.Domain.Abstractions;
using LabAnalyzerConnector.Domain.Entities;
using LabAnalyzerConnector.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;


namespace LabAnalyzerConnector.Infrastructure.Persistence;

public sealed class SqliteLabResultRepository
    : ILabResultRepository
{
    private readonly IDbContextFactory<
        LabAnalyzerDbContext>
        _dbContextFactory;


    public SqliteLabResultRepository(
        IDbContextFactory<LabAnalyzerDbContext>
            dbContextFactory)
    {
        _dbContextFactory =
            dbContextFactory
            ?? throw new ArgumentNullException(
                nameof(dbContextFactory));
    }


    // =====================================================
    // ADD RESULT
    // =====================================================

    public async Task AddAsync(
     LabResult result,
     CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(result);

        LabResultEntity entity =
            ToEntity(result);

        await using LabAnalyzerDbContext dbContext =
            await _dbContextFactory.CreateDbContextAsync(
                cancellationToken);

        await dbContext.LabResults.AddAsync(
            entity,
            cancellationToken);

        int saved =
            await dbContext.SaveChangesAsync(
                cancellationToken);

        System.Diagnostics.Debug.WriteLine(
            $"DATABASE SAVE -> LabResult rows affected = {saved}");

        System.Diagnostics.Debug.WriteLine(
            $"DATABASE SAVE -> Id = {entity.Id}");

        System.Diagnostics.Debug.WriteLine(
            $"DATABASE SAVE -> TestCode = {entity.TestCode}");

        System.Diagnostics.Debug.WriteLine(
            $"DATABASE SAVE -> StandardTestCode = {entity.StandardTestCode}");

        System.Diagnostics.Debug.WriteLine(
            $"DATABASE SAVE -> ResultValue = {entity.ResultValue}");
    }


    // =====================================================
    // GET BY ID
    // =====================================================

    public async Task<LabResult?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        await using LabAnalyzerDbContext dbContext =
            await _dbContextFactory.CreateDbContextAsync(
                cancellationToken);


        LabResultEntity? entity =
            await dbContext.LabResults
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);


        return entity is null
            ? null
            : ToDomain(
                entity);
    }


    // =====================================================
    // GET ALL
    // =====================================================

    public async Task<IReadOnlyCollection<LabResult>>
        GetAllAsync(
            CancellationToken cancellationToken = default)
    {
        await using LabAnalyzerDbContext dbContext =
            await _dbContextFactory.CreateDbContextAsync(
                cancellationToken);


        List<LabResultEntity> entities =
            await dbContext.LabResults
                .AsNoTracking()
                .OrderByDescending(
                    x => x.ReceivedAtUtc)
                .ToListAsync(
                    cancellationToken);


        return entities
            .Select(
                ToDomain)
            .ToList();
    }


    // =====================================================
    // GET BY SAMPLE ID
    // =====================================================

    public async Task<IReadOnlyCollection<LabResult>>
        GetBySampleIdAsync(
            string sampleId,
            CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(
                sampleId))
        {
            return Array.Empty<LabResult>();
        }


        string normalizedSampleId =
            sampleId.Trim();


        await using LabAnalyzerDbContext dbContext =
            await _dbContextFactory.CreateDbContextAsync(
                cancellationToken);


        List<LabResultEntity> entities =
            await dbContext.LabResults
                .AsNoTracking()
                .Where(
                    x =>
                        x.SampleId ==
                        normalizedSampleId)
                .OrderByDescending(
                    x => x.ReceivedAtUtc)
                .ToListAsync(
                    cancellationToken);


        return entities
            .Select(
                ToDomain)
            .ToList();
    }


    // =====================================================
    // GET BY PATIENT ID
    // =====================================================

    public async Task<IReadOnlyCollection<LabResult>>
        GetByPatientIdAsync(
            string patientId,
            CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(
                patientId))
        {
            return Array.Empty<LabResult>();
        }


        string normalizedPatientId =
            patientId.Trim();


        await using LabAnalyzerDbContext dbContext =
            await _dbContextFactory.CreateDbContextAsync(
                cancellationToken);


        List<LabResultEntity> entities =
            await dbContext.LabResults
                .AsNoTracking()
                .Where(
                    x =>
                        x.PatientId ==
                        normalizedPatientId)
                .OrderByDescending(
                    x => x.ReceivedAtUtc)
                .ToListAsync(
                    cancellationToken);


        return entities
            .Select(
                ToDomain)
            .ToList();
    }


    // =====================================================
    // DOMAIN → ENTITY
    // =====================================================

    private static LabResultEntity ToEntity(
        LabResult result)
    {
        return new LabResultEntity
        {
            Id =
                result.Id,

            AnalyzerId =
                result.AnalyzerId,

            AnalyzerName =
                result.AnalyzerName,

            PatientId =
                result.PatientId,

            SampleId =
                result.SampleId,

            TestCode =
                result.TestCode,

            StandardTestCode =
                result.StandardTestCode,

            TestName =
                result.TestName,

            ResultValue =
                result.ResultValue,

            Units =
                result.Units,

            ReferenceRange =
                result.ReferenceRange,

            AbnormalFlag =
                result.AbnormalFlag,

            ResultDateTime =
                result.ResultDateTime,

            ReceivedAtUtc =
                result.ReceivedAtUtc,

            RawMessage =
                result.RawMessage
        };
    }


    // =====================================================
    // ENTITY → DOMAIN
    // =====================================================

    private static LabResult ToDomain(
        LabResultEntity entity)
    {
        return new LabResult
        {
            Id =
                entity.Id,

            AnalyzerId =
                entity.AnalyzerId,

            AnalyzerName =
                entity.AnalyzerName,

            PatientId =
                entity.PatientId,

            SampleId =
                entity.SampleId,

            TestCode =
                entity.TestCode,

            StandardTestCode =
                entity.StandardTestCode,

            TestName =
                entity.TestName,

            ResultValue =
                entity.ResultValue,

            Units =
                entity.Units,

            ReferenceRange =
                entity.ReferenceRange,

            AbnormalFlag =
                entity.AbnormalFlag,

            ResultDateTime =
                entity.ResultDateTime,

            ReceivedAtUtc =
                entity.ReceivedAtUtc,

            RawMessage =
                entity.RawMessage
        };
    }

    // =====================================================
    // GET BY ANALYZER ID
    // =====================================================

    public async Task<IReadOnlyCollection<LabResult>>
        GetByAnalyzerIdAsync(
            Guid analyzerId,
            CancellationToken cancellationToken = default)
    {
        await using LabAnalyzerDbContext dbContext =
            await _dbContextFactory.CreateDbContextAsync(
                cancellationToken);

        List<LabResultEntity> entities =
            await dbContext.LabResults
                .AsNoTracking()
                .Where(
                    x => x.AnalyzerId == analyzerId)
                .OrderByDescending(
                    x => x.ReceivedAtUtc)
                .ToListAsync(
                    cancellationToken);

        return entities
            .Select(ToDomain)
            .ToList();
    }
}