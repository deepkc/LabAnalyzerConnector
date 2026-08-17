using LabAnalyzerConnector.Mapping.Abstractions;
using LabAnalyzerConnector.Mapping.Models;
using LabAnalyzerConnector.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace LabAnalyzerConnector.Infrastructure.Persistence;

public sealed class SqliteTestCodeMappingRepository
    : ITestCodeMappingRepository
{
    private readonly IDbContextFactory<LabAnalyzerDbContext>
        _dbContextFactory;

    public SqliteTestCodeMappingRepository(
        IDbContextFactory<LabAnalyzerDbContext> dbContextFactory)
    {
        _dbContextFactory =
            dbContextFactory
            ?? throw new ArgumentNullException(
                nameof(dbContextFactory));
    }

    // =====================================================
    // ADD
    // =====================================================

    public async Task AddAsync(
        TestCodeMapping mapping,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(mapping);

        await using LabAnalyzerDbContext dbContext =
            await _dbContextFactory.CreateDbContextAsync(
                cancellationToken);

        TestCodeMappingEntity entity =
            ToEntity(mapping);

        await dbContext.TestCodeMappings.AddAsync(
            entity,
            cancellationToken);

        await dbContext.SaveChangesAsync(
            cancellationToken);
    }

    // =====================================================
    // UPDATE
    // =====================================================

    public async Task UpdateAsync(
        TestCodeMapping mapping,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(mapping);

        await using LabAnalyzerDbContext dbContext =
            await _dbContextFactory.CreateDbContextAsync(
                cancellationToken);

        TestCodeMappingEntity? entity =
            await dbContext.TestCodeMappings
                .FirstOrDefaultAsync(
                    x => x.Id == mapping.Id,
                    cancellationToken);

        if (entity is null)
        {
            throw new InvalidOperationException(
                $"Test code mapping '{mapping.Id}' was not found.");
        }

        entity.AnalyzerId =
            mapping.AnalyzerId;

        entity.AnalyzerTestCode =
            mapping.AnalyzerTestCode;

        entity.StandardTestCode =
            mapping.StandardTestCode;

        entity.StandardTestName =
            mapping.StandardTestName;

        entity.AnalyzerTestName =
            mapping.AnalyzerTestName;

        entity.ExpectedUnit =
            mapping.ExpectedUnit;

        entity.StandardUnit =
            mapping.StandardUnit;

        entity.IsActive =
            mapping.IsActive;

        await dbContext.SaveChangesAsync(
            cancellationToken);
    }

    // =====================================================
    // DELETE
    // =====================================================

    public async Task<bool> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        await using LabAnalyzerDbContext dbContext =
            await _dbContextFactory.CreateDbContextAsync(
                cancellationToken);

        TestCodeMappingEntity? entity =
            await dbContext.TestCodeMappings
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);

        if (entity is null)
        {
            return false;
        }

        dbContext.TestCodeMappings.Remove(entity);

        await dbContext.SaveChangesAsync(
            cancellationToken);

        return true;
    }

    // =====================================================
    // GET BY ID
    // =====================================================

    public async Task<TestCodeMapping?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        await using LabAnalyzerDbContext dbContext =
            await _dbContextFactory.CreateDbContextAsync(
                cancellationToken);

        TestCodeMappingEntity? entity =
            await dbContext.TestCodeMappings
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);

        return entity is null
            ? null
            : ToDomain(entity);
    }

    // =====================================================
    // GET BY ANALYZER
    // =====================================================

    public async Task<IReadOnlyCollection<TestCodeMapping>>
        GetByAnalyzerIdAsync(
            Guid analyzerId,
            CancellationToken cancellationToken = default)
    {
        await using LabAnalyzerDbContext dbContext =
            await _dbContextFactory.CreateDbContextAsync(
                cancellationToken);

        List<TestCodeMappingEntity> entities =
            await dbContext.TestCodeMappings
                .AsNoTracking()
                .Where(
                    x => x.AnalyzerId == analyzerId)
                .OrderBy(
                    x => x.AnalyzerTestCode)
                .ToListAsync(
                    cancellationToken);

        return entities
            .Select(ToDomain)
            .ToList();
    }

    // =====================================================
    // FIND MAPPING
    // =====================================================

    public async Task<TestCodeMapping?> FindAsync(
     Guid analyzerId,
     string analyzerTestCode,
     CancellationToken cancellationToken = default)
    {
        if (analyzerId == Guid.Empty)
        {
            return null;
        }

        if (string.IsNullOrWhiteSpace(analyzerTestCode))
        {
            return null;
        }

        string normalizedCode =
            analyzerTestCode.Trim();

        await using LabAnalyzerDbContext dbContext =
            await _dbContextFactory.CreateDbContextAsync(
                cancellationToken);

        TestCodeMappingEntity? entity =
            await dbContext.TestCodeMappings
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x =>
                        x.AnalyzerId == analyzerId &&
                        x.AnalyzerTestCode == normalizedCode &&
                        x.IsActive,
                    cancellationToken);

        if (entity is null)
        {
            System.Diagnostics.Debug.WriteLine(
                "TEST CODE REPOSITORY -> NO ENTITY FOUND");

            return null;
        }

        System.Diagnostics.Debug.WriteLine(
            "========== REPOSITORY RESULT ==========");

        System.Diagnostics.Debug.WriteLine(
            $"Entity Id = [{entity.Id}]");

        System.Diagnostics.Debug.WriteLine(
            $"Entity AnalyzerId = [{entity.AnalyzerId}]");

        System.Diagnostics.Debug.WriteLine(
            $"Entity AnalyzerTestCode = [{entity.AnalyzerTestCode}]");

        System.Diagnostics.Debug.WriteLine(
            $"Entity StandardTestCode = [{entity.StandardTestCode}]");

        System.Diagnostics.Debug.WriteLine(
            $"Entity StandardTestName = [{entity.StandardTestName}]");

        System.Diagnostics.Debug.WriteLine(
            $"Entity IsActive = [{entity.IsActive}]");

        TestCodeMapping mapping =
            ToDomain(entity);

        System.Diagnostics.Debug.WriteLine(
            "========== DOMAIN RESULT ==========");

        System.Diagnostics.Debug.WriteLine(
            $"Domain AnalyzerTestCode = [{mapping.AnalyzerTestCode}]");

        System.Diagnostics.Debug.WriteLine(
            $"Domain StandardTestCode = [{mapping.StandardTestCode}]");

        System.Diagnostics.Debug.WriteLine(
            $"Domain IsActive = [{mapping.IsActive}]");

        System.Diagnostics.Debug.WriteLine(
            "===================================");

        return mapping;
    }

    // =====================================================
    // DOMAIN → ENTITY
    // =====================================================

    private static TestCodeMappingEntity ToEntity(
        TestCodeMapping mapping)
    {
        return new TestCodeMappingEntity
        {
            Id =
                mapping.Id == Guid.Empty
                    ? Guid.NewGuid()
                    : mapping.Id,

            AnalyzerId =
                mapping.AnalyzerId,

            AnalyzerTestCode =
                mapping.AnalyzerTestCode,

            StandardTestCode =
                mapping.StandardTestCode,

            StandardTestName =
                mapping.StandardTestName,

            AnalyzerTestName =
                mapping.AnalyzerTestName,

            ExpectedUnit =
                mapping.ExpectedUnit,

            StandardUnit =
                mapping.StandardUnit,

            IsActive =
                mapping.IsActive
        };
    }

    // =====================================================
    // ENTITY → DOMAIN
    // =====================================================

    private static TestCodeMapping ToDomain(
        TestCodeMappingEntity entity)
    {
        return new TestCodeMapping
        {
            Id =
                entity.Id,

            AnalyzerId =
                entity.AnalyzerId,

            AnalyzerTestCode =
                entity.AnalyzerTestCode,

            StandardTestCode =
                entity.StandardTestCode,

            StandardTestName =
                entity.StandardTestName,

            AnalyzerTestName =
                entity.AnalyzerTestName,

            ExpectedUnit =
                entity.ExpectedUnit,

            StandardUnit =
                entity.StandardUnit,

            IsActive =
                entity.IsActive
        };
    }
}